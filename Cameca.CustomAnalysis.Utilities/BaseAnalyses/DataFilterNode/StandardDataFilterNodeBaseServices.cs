using Cameca.CustomAnalysis.Interface;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public interface IStandardDataFilterNodeBaseServices : IDataFilterNodeBaseServices
{
	INodeMenuFactoryProvider NodeMenuFactoryProvider { get; }
	INodeInfoProvider NodeInfoProvider { get; }
	IDialogService DialogService { get; }
}

internal class StandardDataFilterNodeBaseServices : IStandardDataFilterNodeBaseServices
{
	private readonly IDataFilterNodeBaseServices _coreNodeServices;

	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _coreNodeServices.MenuFactoryProvider;
	public IIonDataProvider IonDataProvider => _coreNodeServices.IonDataProvider;
	public INodeDataStateProvider DataStateProvider => _coreNodeServices.DataStateProvider;
	public INodePersistenceProvider NodePersistenceProvider => _coreNodeServices.NodePersistenceProvider;
	public ICanSaveStateProvider CanSaveStateProvider => _coreNodeServices.CanSaveStateProvider;
	public INodeDataFilterProvider DataFilterProvider => _coreNodeServices.DataFilterProvider;
	public INodeMenuFactoryProvider NodeMenuFactoryProvider { get; }
	public INodeInfoProvider NodeInfoProvider { get; }
	public IDialogService DialogService { get; }

	public StandardDataFilterNodeBaseServices(
		IDataFilterNodeBaseServices coreNodeServices,
		INodeMenuFactoryProvider nodeMenuFactoryProvider,
		INodeInfoProvider nodeInfoProvider,
		IDialogService dialogService)
	{
		_coreNodeServices = coreNodeServices;
		NodeMenuFactoryProvider = nodeMenuFactoryProvider;
		NodeInfoProvider = nodeInfoProvider;
		DialogService = dialogService;
	}
}
