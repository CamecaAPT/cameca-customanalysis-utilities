using System;
using System.Collections.Generic;
using System.Threading;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class DataFilterNodeBase : DataFilterNodeBase<IDataFilterNodeBaseServices>
{
	protected DataFilterNodeBase(IDataFilterNodeBaseServices services, ResourceFactory resourceFactory) : base(services, resourceFactory) { }
}

[NodeType(NodeType.DataFilter)]
public abstract class DataFilterNodeBase<TServices> : CoreNodeBase<TServices>
	where TServices : IDataFilterNodeBaseServices
{
	protected DataFilterNodeBase(TServices services, ResourceFactory resourceFactory) : base(services, resourceFactory) { }

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.DataFilterProvider.Resolve(Id) is { } dataFilterInterceptor)
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
		get => Services.DataFilterProvider.Resolve(Id)?.FilterProgressMessage;
		set
		{
			if (Services.DataFilterProvider.Resolve(Id) is { } dataFilterInterceptor)
			{
				dataFilterInterceptor.FilterProgressMessage = value;
			}
		}
	}

	protected bool FilterIsInverted
	{
		get => Services.DataFilterProvider.Resolve(Id)?.IsInverted ?? false;
		set
		{
			if (Services.DataFilterProvider.Resolve(Id) is { } dataFilterInterceptor)
			{
				dataFilterInterceptor.IsInverted = value;
			}
		}
	}


	protected abstract IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token);

	protected void FilterDataChanged() => Services.DataFilterProvider.Resolve(Id)?.FilterDataChanged();
}
