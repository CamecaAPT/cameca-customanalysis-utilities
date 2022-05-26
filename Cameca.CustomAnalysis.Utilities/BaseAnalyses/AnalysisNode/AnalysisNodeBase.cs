using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisNodeBase : CoreNodeBase<IAnalysisNodeBaseServices>
{
	protected record View(string Identifier, Type Type);

	private readonly Lazy<View[]> _defaultViews;

	private View[] GetDefaultViews() => GetType()
		.GetCustomAttributes(typeof(DefaultViewAttribute), true)
		.OfType<DefaultViewAttribute>()
		.Select(x => new View(x.ViewModelUniqueId, x.ViewModelType))
		.ToArray();

	protected virtual IEnumerable<View> DisplayViews => _defaultViews.Value;

	protected INodeDataState? DataState;

	protected virtual IEnumerable<IMenuItem> ContextMenuItems => Enumerable.Empty<IMenuItem>();

	protected AnalysisNodeBase(IAnalysisNodeBaseServices services) : base(services)
	{
		// Get attribute data
		_defaultViews = new(GetDefaultViews);
	}

	internal override void OnAfterCreatedCore(NodeAfterCreatedEventArgs eventArgs)
	{
		base.OnAfterCreatedCore(eventArgs);
		RequestDisplayViews();
	}

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

	protected virtual void RequestDisplayViews()
	{
		foreach (var (identifier, type) in DisplayViews)
		{
			Services.EventAggregator.PublishDisplayView(identifier, InstanceId, TypedPublishDisplayViewPredicate(type));
		}
	}

	protected Task<IIonData?> GetIonData(IProgress<double>? progress = null, CancellationToken cancellationToken = default)
		=> Services.IonDataProvider.GetIonData(InstanceId, progress, cancellationToken);

	protected object? Properties
	{
		get => Services.NodeProperties.Properties;
		set => Services.NodeProperties.Properties = value;
	}

	protected virtual byte[]? OnSave() => null;

	protected virtual byte[]? OnPreviewSave() => OnSave();

	private Predicate<object> TypedPublishDisplayViewPredicate(Type targetType)
		=> viewModel => MatchExistingViewPredicate(targetType, viewModel);

	protected virtual bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> viewModel.GetType() == targetType;

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
#nullable disable
