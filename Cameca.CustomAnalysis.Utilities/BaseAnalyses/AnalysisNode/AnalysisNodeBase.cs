using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisNodeBase<TProperties> : AnalysisNodeBase<TProperties, IAnalysisNodeBaseServices>
	where TProperties : INotifyPropertyChanged, new()
{
	protected AnalysisNodeBase(IAnalysisNodeBaseServices services, ResourceFactory resourceFactory) : base(services, resourceFactory) { }
}

public abstract class AnalysisNodeBase<TProperties, TServices> : CoreNodeBase<TServices>
	where TServices : IAnalysisNodeBaseServices
	where TProperties : INotifyPropertyChanged, new()
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

	protected AnalysisNodeBase(TServices services, ResourceFactory resourceFactory) : base(services, resourceFactory)
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

	protected override byte[]? GetSaveContent()
	{
		var serializer = new XmlSerializer(typeof(TProperties));
		using var stringWriter = new StringWriter();
		serializer.Serialize(stringWriter, Properties);
		return Encoding.UTF8.GetBytes(stringWriter.ToString());
	}

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.NodePropertiesProvider.Resolve(Id) is { } nodeProperties)
		{
			_nodeProperties = nodeProperties;
		}

		// If loading existing and data is present, populate properties from serialized data
		if (eventArgs.Trigger == EventTrigger.Load && eventArgs.Data is { } data)
		{
			var xmlData = Encoding.UTF8.GetString(data);
			var serializer = new XmlSerializer(typeof(TProperties));
			using var stringReader = new StringReader(xmlData);
			if (serializer.Deserialize(stringReader) is TProperties loadedOptions)
			{
				Properties = loadedOptions;
			}
		}

		// Wire up change notification events
		Properties = _nodeProperties?.Properties is TProperties typedProperties ? typedProperties : new();
		Properties.PropertyChanged += PropertiesObjectOnPropertyChanged;
	}

	private void PropertiesObjectOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		OnPropertiesChanged(e);
	}

	protected virtual void OnPropertiesChanged(PropertyChangedEventArgs e)
	{
		CanSave = true;
	}

	//protected virtual void PropertiesObjectOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	//{
	//	Services.DataFilterProvider.Resolve(base.Id)?.FilterDataChanged();
	//	CanSave = true;
	//}

	//protected virtual 

	protected void RequestDisplayViews()
	{
		foreach (var (identifier, type) in DisplayViews)
		{
			Services.EventAggregator.PublishCreateViewModel(identifier, Id, ViewModelMode.Interactive, matchExisting: TypedPublishDisplayViewPredicate(type));
		}
	}

	public TProperties Properties
	{
		get => (TProperties)(_nodeProperties?.Properties ?? throw new InvalidOperationException($"Cannot call {nameof(Properties)} until after the node is fully created"));
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

	protected bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> viewModel.GetType() == targetType;

	internal override void OnDoubleClickCore()
	{
		base.OnDoubleClickCore();
		RequestDisplayViews();
	}
}
#nullable disable
