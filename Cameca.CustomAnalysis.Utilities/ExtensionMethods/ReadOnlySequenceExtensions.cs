using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities;

public static class ReadOnlySequenceExtensions
{
	/// <summary>
	/// Create a new <see cref="ReadOnlySequence{T}" /> instance filled with the given initial value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="length"></param>
	/// <param name="value"></param>
	/// <param name="maxArrayLength">
	/// Default from: https://learn.microsoft.com/en-us/dotnet/api/system.array?view=net-7.0#remarks
	/// </param>
	/// <returns></returns>
	public static ReadOnlySequence<T> Create<T>(ulong length, T value = default, int maxArrayLength = 0X7FEFFFFF) where T : struct
	{
		if (maxArrayLength <= 0) throw new ArgumentOutOfRangeException(nameof(maxArrayLength), "Must be greater than zero");
		if (length == 0) return ReadOnlySequence<T>.Empty;
		return InnerEnumerableGenerator(length, value, maxArrayLength)
			.AsReadOnlySequence();

		static IEnumerable<ReadOnlyMemory<T>> InnerEnumerableGenerator(ulong length, T value, int maxArrayLength)
		{
			ulong remainingLength = length;
			do
			{
				int yieldArraySize = remainingLength <= (ulong)maxArrayLength
					? (int)remainingLength
					: maxArrayLength;
				var buffer = new T[yieldArraySize];
				if (!Equals(value, default))
					Array.Fill<T>(buffer, value);
				yield return buffer;
				remainingLength -= (ulong)yieldArraySize;
			} while (remainingLength > 0);
		}
	}


	/// <summary>
	/// Creates a new array from a ReadOnlySequence.
	/// Elements are copied to the new array to ensure contiguous data.
	/// </summary>
	/// <remarks>
	/// Useful if slicing a <see cref="ReadOnlySequence{T}" /> where the sliced output is known
	/// to fit into an array but the underlying sequence segments may not be contiguous.
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="sequence"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static T[] ToArray<T>(this ReadOnlySequence<T> sequence)
	{
		if (sequence.IsSingleSegment)
		{
			return sequence.First.ToArray();
		}
		// Guard against sequences too long for which to create an array
		if (!ValidateArrayMaxLength(sequence.Length))
		{
			throw new ArgumentOutOfRangeException(nameof(sequence), "Sequence length must be less than or equal to Array.MaxLength");
		}

		var array = new T[(int)sequence.Length];  // Safe cast due to previous guard clause
		var enumerator = sequence.GetEnumerator();
		int offset = 0;
		while (enumerator.MoveNext())
		{
			enumerator.Current.CopyTo(array[offset..(offset + enumerator.Current.Length)]);
			offset += enumerator.Current.Length;
		}
		return array;
	}

	public static IEnumerable<T> MarshalToEnumerable<T>(this ReadOnlySequence<T> sequence)
	{
		return ToMemoryEnumerable(sequence).SelectMany(innerEnumerable => innerEnumerable);

		static IEnumerable<IEnumerable<T>> ToMemoryEnumerable(ReadOnlySequence<T> sequence)
		{
			var enumerator = sequence.GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return MemoryMarshal.ToEnumerable(enumerator.Current);
			}
		}
	}

	[DebuggerStepThrough]
	private static bool ValidateArrayMaxLength(long length)
	{
		// https://learn.microsoft.com/en-us/dotnet/api/system.array.maxlength?source=recommendations
		// There is no guarantee that an allocation under this length will succeed, but all attempts to allocate a larger array will fail.
		return length <= Array.MaxLength;
	}
}
