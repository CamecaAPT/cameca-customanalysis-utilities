using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface ICoreNodeServices : ICoreServices
{
	INodeMenuFactoryProvider MenuFactoryProvider { get; }
	IIonDataProvider IonDataProvider { get; }
	INodeDataStateProvider DataStateProvider { get; }
	INodePersistenceProvider NodePersistenceProvider { get; }
	ICanSaveStateProvider CanSaveStateProvider { get; }
}

internal class CoreNodeServices : ICoreNodeServices
{
	private readonly ICoreServices _coreServices;
	public IEventAggregator EventAggregator => _coreServices.EventAggregator;
	public IIdProvider IdProvider => _coreServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider { get; }
	public IIonDataProvider IonDataProvider { get; }
	public INodeDataStateProvider DataStateProvider { get; }
	public INodePersistenceProvider NodePersistenceProvider { get; }
	public ICanSaveStateProvider CanSaveStateProvider { get; }

	public CoreNodeServices(
		ICoreServices coreServices,
		INodeMenuFactoryProvider menuFactoryProvider,
		IIonDataProvider ionDataProvider,
		INodeDataStateProvider dataStateProvider,
		INodePersistenceProvider nodePersistenceProvider,
		ICanSaveStateProvider canSaveStateProvider)
	{
		_coreServices = coreServices;
		MenuFactoryProvider = menuFactoryProvider;
		IonDataProvider = ionDataProvider;
		DataStateProvider = dataStateProvider;
		NodePersistenceProvider = nodePersistenceProvider;
		CanSaveStateProvider = canSaveStateProvider;
	}
}
