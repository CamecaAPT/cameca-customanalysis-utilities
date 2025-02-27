using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Legacy;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public class ResourceFactory
{
	private readonly Func<IResources> _resourceFactory;

	public ResourceFactory(Func<IResources> resourceFactory)
	{
		this._resourceFactory = resourceFactory;
	}

	public IResources CreateResource(Guid nodeId)
	{
		var resources = _resourceFactory();
		if (resources is BasicAnalysisResources basicResources)
		{
			basicResources.Id = nodeId;
		}
		return resources;
	}
}

public interface IResources : INodeResource
{
	string AnalysisSetTitle { get; }
	INodeResource? SelectedNode { get; }
	IViewBuilder ViewBuilder { get; }
	IEventAggregator Events { get; }
	IChart3D MainChart { get; }
	IRenderDataFactory ChartObjects { get; }
	IColorMapFactory ColorMap { get; }
	INodeVisibilityControl Visibility { get; }
	INodeMenuFactory MenuFactory { get; }
	INodeDataState DataState { get; }
	ICanSaveState CanSaveState { get; }
	IIonDisplayInfo IonDisplayInfo { get; }
	IDialogService Dialog { get; }
	IExperimentInfo? ExperimentInfo { get; }
	void RefreshExperimentInfo();
	string? ExperimentFileName { get; }
	IElementDataSetService ElementDataService { get; }
	IIonFormulaIsotopeCalculator FormulaIsotopeCalculator { get; }


	/// <inheritdoc cref="IonDataExtensions.EnsureSectionsAvailable"/>
	Task<bool> EnsureRequiredSectionsAvailable(IIonData ionData, IEnumerable<string> requiredSections,
		IProgress<double>? progress = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// IMPORTANT: Do not call from <see cref="IAnalysis{TProperties}.Update"/> method. Supports progress bars for independent actions.
	/// </summary>
	/// <remarks>
	/// Currently this should only be reference and used in actions that are triggered independently of the extension control flow.
	/// An example of this is a custom button added to a view that should have a progress bar. Any attempt to call from methods that
	/// are triggered by AP Suite directly already use this to display a progress bar and will almost certainly deadlock in all cases.
	/// Any long-running tasks call from the Update method will already have a progress bar displayed in AP Suite
	/// </remarks>
	IProgressDialog Progress { get; }
	//IViewModelResource? SelectedViewModel { get; }
}
