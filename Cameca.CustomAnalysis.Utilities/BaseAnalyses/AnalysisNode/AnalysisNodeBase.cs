﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisNodeBase : AnalysisNodeBase<IAnalysisNodeBaseServices>
{
	protected AnalysisNodeBase(IAnalysisNodeBaseServices services) : base(services) { }
}

public abstract class AnalysisNodeBase<TServices> : CoreNodeBase<TServices>
	where TServices : IAnalysisNodeBaseServices
{
	private INodeProperties? _nodeProperties = null;

	protected record View(string Identifier, Type Type);

	private readonly Lazy<View[]> _defaultViews;

	private View[] GetDefaultViews() => GetType()
		.GetCustomAttributes(typeof(DefaultViewAttribute), true)
		.OfType<DefaultViewAttribute>()
		.Select(x => new View(x.ViewModelUniqueId, x.ViewModelType))
		.ToArray();

	protected virtual IEnumerable<View> DisplayViews => _defaultViews.Value;

	protected AnalysisNodeBase(TServices services) : base(services)
	{
		// Get attribute data
		_defaultViews = new(GetDefaultViews);
	}

	internal override void OnAddedCore(NodeAddedEventArgs eventArgs)
	{
		base.OnAddedCore(eventArgs);
		if (eventArgs.Trigger == EventTrigger.Create)
		{
			RequestDisplayViews();
		}
	}

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.NodePropertiesProvider.Resolve(InstanceId) is { } nodeProperties)
		{
			_nodeProperties = nodeProperties;
		}
	}

	protected virtual void RequestDisplayViews()
	{
		foreach (var (identifier, type) in DisplayViews)
		{
			Services.EventAggregator.PublishCreateViewModel(identifier, InstanceId, ViewModelMode.Interactive, matchExisting: TypedPublishDisplayViewPredicate(type));
		}
	}

	protected object? Properties
	{
		get => _nodeProperties?.Properties;
		set
		{
			if (_nodeProperties is null) return;
			if (_nodeProperties.Properties is IDisposable disposable)
			{
				disposable.Dispose();
			}
			_nodeProperties.Properties = value;
		}
	}

	private Predicate<object> TypedPublishDisplayViewPredicate(Type targetType)
		=> viewModel => MatchExistingViewPredicate(targetType, viewModel);

	protected virtual bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> viewModel.GetType() == targetType;

	internal override void OnDoubleClickCore()
	{
		base.OnDoubleClickCore();
		RequestDisplayViews();
	}
}
#nullable disable
