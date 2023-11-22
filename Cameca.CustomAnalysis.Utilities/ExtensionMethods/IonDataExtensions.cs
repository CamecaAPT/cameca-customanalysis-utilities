using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;
using CommunityToolkit.HighPerformance.Buffers;

namespace Cameca.CustomAnalysis.Utilities;

public static class IonDataExtensions
{
	/// <summary>
	/// Wraps the <see cref="IIonData.CreateSectionDataEnumerator" /> method to return an enumerable for easier use in foreach loops.
	/// </summary>
	/// <param name="ionData"></param>
	/// <param name="sections"></param>
	/// <returns></returns>
	public static IEnumerable<IChunkState> CreateSectionDataEnumerable(this IIonData ionData, params string[] sections)
	{
		using var enumerator = ionData.CreateSectionDataEnumerator(sections);
		while (enumerator.MoveNext())
		{
			// Intended to simplify foreach loops, where we likely don't want to keep references beyond the loop.
			// Therefore this utility will managed disposing the object for the caller.
			// If a need exists for an enumerable that does not dispose on enumeration, that can be added seperately
			using (enumerator.Current)
			{
				yield return enumerator.Current;
			}
		}
	}

	#region IIonDataProvider Extensions
	public static async Task<IIonData?> GetIonData(this IIonDataProvider ionDataProvider, Guid nodeId, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		return ionDataProvider.Resolve(nodeId) is { } resolver ? await resolver.GetIonData(progress, cancellationToken) : null;
	}

	public static IIonData? GetResolvedIonData(this IIonDataProvider ionDataProvider, Guid nodeId)
	{
		return ionDataProvider.Resolve(nodeId)?.GetValidIonData();
	}
	public static async Task<IIonData?> GetOwnerIonData(this IIonDataProvider ionDataProvider, Guid nodeId, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		return ionDataProvider.Resolve(nodeId) is { OwnerNodeId: { } ownerNodeId }
			? await ionDataProvider.GetIonData(ownerNodeId, progress, cancellationToken)
			: null;
	}

	public static IIonData? GetResolvedOwnerIonData(this IIonDataProvider ionDataProvider, Guid nodeId)
	{
		return ionDataProvider.Resolve(nodeId) is { OwnerNodeId: { } ownerNodeId }
			? ionDataProvider.GetResolvedIonData(ownerNodeId)
			: null;
	}

	public static IIonData? GetResolvedRootIonData(this IIonDataProvider ionDataProvider, Guid nodeId)
	{
		var rootNodeId = GetRootIonDataNodeId(ionDataProvider, nodeId);
		return ionDataProvider.GetResolvedIonData(rootNodeId);
	}

    private static Guid GetRootIonDataNodeId(IIonDataProvider ionDataProvider, Guid nodeId)
    {
        Guid nodeIdPtr = nodeId;
        Guid? ownerNodeId = ionDataProvider.Resolve(nodeIdPtr)?.OwnerNodeId;
        while (ownerNodeId.HasValue && ownerNodeId.Value != Guid.Empty)
        {
            nodeIdPtr = ownerNodeId.Value;
            ownerNodeId = ionDataProvider.Resolve(nodeIdPtr)?.OwnerNodeId;
        }
        return nodeIdPtr;
    }
	#endregion

	/// <summary>
	/// Checks <see cref="IIonData"/> for all given sections. If present, returns <c>true</c>.
	/// If not present, then attempt to use reconstruction section service to recalculate from source data.
	/// If for any reason section data is unable to be added, then return <c>false</c>.
	/// </summary>
	/// <remarks>
	/// True result indicates that all listed sections are available and it is safe to read the associated data.
	/// False result means that one or more of the requested sections are not available,
	/// and additionally can not be made available using <see cref="IReconstructionSections" />.
	/// <br />
	/// If false, calling methods should gracefully abort the request that requires the given sections,
	/// or adjust the requirements to use fewer or different sections before calling this method again.
	/// </remarks>
	/// <param name="ionData"></param>
	/// <param name="requiredSectionNames"></param>
	/// <param name="reconstructionSections"></param>
	/// <param name="progress"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>If after the method returns, all requested sections are available, return <c>true</c>, else <c>false</c></returns>
	public static async Task<bool> EnsureSectionsAvailable(
		this IIonData ionData,
		IEnumerable<string> requiredSectionNames,
		IReconstructionSections? reconstructionSections,
		IProgress<double>? progress = null,
		CancellationToken cancellationToken = default)
	{
		var missingSections = requiredSectionNames.Except(ionData.Sections.Keys).ToList();
		// If no sections are missing, all sections are available
		if (!missingSections.Any()) { return true; }

		// Otherwise dynamically add sections if possible.
		if (reconstructionSections is null)
		{
			// Missing sections, but reconstruction sections service was not provided. All sections are not available.
			return false;
		}

		// Ensure the add section feature is available for use
		if (!reconstructionSections.IsAddSectionAvailable)
		{
			// Add section feature is unavailable, usually due to missing experiment data file
			// AP Suite may have lost track of the file due to data file path settings or the reconstruction may have been imported
			// or created from an ROI of another reconstruction, situations for which no experiment file will be present.
			return false;
		}

		// Check that the add section feature can recalculate the missing sections.
		// Different inputs may have different sections available for recalculations. Just having this feature available
		// does not necessarily guarantee all requested sections can be calculated.
		var availableSectionNames = (await reconstructionSections.GetAvailableSections(progress, cancellationToken))
			.Select(x => x.Name)
			.ToList();
		if (missingSections.Except(availableSectionNames).Any())
		{
			// If any missing section cannot be recalculated, then all requested sections can not be considered available
			// Assuming we require all sections, no section will be recalculated here, not even those that may be avaialbe
			return false;
		}

		// Add recalculated sections as virtual data sections. This only should return false at this point for
		// exceptional circumstances (file corruption, etc)
		return await reconstructionSections.AddSections(missingSections, false, progress, cancellationToken);
	}

