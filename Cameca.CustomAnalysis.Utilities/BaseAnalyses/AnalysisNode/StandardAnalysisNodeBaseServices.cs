using Cameca.CustomAnalysis.Interface;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public interface IStandardAnalysisNodeBaseServices : IAnalysisNodeBaseServices
{
	INodeMenuFactoryProvider NodeMenuFactoryProvider { get; }
	INodeInfoProvider NodeInfoProvider { get; }
	IDialogService DialogService { get; }
}


internal class StandardAnalysisNodeBaseServices : IStandardAnalysisNodeBaseServices
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
	public INodeMenuFactoryProvider NodeMenuFactoryProvider { get; }
	public INodeInfoProvider NodeInfoProvider { get; }
	public IDialogService DialogService { get; }

	public StandardAnalysisNodeBaseServices(
		IAnalysisNodeBaseServices analysisNodeBaseServices,
		INodeMenuFactoryProvider nodeMenuFactoryProvider,
		INodeInfoProvider nodeInfoProvider,
		IDialogService dialogService)
	{
		_analysisNodeBaseServices = analysisNodeBaseServices;
		NodeMenuFactoryProvider = nodeMenuFactoryProvider;
		NodeInfoProvider = nodeInfoProvider;
		DialogService = dialogService;
	}
}
