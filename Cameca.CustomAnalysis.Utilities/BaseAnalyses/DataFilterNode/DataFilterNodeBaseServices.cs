using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IDataFilterNodeBaseServices : ICoreNodeServices
{
	INodeDataFilterProvider DataFilterProvider { get; }
	INodeDataStateProvider DataStateProvider { get; }
	INodePersistenceProvider NodePersistenceProvider { get; }
}

internal class DataFilterNodeBaseServices : IDataFilterNodeBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;
	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _coreNodeServices.MenuFactoryProvider;
	public INodeDataStateProvider DataStateProvider { get; }
	public INodePersistenceProvider NodePersistenceProvider { get; }
	public INodeDataFilterProvider DataFilterProvider { get; }

	public DataFilterNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodeDataStateProvider dataStateProvider,
		INodePersistenceProvider nodePersistenceProvider,
		INodeDataFilterProvider dataFilterProvider)
	{
		_coreNodeServices = coreNodeServices;
		DataStateProvider = dataStateProvider;
		NodePersistenceProvider = nodePersistenceProvider;
		DataFilterProvider = dataFilterProvider;
	}
}
