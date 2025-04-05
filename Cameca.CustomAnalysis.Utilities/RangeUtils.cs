using Cameca.CustomAnalysis.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Cameca.CustomAnalysis.Utilities;

public static class RangeUtils
{
	public static bool RangesOverlap(IEnumerable<IonTypeInfoRange> ranges, IonTypeInfoRange testRange)
	{
		return ranges.Any(r => RangesOverlap(r, testRange));
	}

	public static bool RangesOverlap(IonTypeInfoRange range1, IonTypeInfoRange range2)
	{
		return range1.Min < range2.Max && range1.Max > range2.Min;
	}

	public static IEnumerable<IonTypeInfoRange> ConcatNonOverlappingRanges(this IEnumerable<IonTypeInfoRange> existingRanges, IEnumerable<IonTypeInfoRange> newRanges)
	{
		var existingCollection = existingRanges is ICollection<IonTypeInfoRange> collection ? collection : existingRanges.ToList();
		foreach (var range in existingCollection)
		{
			yield return range;
		}
		foreach (var newRange in newRanges)
		{
			if (!RangesOverlap(existingCollection, newRange))
			{
				yield return newRange;
			}
		}
	}
}
