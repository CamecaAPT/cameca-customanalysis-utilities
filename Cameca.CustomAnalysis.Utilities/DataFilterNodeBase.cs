using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class DataFilterNodeBase
{
	protected readonly IAnalysisNodeBaseServices baseServices;
	private readonly INodeDataFilterInterceptorProvider dataFilterInterceptorProvider;
	protected readonly IEventAggregator eventAggregator;

	protected Guid InstanceId => baseServices.IdProvider.Get(this);

	protected INodeDataState? DataState { get; private set; }

	protected DataFilterNodeBase(
		IAnalysisNodeBaseServices baseServices,
		INodeDataFilterInterceptorProvider dataFilterInterceptorProvider)
	{
		this.baseServices = baseServices;
		this.dataFilterInterceptorProvider = dataFilterInterceptorProvider;
		eventAggregator = this.baseServices.EventAggregator;

		// Register events
		eventAggregator.SubscribeNodeAdded(OnAdded, InstanceIdFilter);
		eventAggregator.SubscribeNodeCreated(OnCreated, InstanceIdFilter);
		eventAggregator.SubscribeNodeInteraction(NodeInteractionRouter, InstanceIdFilter);
		eventAggregator.SubscribeNodeLoaded(OnLoaded, InstanceIdFilter);
	}

	private bool initialCreateAdd = false;
	protected virtual void OnAdded(NodeAddedEventArgs eventArgs)
	{
		if (initialCreateAdd)
			eventAggregator.PublishActivateMainView(InstanceId);
	}

	protected virtual void OnCreated(NodeCreatedEventArgs eventArgs)
	{
		OnInstantiated(eventArgs);
		initialCreateAdd = true;
	}

	protected virtual void NodeInteractionRouter(NodeInteractionEventArgs eventArgs)
	{
		switch (eventArgs.Interaction)
		{
			case NodeInteractionType.DoubleClick:
				OnDoubleClick();
				break;
		}
	}

	protected virtual void OnLoaded(NodeLoadedEventArgs eventArgs) => OnInstantiated(eventArgs);

	protected virtual void RequestDisplayViews()
	{
		eventAggregator.PublishActivateMainView(InstanceId);
	}

	protected virtual void OnInstantiated(INodeInstantiatedEventArgs eventArgs)
	{
		if (baseServices.NodeSaveInterceptorProvider.Resolve(InstanceId) is { } saveInterceptor)
		{
			saveInterceptor.SaveInterceptor = OnInteractiveSave;
			saveInterceptor.SavePreviewInterceptor = OnPreviewSave;
		}
		DataState = baseServices.DataStateProvider.Resolve(InstanceId);
		if (dataFilterInterceptorProvider.Resolve(InstanceId) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegate;
			dataFilterInterceptor.IsInverted = IsInverted;
		}
	}

	protected virtual string? FilterCalculationMessage => null;

	protected virtual bool IsInverted => false;

#pragma warning disable CS1998  // Allow async base class to provide code example of how this method could be overridden
	protected virtual async IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, [EnumeratorCancellation] CancellationToken token)
#pragma warning restore CS1998
	{																			 
		yield break;
	}

	protected virtual byte[]? OnInteractiveSave() => OnSave();

	protected virtual byte[]? OnPreviewSave() => OnSave();

	protected virtual byte[]? OnSave() => null;

	protected virtual void OnDoubleClick() => RequestDisplayViews();

	protected virtual bool InstanceIdFilter(INodeTargetEvent targetEventArgs) => targetEventArgs.NodeId == InstanceId;
}
