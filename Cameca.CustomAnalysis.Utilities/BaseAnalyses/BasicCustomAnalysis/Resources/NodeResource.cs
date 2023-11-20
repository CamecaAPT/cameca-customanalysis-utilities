using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public class NodeResource : INodeResource
{
	protected readonly INodeInfoProvider _nodeInfoProvider;
	private readonly IExportToCsvProvider _exportToCsvProvider;
	private readonly IIonDataProvider _ionDataProvider;
	private readonly IMassSpectrumRangeManagerProvider _massSpectrumRangeManagerProvider;
	internal readonly RequiredSetOnce<AnalysisSetNodeResources> AnalysisSetNodeResourcesBacking = new();
	internal AnalysisSetNodeResources AnalysisSetNodeResources => AnalysisSetNodeResourcesBacking.Value;

	public Guid Id { get; internal set; }
	public string Name => _nodeInfoProvider.Resolve(Id).ThrowIfUnresolved().Name;
	public string Title => _nodeInfoProvider.Resolve(Id).ThrowIfUnresolved().Title;
	public string TypeId => _nodeInfoProvider.Resolve(Id).ThrowIfUnresolved().TypeId;
	public ImageSource? Icon => _nodeInfoProvider.Resolve(Id).ThrowIfUnresolved().Icon;

	public IGeometricRegion? Region =>
		(_nodeInfoProvider.Resolve(Id).ThrowIfUnresolved() as IGeometricRoiNodeInfo)?.Region;

	public INodeResource? Parent =>
		_nodeInfoProvider.Resolve(Id).ThrowIfUnresolved().Parent is { } parentId
			? AnalysisSetNodeResources.GetOrCreate(parentId)
			: null;

	public IEnumerable<INodeResource> Children =>
		_nodeInfoProvider.Resolve(Id).ThrowIfUnresolved().Children
			.Select(x => AnalysisSetNodeResources.GetOrCreate(x))
			.ToList();

	public IMassSpectrumRangeManager RangeManager => _massSpectrumRangeManagerProvider.Resolve(Id).ThrowIfUnresolved();

	public IExportToCsv? ExportToCsv => _exportToCsvProvider.Resolve(Id);

	public INodeResource IonDataOwnerNode
	{
		get
		{
			var ownerId = _ionDataProvider.Resolve(Id).ThrowIfUnresolved().OwnerNodeId;
			return AnalysisSetNodeResources.GetOrCreate(ownerId);
		}
	}

	public Task<IIonData?> GetIonData(IProgress<double>? progress = null, CancellationToken cancellationToken = default) =>
		_ionDataProvider.GetOwnerIonData(Id, progress, cancellationToken);

	public IIonData? GetValidIonData() => _ionDataProvider.GetResolvedOwnerIonData(Id);

	public NodeResource(
		INodeInfoProvider nodeInfoProvider,
		IExportToCsvProvider exportToCsvProvider,
		IIonDataProvider ionDataProvider,
		IMassSpectrumRangeManagerProvider massSpectrumRangeManagerProvider)
	{
		_nodeInfoProvider = nodeInfoProvider;
		_exportToCsvProvider = exportToCsvProvider;
		_ionDataProvider = ionDataProvider;
		_massSpectrumRangeManagerProvider = massSpectrumRangeManagerProvider;
	}
}
