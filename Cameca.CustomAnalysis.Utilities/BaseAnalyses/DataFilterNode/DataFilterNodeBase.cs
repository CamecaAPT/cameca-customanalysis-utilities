using System;
using System.Collections.Generic;
using System.Threading;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class DataFilterNodeBase : CoreNodeBase<IDataFilterNodeBaseServices>
{
	protected INodeDataState? DataState;

	protected DataFilterNodeBase(IDataFilterNodeBaseServices services) : base(services) { }
	
	internal override void OnInstantiatedCore(INodeInstantiatedEventArgs eventArgs)
	{
		if (Services.NodeSaveInterceptorProvider.Resolve(InstanceId) is { } saveInterceptor)
		{
			saveInterceptor.SaveInterceptor = OnSave;
			saveInterceptor.SavePreviewInterceptor = OnPreviewSave;
		}
		DataState = Services.DataStateProvider.Resolve(InstanceId);
		if (Services.DataFilterInterceptorProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegate;
			dataFilterInterceptor.IsInverted = IsInverted;
		}
	}

	protected virtual string? FilterCalculationMessage => null;

	protected virtual bool IsInverted => false;

	protected abstract IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token);

	protected virtual byte[]? OnSave() => null;

	protected virtual byte[]? OnPreviewSave() => OnSave();
}
