using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisNodeBaseServices : ICoreNodeServices
{
	INodeDataStateProvider DataStateProvider { get; }
	INodePersistenceProvider NodePersistenceProvider { get; }
	INodeProperties NodeProperties { get; }
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
	public INodeProperties NodeProperties { get; }

	public AnalysisNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodeDataStateProvider dataStateProvider,
		INodePersistenceProvider nodePersistenceProvider,
		INodeProperties nodeProperties)
	{
		_coreNodeServices = coreNodeServices;
		DataStateProvider = dataStateProvider;
		NodePersistenceProvider = nodePersistenceProvider;
		NodeProperties = nodeProperties;
	}
}
