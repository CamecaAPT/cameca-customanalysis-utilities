using System;
using System.Collections.Generic;
using System.Threading;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class DataFilterNodeBase : DataFilterNodeBase<IDataFilterNodeBaseServices>
{
	protected DataFilterNodeBase(IDataFilterNodeBaseServices services) : base(services) { }
}


public abstract class DataFilterNodeBase<TServices> : CoreNodeBase<TServices>
	where TServices : IDataFilterNodeBaseServices
{
	protected DataFilterNodeBase(TServices services) : base(services) { }

	internal override void OnInstantiatedCore(INodeInstantiatedEventArgs eventArgs)
	{
		base.OnInstantiatedCore(eventArgs);
		if (Services.DataFilterProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegate;
			dataFilterInterceptor.IsInverted = IsInverted;
		}
	}

	protected virtual string? FilterCalculationMessage => null;

	protected virtual bool IsInverted => false;

	protected abstract IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token);
}
