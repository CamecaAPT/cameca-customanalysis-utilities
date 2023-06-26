using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class BasicCustomAnalysisNodeBase<TAnalysis, TProperties> : StandardAnalysisFilterNodeBase
	where TAnalysis : IAnalysis<TProperties>
	where TProperties : INotifyPropertyChanged, new()
{
	private readonly Func<IResources> _resourceFactory;

	protected IAnalysis<TProperties> Analysis { get; }


	protected BasicCustomAnalysisNodeBase(IStandardAnalysisFilterNodeBaseServices services, TAnalysis analysis, Func<IResources> resourceFactory) : base(services)
	{
		Analysis = analysis;
		_resourceFactory = resourceFactory;
	}

	protected override byte[]? GetSaveContent()
	{
		var serializer = new XmlSerializer(typeof(TProperties));
		using var stringWriter = new StringWriter();
		serializer.Serialize(stringWriter, ResolvedProperties);
		return Encoding.UTF8.GetBytes(stringWriter.ToString());
	}

	protected override void OnCreated(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreated(eventArgs);

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
		Properties = Properties is TProperties typedProperties ? typedProperties : new();
		if (Properties is INotifyPropertyChanged notifyPropertyChanged)
		{
			notifyPropertyChanged.PropertyChanged += PropertiesObjectOnPropertyChanged;
		}
	}
	
	protected override async IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegateAsync(IIonData ownerIonData, IProgress<double>? progress, [EnumeratorCancellation]CancellationToken token)
	{
		var asyncEnumerable = Analysis.FilterAsync(ownerIonData, ResolvedProperties, CreateResources(), progress, token);
		if (asyncEnumerable is not null)
		{
			await foreach (var memory in asyncEnumerable.WithCancellation(token))
			{
				yield return memory;
			}
		}
	}

	public async Task<object?> Update()
	{
		// Get or create ion data
		try
		{
			if (DataState is { } dataState)
			{
				dataState.IsErrorState = false;
			}

			// Try to call async update with progress dialog if available
			if (Services.ProgressDialogProvider.Resolve(InstanceId) is { } progressDialog)
			{
				return await progressDialog.ShowDialog(Analysis.UpdateProgressText, CallAnalysisUpdate);
			}
			else
			{
				return await CallAnalysisUpdate(null, default);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch
		{
			if (DataState is { } dataState)
			{
				dataState.IsErrorState = true;
			}
			throw;
		}
		return null;
	}

	public async Task<object?> CallAnalysisUpdate(IProgress<double>? progress, CancellationToken cancellationToken)
	{
		if (await Services.IonDataProvider.GetOwnerIonData(InstanceId, progress, cancellationToken) is { } ionData)
		{
			return await Analysis.Update(ionData, ResolvedProperties, CreateResources(), progress, cancellationToken);
		}
		return null;
	}

	private void PropertiesObjectOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		Services.DataFilterProvider.Resolve(InstanceId)?.FilterDataChanged();
		CanSave = true;
	}

	private IResources CreateResources()
	{
		var resources = _resourceFactory();
		if (resources is BasicAnalysisResources basicResources)
		{
			basicResources.Id = InstanceId;
		}
		return resources;
	}

	protected TProperties ResolvedProperties => (TProperties)Properties!;
}
