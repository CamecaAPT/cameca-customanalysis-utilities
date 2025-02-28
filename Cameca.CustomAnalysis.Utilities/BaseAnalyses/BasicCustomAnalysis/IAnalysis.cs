using Cameca.CustomAnalysis.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysis<in TProperties>
{
	string UpdateProgressText => "Updating";

#pragma warning disable CS1998
	async IAsyncEnumerable<ReadOnlyMemory<ulong>>? FilterAsync(
		IIonData ionData,
		TProperties properties,
		IResources resources,
		IProgress<double>? progress,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		if (Filter(ionData, properties, resources, progress, cancellationToken) is { } syncEnumerable)
		{
			foreach (var memory in syncEnumerable)
			{
				yield return memory;
			}
		}
	}
#pragma warning restore CS1998

	IEnumerable<ReadOnlyMemory<ulong>>? Filter(
		IIonData ionData,
		TProperties properties,
		IResources resources,
		IProgress<double>? progress,
		CancellationToken cancellationToken) => Array.Empty<ReadOnlyMemory<ulong>>();
	
	Task<bool> Update(
		TProperties properties,
		IResources resources,
		IProgress<double>? progress,
		CancellationToken cancellationToken);
}
