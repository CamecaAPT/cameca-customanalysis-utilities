using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Legacy;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public class BasicAnalysisResources : NodeResource, IResources
{
	private readonly IMainChartProvider _mainChartProvider;
	private readonly INodeVisibilityControlProvider _nodeVisibilityControlProvider;
	private readonly IAnalysisSetInfoProvider _analysisSetInfoProvider;
	private readonly INodeDataStateProvider _nodeDataStateProvider;
	private readonly ICanSaveStateProvider _canSaveStateProvider;
	private readonly INodeMenuFactoryProvider _nodeMenuFactoryProvider;
	private readonly IIonDisplayInfoProvider _ionDisplayInfoProvider;
	private readonly IReconstructionSectionsProvider _reconstructionSectionsProvider;
	private readonly IProgressDialogProvider _progressDialogProvider;
	private readonly IExperimentInfoProvider _experimentInfoProvider;


	public string AnalysisSetTitle => _analysisSetInfoProvider.Resolve(Id).ThrowIfUnresolved().Title;

	public string? ExperimentFileName => _experimentInfoProvider.Resolve(Id)?.ExperimentFileName;

	/// <inheritdoc />
	public async Task<bool> EnsureRequiredSectionsAvailable(IIonData ionData, IEnumerable<string> requiredSections, IProgress<double>? progress = null,
		CancellationToken cancellationToken = default)
	{
		return await ionData.EnsureSectionsAvailable(
			requiredSections,
			_reconstructionSectionsProvider.Resolve(Id).ThrowIfUnresolved(),
			progress,
			cancellationToken);
	}

	public INodeResource? SelectedNode
	{
		get
		{
			var selectedNodeId = _analysisSetInfoProvider.Resolve(Id).ThrowIfUnresolved().SelectedNodeId;
			return AnalysisSetNodeResources.GetOrCreate(selectedNodeId);
		}
	}
	public IViewBuilder ViewBuilder { get; }
	public INodeResource TopLevelNode
	{
		get
		{
			var rootNodeId = _nodeInfoProvider.GetRootNodeContainer(Id).NodeId;
			return AnalysisSetNodeResources.GetOrCreate(rootNodeId);
		}
	}
	public IEventAggregator Events { get; }
	public IChart3D MainChart => _mainChartProvider.Resolve(Id).ThrowIfUnresolved();
	public IRenderDataFactory ChartObjects { get; }
	public IDialogService Dialog { get; }
	public IExperimentInfo? ExperimentInfo => _experimentInfoProvider.Resolve(Id)?.ExperimentInfo;
	public void RefreshExperimentInfo() => _experimentInfoProvider.Resolve(Id)?.Refresh();
	public IColorMapFactory ColorMap { get; }
	public INodeVisibilityControl Visibility => _nodeVisibilityControlProvider.Resolve(Id).ThrowIfUnresolved();
	public INodeMenuFactory MenuFactory => _nodeMenuFactoryProvider.Resolve(Id).ThrowIfUnresolved();
	public INodeDataState DataState => _nodeDataStateProvider.Resolve(Id).ThrowIfUnresolved();
	public ICanSaveState CanSaveState => _canSaveStateProvider.Resolve(Id).ThrowIfUnresolved();
	public IProgressDialog Progress => _progressDialogProvider.Resolve(Id).ThrowIfUnresolved();

	public Color GetIonColor(IIonTypeInfo ionTypeInfo) =>
		_ionDisplayInfoProvider.Resolve(Id).ThrowIfUnresolved().GetColor(ionTypeInfo);

	public IEnumerable<INodeResource> AnalysisTreeNodes()
	{
		var rootId = TopLevelNode.Id;
		foreach (var id in _nodeInfoProvider.IterateNodeContainers(rootId).Select(x => x.NodeId))
		{
			yield return AnalysisSetNodeResources.GetOrCreate(id);
		}
	}

	public BasicAnalysisResources(
		IDialogService dialogService,
		IEventAggregator eventAggregator,
		IMainChartProvider mainChartProvider,
		INodeVisibilityControlProvider nodeVisibilityControlProvider,
		IAnalysisSetInfoProvider analysisSetInfoProvider,
		AnalysisSetNodeResources analysisSetNodeResources,
		INodeInfoProvider nodeInfoProvider,
		IExportToCsvProvider exportToCsvProvider,
		IIonDataProvider ionDataProvider,
		IMassSpectrumRangeManagerProvider massSpectrumRangeManagerProvider,
		IReconstructionSectionsProvider reconstructionSectionsProvider,
		INodeDataStateProvider nodeDataStateProvider,
		ICanSaveStateProvider canSaveStateProvider,
		INodeMenuFactoryProvider nodeMenuFactoryProvider,
		IIonDisplayInfoProvider ionDisplayInfoProvider,
		IColorMapFactory colorMapFactory,
		IRenderDataFactory renderDataFactory,
		IViewBuilder viewBuilder,
		IProgressDialogProvider progressDialogProvider,
		IExperimentInfoProvider experimentInfoProvider)
		: base(nodeInfoProvider, exportToCsvProvider, ionDataProvider, massSpectrumRangeManagerProvider)
	{
		_mainChartProvider = mainChartProvider;
		_nodeVisibilityControlProvider = nodeVisibilityControlProvider;
		_analysisSetInfoProvider = analysisSetInfoProvider;
		AnalysisSetNodeResourcesBacking.Value = analysisSetNodeResources;
		_nodeDataStateProvider = nodeDataStateProvider;
		_canSaveStateProvider = canSaveStateProvider;
		_nodeMenuFactoryProvider = nodeMenuFactoryProvider;
		_ionDisplayInfoProvider = ionDisplayInfoProvider;
		_reconstructionSectionsProvider = reconstructionSectionsProvider;
		_progressDialogProvider = progressDialogProvider;
		_experimentInfoProvider = experimentInfoProvider;
		Dialog = dialogService;
		ColorMap = colorMapFactory;
		ChartObjects = renderDataFactory;
		ViewBuilder = viewBuilder;
		Events = eventAggregator;
	}
}
