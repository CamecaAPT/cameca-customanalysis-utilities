using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class DataFilterNodeBase : CoreNodeBase<IDataFilterNodeBaseServices>
{
	protected INodeDataState? DataState;

	protected virtual IEnumerable<IMenuItem> ContextMenuItems => Enumerable.Empty<IMenuItem>();

	protected DataFilterNodeBase(IDataFilterNodeBaseServices services) : base(services) { }


	internal override void OnInstantiatedCore(INodeInstantiatedEventArgs eventArgs)
	{
		base.OnInstantiatedCore(eventArgs);
		if (Services.NodePersistenceProvider.Resolve(InstanceId) is { } saveInterceptor)
		{
			saveInterceptor.SaveDelegate = OnSave;
			saveInterceptor.SavePreviewDelegate = OnPreviewSave;
		}
		DataState = Services.DataStateProvider.Resolve(InstanceId);
		if (DataState is not null)
		{
			DataState.PropertyChanged += DataStateOnPropertyChangedRouter;
		}
		if (Services.MenuFactoryProvider.Resolve(InstanceId) is { } menuFactory)
		{
			menuFactory.ContextMenuItems = ContextMenuItems;
		}
		if (Services.DataFilterProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegate;
			dataFilterInterceptor.IsInverted = IsInverted;
		}
	}

	private void DataStateOnPropertyChangedRouter(object? sender, PropertyChangedEventArgs e)
	{
		if (DataState is null) return;  // Should never occur if only handling DataState property
		switch (e.PropertyName)
		{
			case nameof(INodeDataState.IsValid):
				OnDataIsValidChanged(DataState.IsValid);
				break;
			case nameof(INodeDataState.IsErrorState):
				OnDataErrorStateChanged(DataState.IsErrorState);
				break;
		}
	}

	protected virtual void OnDataIsValidChanged(bool isValid) { }

	protected virtual void OnDataErrorStateChanged(bool isErrorState) { }

	protected virtual string? FilterCalculationMessage => null;

	protected virtual bool IsInverted => false;

	protected abstract IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token);

	protected Task<IIonData?> GetIonData(IProgress<double>? progress = null, CancellationToken cancellationToken = default)
		=> Services.IonDataProvider.GetIonData(InstanceId, progress, cancellationToken);

	protected virtual byte[]? OnSave() => null;

	protected virtual byte[]? OnPreviewSave() => OnSave();

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (DataState is not null)
			{
				DataState.PropertyChanged -= DataStateOnPropertyChangedRouter;
			}
		}
		base.Dispose(disposing);
	}
}
