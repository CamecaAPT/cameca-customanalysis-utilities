using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisFilterNodeBase : AnalysisFilterNodeBase<IAnalysisFilterNodeBaseServices>
{
	protected AnalysisFilterNodeBase(IAnalysisFilterNodeBaseServices services) : base(services) { }
}

[NodeType(NodeType.DataFilter)]
public abstract class AnalysisFilterNodeBase<TServices> : AnalysisNodeBase<TServices>
	where TServices : IAnalysisFilterNodeBaseServices
{
	protected AnalysisFilterNodeBase(TServices services) : base(services)
	{
	}

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.DataFilterProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegateAsync;
			dataFilterInterceptor.IsInverted = IsInverted;
		}
	}

	protected virtual string? FilterCalculationMessage => null;

	protected virtual bool IsInverted => false;

#pragma warning disable CS1998
	protected virtual async IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegateAsync(IIonData ownerIonData, IProgress<double>? progress, [EnumeratorCancellation]CancellationToken token)
#pragma warning restore CS1998
	{
		foreach (var memory in GetIndicesDelegate(ownerIonData, progress, token))
		{
			yield return memory;
		}
	}

	protected virtual IEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token)
	{
		yield return ReadOnlyMemory<ulong>.Empty;
	}
}