	#region Buffers I/O
	/// <summary>
	/// Reads the contents of an <see cref="IIonData" /> section to a <see cref="ReadOnlySequence{T}" /> instance.
	/// This supports collections longer than the maximum length of a C# array and allows slicing by long values
	/// instead of by int.
	/// </summary>
	/// <remarks>
	/// Combined with other extension methods in this package, this can dramatically simplify working with section
	/// data while supporting ion counts that potentially exceed the maximum size of standard C# arrays.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="ionData"></param>
	/// <param name="sectionName"></param>
	/// <returns></returns>
	internal static ReadOnlySequence<T> ReadSection<T>(this IIonData ionData, string sectionName) where T : unmanaged
	{
		// Does not exist
		if (!ionData.Sections.ContainsKey(sectionName))
			return ReadOnlySequence<T>.Empty;

		return ionData.CreateSectionDataEnumerable(sectionName)
			.Select(chunk => chunk.ReadSectionData<T>(sectionName))
			.AsReadOnlySequence();
	}

	/// <summary>
	/// Writes the contents of a <see cref="ReadOnlySequence{T}" /> instance to an <see cref="IIonData" /> section.
	/// This supports collections longer than the maximum length of a C# array.
	/// </summary>
	/// <remarks>
	/// Combined with other extension methods in this package, this can dramatically simplify working with section
	/// data while supporting ion counts that potentially exceed the maximum size of standard C# arrays.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="ionData"></param>
	/// <param name="sectionName"></param>
	/// <param name="data"></param>
	/// <param name="sectionDefinition"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	internal static bool WriteSection<T>(this IIonData ionData, string sectionName, ReadOnlySequence<T> data, IonDataSectionDefinition? sectionDefinition = null)
		where T : unmanaged
	{
		// If not already exists, create section
		if (!ionData.Sections.ContainsKey(sectionName))
		{
			if (sectionDefinition is null)
				throw new InvalidOperationException($"Section does not exist and {nameof(sectionDefinition)} is null");
			int size = Marshal.SizeOf<T>();
			ionData.AddSection(
				sectionDefinition.Name,
				sectionDefinition.Type,
				(ulong)data.Length,
				(uint)(size * 8),
				sectionDefinition.Unit,
				sectionDefinition.ValuePerRecord,
				isVirtual: !ionData.CanWrite);
		}

		long chunkOffset = 0L;
		foreach (var chunk in ionData.CreateSectionDataEnumerable(sectionName))
		{
			var writeSlice = data.Slice(chunkOffset, chunk.Length);
			if (writeSlice.IsSingleSegment)
			{
				chunk.WriteSectionData<T>(sectionName, writeSlice.First);

			}
			else
			{
				using MemoryOwner<T> buffer = MemoryOwner<T>.Allocate(chunk.Length);
				int bufferIndex = 0;
				foreach (var segment in writeSlice)
				{
					segment.CopyTo(buffer.Memory[bufferIndex..(bufferIndex + segment.Length)]);
					bufferIndex += segment.Length;
				}
				chunk.WriteSectionData<T>(sectionName, buffer.Memory);
			}
			chunkOffset += chunk.Length;
		}

		return true;
	}
	#endregion

