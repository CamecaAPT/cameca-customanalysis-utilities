using System;
using System.Collections.Generic;
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
}
