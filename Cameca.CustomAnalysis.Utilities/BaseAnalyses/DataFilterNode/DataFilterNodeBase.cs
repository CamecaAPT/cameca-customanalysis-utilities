using System;
using System.Collections.Generic;
using System.Threading;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class DataFilterNodeBase : DataFilterNodeBase<IDataFilterNodeBaseServices>
{
	protected DataFilterNodeBase(IDataFilterNodeBaseServices services) : base(services) { }
}

[NodeType(NodeType.DataFilter)]
public abstract class DataFilterNodeBase<TServices> : CoreNodeBase<TServices>
	where TServices : IDataFilterNodeBaseServices
{
	protected DataFilterNodeBase(TServices services) : base(services) { }

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.DataFilterProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegate;
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


	protected abstract IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token);

	protected void FilterDataChanged() => Services.DataFilterProvider.Resolve(InstanceId)?.FilterDataChanged();
}
