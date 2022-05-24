using System;
using Cameca.CustomAnalysis.Interface;
using Prism.Events;
using Prism.Mvvm;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class CoreViewModelBase<TServices> : BindableBase, IDisposable
	where TServices : ICoreNodeServices
{
	protected TServices Services { get; }

	protected Guid InstanceId => Services.IdProvider.Get(this);

	protected IDisposableCollection<SubscriptionToken> ManagedSubscriptions { get; }

	protected CoreViewModelBase(TServices services)
	{
		Services = services;
		ManagedSubscriptions = new DisposableList<SubscriptionToken>();
		
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeViewModelCreated(OnCreatedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeViewModelActivated(OnActivatedCore, InstanceIdFilter));
		ManagedSubscriptions.Add(Services.EventAggregator.SubscribeViewModelClosed(OnClosedCore, InstanceIdFilter));
	}

	internal virtual void OnCreatedCore(ViewModelCreatedEventArgs eventArgs)
	{
		OnCreated(eventArgs);
	}

	internal virtual void OnActivatedCore(ViewModelActivatedEventArgs eventArgs)
	{
		OnActivated(eventArgs);
	}

	internal virtual void OnClosedCore(ViewModelClosedEventArgs eventArgs)
	{
		OnClosed(eventArgs);
	}

	protected virtual void OnCreated(ViewModelCreatedEventArgs eventArgs) { }

	protected virtual void OnActivated(ViewModelActivatedEventArgs eventArgs) { }

	protected virtual void OnClosed(ViewModelClosedEventArgs eventArgs) { }

	protected virtual bool InstanceIdFilter(IViewModelTargetEvent targetEventArgs)
		=> targetEventArgs.ViewModelId == InstanceId;

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
