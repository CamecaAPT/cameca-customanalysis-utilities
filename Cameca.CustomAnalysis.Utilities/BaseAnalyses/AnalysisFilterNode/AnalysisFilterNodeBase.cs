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
		}
	}

	[Obsolete("Use FilterProgressMessage")]
	protected virtual string? FilterCalculationMessage => null;

	[Obsolete("Use FilterIsInverted")]
	protected virtual bool IsInverted => false;

	protected string? FilterProgressMessage
	{
		get => Services.DataFilterProvider.Resolve(InstanceId)?.FilterProgressMessage;
		set
		{
			if (Services.DataFilterProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
			{
				dataFilterInterceptor.FilterProgressMessage = value;
			}
		}
	}

	protected bool FilterIsInverted
	{
		get => Services.DataFilterProvider.Resolve(InstanceId)?.IsInverted ?? false;
		set
		{
			if (Services.DataFilterProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
			{
				dataFilterInterceptor.IsInverted = value;
			}
		}
	}

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

	protected void FilterDataChanged() => Services.DataFilterProvider.Resolve(InstanceId)?.FilterDataChanged();
}
