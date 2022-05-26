using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisNodeBaseServices : ICoreNodeServices
{
	INodeDataStateProvider DataStateProvider { get; }
	INodePersistenceProvider NodePersistenceProvider { get; }
	INodePropertiesProvider NodePropertiesProvider { get; }
}

internal class AnalysisNodeBaseServices : IAnalysisNodeBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;

	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _coreNodeServices.MenuFactoryProvider;
	public IIonDataProvider IonDataProvider => _coreNodeServices.IonDataProvider;
	public INodeDataStateProvider DataStateProvider { get; }
	public INodePersistenceProvider NodePersistenceProvider { get; }
	public INodePropertiesProvider NodePropertiesProvider { get; }

	public AnalysisNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodeDataStateProvider dataStateProvider,
		INodePersistenceProvider nodePersistenceProvider,
		INodePropertiesProvider nodePropertiesProvider)
	{
		_coreNodeServices = coreNodeServices;
		DataStateProvider = dataStateProvider;
		NodePersistenceProvider = nodePersistenceProvider;
		NodePropertiesProvider = nodePropertiesProvider;
	}
}
