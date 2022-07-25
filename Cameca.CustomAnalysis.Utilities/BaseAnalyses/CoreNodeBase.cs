using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class CoreNodeBase<TServices> : IDisposable where TServices : ICoreNodeServices
{
	protected INodeDataState? DataState { get; private set; }
	protected ICanSaveState? CanSaveState { get; private set; }

	protected TServices Services { get; }

	protected Guid InstanceId => Services.IdProvider.Get(this);

	protected IDisposableCollection<SubscriptionToken> ManagedSubscriptions { get; }

	protected virtual IEnumerable<IMenuItem> ContextMenuItems => Enumerable.Empty<IMenuItem>();

	protected CoreNodeBase(TServices services)
	{
		Services = services;
		ManagedSubscriptions = new DisposableList<SubscriptionToken>();
		
		// Register events
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeCreated(OnCreatedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeSaved(OnSavedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeAdded(OnAddedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeInteracted(NodeInteractedRouter, InstanceIdFilter));
	}

	internal virtual void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		OnInstantiatedCore(eventArgs);
		// Call overridable handler after all internal setup complete
		OnCreated(eventArgs);
	}

	internal virtual void OnSavedCore(NodeSavedEventArgs eventArgs)
	{
		if (CanSaveState is { } canSaveState)
		{
			canSaveState.CanSave = false;
		}
	}

	internal virtual void OnAddedCore(NodeAddedEventArgs eventArgs)
	{
		OnAdded(eventArgs);
	}

	internal virtual void NodeInteractedRouter(NodeInteractedEventArgs eventArgs)
	{
		switch (eventArgs.Interaction)
		{
			case InteractionType.DoubleClick:
				OnDoubleClickCore();
				break;
		}
	}

	internal virtual void OnInstantiatedCore(NodeCreatedEventArgs eventArgs)
	{
		if (Services.NodePersistenceProvider.Resolve(InstanceId) is { } saveInterceptor)
		{
			saveInterceptor.SaveDelegate = GetSaveContent;
			saveInterceptor.SavePreviewDelegate = GetPreviewSaveContent;
		}
		CanSaveState = Services.CanSaveStateProvider.Resolve(InstanceId);
		DataState = Services.DataStateProvider.Resolve(InstanceId);
		if (DataState is not null)
		{
			DataState.PropertyChanged += DataStateOnPropertyChangedRouter;
		}
		if (Services.MenuFactoryProvider.Resolve(InstanceId) is { } menuFactory)
		{
			menuFactory.ContextMenuItems = ContextMenuItems;
		}
	}

	internal virtual void OnDoubleClickCore()
	{
		OnDoubleClick();
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

	protected Task<IIonData?> GetIonData(IProgress<double>? progress = null, CancellationToken cancellationToken = default)
		=> Services.IonDataProvider.GetIonData(InstanceId, progress, cancellationToken);

	protected virtual byte[]? GetSaveContent() => null;

	protected virtual byte[]? GetPreviewSaveContent() => null;

	protected virtual void OnCreated(NodeCreatedEventArgs eventArgs) { }

	protected virtual void OnAdded(NodeAddedEventArgs eventArgs) { }

	protected virtual void OnAfterCreated(NodeCreatedEventArgs eventArgs) { }

	protected virtual void OnDoubleClick() { }

	protected virtual void OnDataIsValidChanged(bool isValid) { }

	protected virtual void OnDataErrorStateChanged(bool isErrorState) { }

	protected virtual bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> targetEventArgs.NodeId == InstanceId;

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (DataState is not null)
			{
				DataState.PropertyChanged -= DataStateOnPropertyChangedRouter;
			}
			ManagedSubscriptions.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
