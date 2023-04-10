using System.Collections.Generic;
using System.Linq;

namespace Cameca.CustomAnalysis.Utilities;

public static class PersistedIonDataSectionExtensions
{
	public static List<PersistedIonDataSection> FromJsonModels(this IEnumerable<PersistedIonDataSectionJsonModel>? sectionJsonModels)
	{
		return (sectionJsonModels ?? Enumerable.Empty<PersistedIonDataSectionJsonModel>()).Select(JsonModelMapper.FromJsonModel).ToList();
	}

	public static List<PersistedIonDataSection> MergeBySectionName(this IEnumerable<PersistedIonDataSection> first, IEnumerable<PersistedIonDataSection>? second)
	{
		var returnList = first.ToList();
		foreach (var section in second ?? Enumerable.Empty<PersistedIonDataSection>())
		{
			// Check if any of the defined sections to be written to disk are not in the save list. This is likely due to loading an old save version
			if (returnList.All(s => section.IonDataSection?.Name != s.IonDataSection?.Name))
				returnList.Add(section);
		}
		return returnList;
	}
}