	#region Stream I/O
	/// <summary>
	/// Write the contents of a <see cref="Stream" /> instance to a specified section by name.
	/// If the section does not exits, then the optional <see cref="IonDataSectionDefinition" /> instance is used to add a new virtual data section.
	/// If both the section does not exist and no section definition is not provided, then an <see cref="InvalidOperationException"/> will be thrown for the 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="stream">Stream source to write to <see cref="IIonData" /> section. Assume stream is the proper size for writing to the section.</param>
	/// <param name="ionData"></param>
	/// <param name="sectionName"></param>
	/// <param name="sectionDefinition"></param>
	/// <exception cref="InvalidOperationException">Section does not currently exist in and no section definition was given</exception>
	/// <exception cref="InvalidOperationException">Stream size was not the expected length required to write the full ion data section</exception>
	/// <exception cref="ArgumentException">Input stream was not readable</exception>
	internal static void WriteStreamToSection<T>(this IIonData ionData, string sectionName, Stream stream, IonDataSectionDefinition? sectionDefinition = null) where T : unmanaged
	{
		if (!stream.CanRead) throw new ArgumentException("Stream must be readable", nameof(stream));

		ionData.WriteSection(sectionName, ReadAsEnumerableReadOnlyMemory().AsReadOnlySequence(), sectionDefinition);

		// Stream is read and returns in enumerable to take advantage of conversion to ReadOnlySequence method
		IEnumerable<ReadOnlyMemory<T>> ReadAsEnumerableReadOnlyMemory()
		{
			foreach (var chunk in ionData.CreateSectionDataEnumerable())
			{
				using var buffer = MemoryOwner<T>.Allocate(chunk.Length);
				var bytesBuffer = MemoryMarshal.AsBytes(buffer.Span);
				if (stream.Read(bytesBuffer) != bytesBuffer.Length)
					throw new InvalidOperationException("Stream size must be the correct length to write to ion data section");
				yield return buffer.Memory;
			}
		}
	}

	/// <inheritdoc cref="WriteStreamToSection{T}"/>
	/// <remarks>
	/// Usually the generic method would call a non-generic method instead of reflection to do the opposite.
	/// In this case there should be an unmanaged type constraint on the System.RuntimeType, and that can only be
	/// accomplished with a generic type constraint. Using reflection to create a generic method of the given
	/// type will throw an ArgumentNullException if the runtime type does not match the required type constraint.
	/// </remarks>
	internal static void WriteStreamToSection(this IIonData ionData, string sectionName, Type type, Stream stream, IonDataSectionDefinition? sectionDefinition = null)
	{
		MethodInfo methodInfo;
		try
		{
			methodInfo = typeof(IonDataExtensions)
				.GetMethods()
				.Single(x => x.IsGenericMethod && x.Name == nameof(WriteStreamToSection))
				.MakeGenericMethod(type);
		}
		catch (ArgumentException ex)
		{
			throw new ArgumentException("RuntimeType does not satisfy the required generic type constraints", nameof(type), ex);
		}
		methodInfo.Invoke(null, new object?[] { ionData, sectionName, stream, sectionDefinition });
	}

	/// <summary>
	/// Writes the contents of the <see cref="IIonData" /> section by name to the provided <see cref="Stream"/> open for writing.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="ionData"></param>
	/// <param name="sectionName"></param>
	/// <param name="stream"></param>
	/// <exception cref="ArgumentException">Stream is not writable</exception>
	internal static void WriteSectionToStream<T>(this IIonData ionData, string sectionName, Stream stream) where T : unmanaged
	{
		if (!stream.CanWrite) throw new ArgumentException("Stream must be writable", nameof(stream));
		var sequence = ionData.ReadSection<T>(sectionName);
		foreach (var memory in sequence)
		{
			var bytesBuffer = MemoryMarshal.AsBytes(memory.Span);
			stream.Write(bytesBuffer);
		}
	}

	/// <inheritdoc cref="WriteSectionToStream{T}"/>
	/// <remarks>
	/// Usually the generic method would call a non-generic method instead of reflection to do the opposite.
	/// In this case there should be an unmanaged type constraint on the System.RuntimeType, and that can only be
	/// accomplished with a generic type constraint. Using reflection to create a generic method of the given
	/// type will throw an ArgumentNullException if the runtime type does not match the required type constraint.
	/// </remarks>
	internal static void WriteSectionToStream(this IIonData ionData, string sectionName, Type type, Stream stream)
	{
		MethodInfo methodInfo;
		try
		{
			methodInfo = typeof(IonDataExtensions)
				.GetMethods()
				.Single(x => x.IsGenericMethod && x.Name == nameof(WriteSectionToStream))
				.MakeGenericMethod(type);
		}
		catch (ArgumentException ex)
		{
			throw new ArgumentException("RuntimeType does not satisfy the required generic type constraints", nameof(type), ex);
		}
		methodInfo.Invoke(null, new object?[] { ionData, sectionName, stream });
	}
	#endregion
}
