using System;
using Cameca.CustomAnalysis.Interface;
using Prism.Events;
using Prism.Mvvm;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisViewModelBase : BindableBase, IDisposable
{
	protected readonly IAnalysisCoreServices coreServices;
	protected readonly IEventAggregator eventAggregator;
	protected readonly IDisposableCollection<SubscriptionToken> managedSubscriptions = new DisposableList<SubscriptionToken>();

	protected Guid InstanceId => coreServices.IdProvider.Get(this);

	protected Guid NodeId { get; private set; } = Guid.Empty;

	protected AnalysisViewModelBase(IAnalysisCoreServices coreServices)
	{
		this.coreServices = coreServices;
		this.eventAggregator = coreServices.EventAggregator;

		managedSubscriptions.Add(eventAggregator.SubscribeViewModelCreated(OnCreated, InstanceIdFilter));
	}

	protected virtual void OnCreated(ViewModelCreatedEventArgs eventArgs)
	{
		NodeId = eventArgs.NodeId;
	}


	protected virtual bool InstanceIdFilter(IViewModelTargetEvent targetEventArgs) => targetEventArgs.ViewModelId == InstanceId;

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			managedSubscriptions.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
