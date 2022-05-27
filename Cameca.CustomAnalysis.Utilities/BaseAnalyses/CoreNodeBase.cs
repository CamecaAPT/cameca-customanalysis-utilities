using System;
using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class CoreNodeBase<TServices> : IDisposable where TServices : ICoreNodeServices
{
	protected TServices Services { get; }

	protected Guid InstanceId => Services.IdProvider.Get(this);

	protected IDisposableCollection<SubscriptionToken> ManagedSubscriptions { get; }

	protected CoreNodeBase(TServices services)
	{
		Services = services;
		ManagedSubscriptions = new DisposableList<SubscriptionToken>();
		
		// Register events
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeCreated(OnCreatedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeLoaded(OnLoadedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeAdded(OnAddedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeAfterCreated(OnAfterCreatedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeNodeInteraction(NodeInteractionRouter, InstanceIdFilter));
	}

	internal virtual void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		OnInstantiatedCore(eventArgs);
		OnCreated(eventArgs);
	}

	internal virtual void OnLoadedCore(NodeLoadedEventArgs eventArgs)
	{
		OnInstantiatedCore(eventArgs);
		OnLoaded(eventArgs);
	}

	internal virtual void OnAddedCore(NodeAddedEventArgs eventArgs)
	{
		OnAdded(eventArgs);
	}

	internal virtual void OnAfterCreatedCore(NodeAfterCreatedEventArgs eventArgs)
	{
		OnAfterCreated(eventArgs);
	}

	internal virtual void NodeInteractionRouter(NodeInteractionEventArgs eventArgs)
	{
		switch (eventArgs.Interaction)
		{
			case NodeInteractionType.DoubleClick:
				OnDoubleClickCore();
				break;
		}
	}

	internal virtual void OnInstantiatedCore(INodeInstantiatedEventArgs eventArgs)
		=> OnInstantiated(eventArgs);

	internal virtual void OnDoubleClickCore()
	{
		OnDoubleClick();
	}

	protected virtual void OnCreated(NodeCreatedEventArgs eventArgs) { }

	protected virtual void OnLoaded(NodeLoadedEventArgs eventArgs) { }

	protected virtual void OnInstantiated(INodeInstantiatedEventArgs eventArgs) { }

	protected virtual void OnAdded(NodeAddedEventArgs eventArgs) { }

	protected virtual void OnAfterCreated(NodeAfterCreatedEventArgs eventArgs) { }

	protected virtual void OnDoubleClick() { }

	protected virtual bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> targetEventArgs.NodeId == InstanceId;

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			ManagedSubscriptions.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
