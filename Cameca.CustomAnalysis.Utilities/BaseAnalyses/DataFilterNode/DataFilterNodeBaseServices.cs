using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IDataFilterNodeBaseServices : ICoreNodeServices
{
	INodeDataFilterProvider DataFilterProvider { get; }
}

internal class DataFilterNodeBaseServices : IDataFilterNodeBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;
	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _coreNodeServices.MenuFactoryProvider;
	public IIonDataProvider IonDataProvider => _coreNodeServices.IonDataProvider;
	public INodeDataStateProvider DataStateProvider => _coreNodeServices.DataStateProvider;
	public INodePersistenceProvider NodePersistenceProvider => _coreNodeServices.NodePersistenceProvider;
	public ICanSaveStateProvider CanSaveStateProvider => _coreNodeServices.CanSaveStateProvider;
	public INodeDataFilterProvider DataFilterProvider { get; }

	public DataFilterNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodeDataFilterProvider dataFilterProvider)
	{
		_coreNodeServices = coreNodeServices;
		DataFilterProvider = dataFilterProvider;
	}
}
