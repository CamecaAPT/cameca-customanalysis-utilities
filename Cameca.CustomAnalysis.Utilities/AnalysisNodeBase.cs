using System;
using System.Collections.Generic;
using System.Linq;
using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisNodeBaseServices : IAnalysisCoreServices
{
	INodeDataStateProvider DataStateProvider { get; }

	INodeSaveInterceptorProvider NodeSaveInterceptorProvider { get; }
}

internal class AnalysisNodeBaseServices : IAnalysisNodeBaseServices
{
	private readonly IAnalysisCoreServices coreServices;

	public IEventAggregator EventAggregator => coreServices.EventAggregator;

	public IIdProvider IdProvider => coreServices.IdProvider;

	public INodeDataStateProvider DataStateProvider { get; }

	public INodeSaveInterceptorProvider NodeSaveInterceptorProvider { get; }

	public AnalysisNodeBaseServices(
		IAnalysisCoreServices coreServices,
		INodeDataStateProvider dataStateProvider,
		INodeSaveInterceptorProvider nodeSaveInterceptorProvider)
	{
		this.coreServices = coreServices;
		DataStateProvider = dataStateProvider;
		NodeSaveInterceptorProvider = nodeSaveInterceptorProvider;
	}
}

public abstract class AnalysisNodeBase : IDisposable
{
	protected readonly IAnalysisNodeBaseServices baseServices;
	protected readonly IEventAggregator eventAggregator;
	protected readonly IDisposableCollection<SubscriptionToken> managedSubscriptions = new DisposableList<SubscriptionToken>();

	protected record View(string Identifier, Type Type);

	private readonly Lazy<View[]> defaultViews;
	protected INodeDataState? DataState;

	private View[] GetDefaultViews() => GetType()
		.GetCustomAttributes(typeof(DefaultViewAttribute), true)
		.OfType<DefaultViewAttribute>()
		.Select(x => new View(x.ViewModelUniqueId, x.ViewModelType))
		.ToArray();

	protected virtual IEnumerable<View> DisplayViews => defaultViews.Value;

	protected Guid InstanceId => baseServices.IdProvider.Get(this);

	protected AnalysisNodeBase(IAnalysisNodeBaseServices baseServices)
	{
		this.baseServices = baseServices;
		eventAggregator = this.baseServices.EventAggregator;

		// Get attribute data
		defaultViews = new(GetDefaultViews);

		// Register events
		eventAggregator.SubscribeNodeAdded(OnAdded, InstanceIdFilter);
		eventAggregator.SubscribeNodeCreated(OnCreated, InstanceIdFilter);
		eventAggregator.SubscribeNodeInteraction(NodeInteractionRouter, InstanceIdFilter);
		eventAggregator.SubscribeNodeLoaded(OnLoaded, InstanceIdFilter);
	}

	protected virtual void OnAdded(NodeAddedEventArgs eventArgs)
	{
		RequestDisplayViews();
	}

	protected virtual void OnCreated(NodeCreatedEventArgs eventArgs)
	{
		OnInstantiated(eventArgs);
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
		foreach (var view in DisplayViews)
		{
			eventAggregator.PublishDisplayView(view.Identifier, InstanceId, TypedPublishDisplayViewPredicate(view.Type));
		}
	}

	protected virtual bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> viewModel.GetType() == targetType;

	protected virtual void OnInstantiated(INodeInstantiatedEventArgs eventArgs)
	{
		if (baseServices.NodeSaveInterceptorProvider.Resolve(InstanceId) is { } saveInterceptor)
		{
			saveInterceptor.SaveInterceptor = OnInteractiveSave;
			saveInterceptor.SavePreviewInterceptor = OnPreviewSave;
		}
		DataState = baseServices.DataStateProvider.Resolve(InstanceId);
	}

	protected virtual byte[]? OnInteractiveSave() => OnSave();

	protected virtual byte[]? OnPreviewSave() => OnSave();

	protected virtual byte[]? OnSave() => null;

	protected virtual void OnDoubleClick() => RequestDisplayViews();

	protected virtual bool InstanceIdFilter(INodeTargetEvent targetEventArgs) => targetEventArgs.NodeId == InstanceId;

	private Predicate<object> TypedPublishDisplayViewPredicate(Type targetType)
		=> viewModel => MatchExistingViewPredicate(targetType, viewModel);

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
#nullable disable
