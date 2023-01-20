using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public static class IonDataExtensions
{
	public static IEnumerable<IChunkState> CreateSectionDataEnumerable(this IIonData ionData, params string[] sections)
	{
		var enumerator = ionData.CreateSectionDataEnumerator(sections);
		while (enumerator.MoveNext())
		{
			yield return enumerator.Current;
		}
	}

	public static async Task<IIonData?> GetIonData(this IIonDataProvider ionDataProvider, Guid nodeId, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		return ionDataProvider.Resolve(nodeId) is { } resolver ? await resolver.GetIonData(progress, cancellationToken) : null;
	}

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
		return await reconstructionSections.AddVirtualSections(missingSections, progress, cancellationToken);
	}
}
