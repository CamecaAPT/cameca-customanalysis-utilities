using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisFilterNodeBaseServices : IAnalysisNodeBaseServices
{
	INodeDataFilterProvider DataFilterProvider { get; }
}

internal class AnalysisFilterNodeBaseServices : IAnalysisFilterNodeBaseServices
{
	private readonly IAnalysisNodeBaseServices _analysisNodeBaseServices;

	public IEventAggregator EventAggregator => _analysisNodeBaseServices.EventAggregator;
	public IIdProvider IdProvider => _analysisNodeBaseServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _analysisNodeBaseServices.MenuFactoryProvider;
	public IIonDataProvider IonDataProvider => _analysisNodeBaseServices.IonDataProvider;
	public INodeDataStateProvider DataStateProvider => _analysisNodeBaseServices.DataStateProvider;
	public INodePersistenceProvider NodePersistenceProvider => _analysisNodeBaseServices.NodePersistenceProvider;
	public ICanSaveStateProvider CanSaveStateProvider => _analysisNodeBaseServices.CanSaveStateProvider;
	public INodePropertiesProvider NodePropertiesProvider => _analysisNodeBaseServices.NodePropertiesProvider;
	public INodeDataFilterProvider DataFilterProvider { get; }

	public AnalysisFilterNodeBaseServices(
		IAnalysisNodeBaseServices analysisNodeBaseServices,
		INodeDataFilterProvider dataFilterProvider)
	{
		_analysisNodeBaseServices = analysisNodeBaseServices;
		DataFilterProvider = dataFilterProvider;
	}
}
