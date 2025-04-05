using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class CoreNodeBase<TServices> : ObservableObject, IDisposable where TServices : ICoreNodeServices
{
	private readonly ResourceFactory resourceFactory;

	public INodeDataState? DataState { get; private set; }
	public ICanSaveState? CanSaveState { get; private set; }

	protected bool DataStateIsValid
	{
		get => DataState?.IsValid ?? true;
		set
		{
			if (DataState is not null)
				DataState.IsValid = value;
		}
	}

	protected bool CanSave
	{
		get => CanSaveState?.CanSave ?? false;
		set
		{
			if (CanSaveState is not null)
				CanSaveState.CanSave = value;
		}
	}

	protected TServices Services { get; }

	private Lazy<IResources> _lazyResources;
	public IResources Resources => _lazyResources.Value;

	public Guid Id => Services.IdProvider.Get(this);

	[Obsolete("Use Id property instead")]
	protected Guid InstanceId => Id;

	protected IDisposableCollection<SubscriptionToken> ManagedSubscriptions { get; }

	protected virtual IEnumerable<IMenuItem> ContextMenuItems => Enumerable.Empty<IMenuItem>();

	protected CoreNodeBase(TServices services, ResourceFactory resourceFactory)
	{
		Services = services;
		this.resourceFactory = resourceFactory;
		_lazyResources = new Lazy<IResources>(() => this.resourceFactory.CreateResource(Id));
		ManagedSubscriptions = new DisposableList<SubscriptionToken>();
		
		// Register events
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeCreated(OnCreatedCoreWrapper, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeSaved(OnSavedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeAdded(OnAddedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeInteracted(NodeInteractedRouter, InstanceIdFilter));
	}

	private void OnCreatedCoreWrapper(NodeCreatedEventArgs eventArgs)
	{
		OnCreatedCore(eventArgs);
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

	internal virtual void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		if (Services.NodePersistenceProvider.Resolve(Id) is { } saveInterceptor)
		{
			saveInterceptor.SaveDelegate = GetSaveContent;
			saveInterceptor.SavePreviewDelegate = GetPreviewSaveContent;
		}
		CanSaveState = Services.CanSaveStateProvider.Resolve(Id);
		DataState = Services.DataStateProvider.Resolve(Id);
		if (DataState is not null)
		{
			DataState.PropertyChanged += DataStateOnPropertyChangedRouter;
		}
		if (Services.MenuFactoryProvider.Resolve(Id) is { } menuFactory)
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
		=> Services.IonDataProvider.GetIonData(Id, progress, cancellationToken);

	protected virtual byte[]? GetSaveContent() => null;

	protected virtual byte[]? GetPreviewSaveContent() => null;

	protected virtual void OnCreated(NodeCreatedEventArgs eventArgs) { }

	protected virtual void OnAdded(NodeAddedEventArgs eventArgs) { }

	protected virtual void OnAfterCreated(NodeCreatedEventArgs eventArgs) { }

	protected virtual void OnDoubleClick() { }

	protected virtual void OnDataIsValidChanged(bool isValid) { }

	protected virtual void OnDataErrorStateChanged(bool isErrorState) { }

	protected bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> targetEventArgs.NodeId == Id;

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
