using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public abstract class LegacyCustomAnalysisNodeBase<TAnalysis, TOptions> : StandardAnalysisNodeBase
	where TAnalysis : ICustomAnalysis<TOptions>
	where TOptions : INotifyPropertyChanged, new()
{
	public TOptions Options { get; private set; } = new();

	protected readonly TAnalysis Analysis;

	protected LegacyCustomAnalysisNodeBase(IStandardAnalysisNodeBaseServices services, TAnalysis analysis) : base(services)
	{
		Analysis = analysis;
	}

	// Calculations should ideally be performed on the Node, and the ViewModels should only request calculation and process results
	public virtual async Task Run(IViewBuilder viewBuilder)
	{
		// Get or create ion data
		if (await Services.IonDataProvider.GetIonData(InstanceId) is not { } ionData) return;
		Analysis.Run(ionData, Options, viewBuilder);
	}

	protected override byte[]? GetSaveContent()
	{
		var serializer = new XmlSerializer(typeof(TOptions));
		using var stringWriter = new StringWriter();
		serializer.Serialize(stringWriter, Options);
		return Encoding.UTF8.GetBytes(stringWriter.ToString());
	}

	protected override void OnCreated(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreated(eventArgs);
		Options.PropertyChanged += OptionsOnPropertyChanged;

		// If loading existing and data is present, populate Options property from serialized data
		if (eventArgs.Trigger == EventTrigger.Load && eventArgs.Data is { } data)
		{
			var xmlData = Encoding.UTF8.GetString(data);
			var serializer = new XmlSerializer(typeof(TOptions));
			using var stringReader = new StringReader(xmlData);
			if (serializer.Deserialize(stringReader) is TOptions loadedOptions)
			{
				Options = loadedOptions;
			}
		}
	}

	private void OptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (CanSaveState is { } canSaveState)
		{
			canSaveState.CanSave = true;
		}
	}
}
