using Cameca.CustomAnalysis.Interface;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public interface IStandardAnalysisFilterNodeBaseServices : IAnalysisFilterNodeBaseServices
{
	INodeMenuFactoryProvider NodeMenuFactoryProvider { get; }
	INodeInfoProvider NodeInfoProvider { get; }
	IDialogService DialogService { get; }
	IProgressDialogProvider ProgressDialogProvider { get; }
}


internal class StandardAnalysisFilterNodeBaseServices : IStandardAnalysisFilterNodeBaseServices
{
	private readonly IAnalysisFilterNodeBaseServices _analysisFilterNodeBaseServices;

	public IEventAggregator EventAggregator => _analysisFilterNodeBaseServices.EventAggregator;
	public IIdProvider IdProvider => _analysisFilterNodeBaseServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _analysisFilterNodeBaseServices.MenuFactoryProvider;
	public IIonDataProvider IonDataProvider => _analysisFilterNodeBaseServices.IonDataProvider;
	public INodeDataStateProvider DataStateProvider => _analysisFilterNodeBaseServices.DataStateProvider;
	public INodePersistenceProvider NodePersistenceProvider => _analysisFilterNodeBaseServices.NodePersistenceProvider;
	public ICanSaveStateProvider CanSaveStateProvider => _analysisFilterNodeBaseServices.CanSaveStateProvider;
	public INodePropertiesProvider NodePropertiesProvider => _analysisFilterNodeBaseServices.NodePropertiesProvider;
	public INodeDataFilterProvider DataFilterProvider => _analysisFilterNodeBaseServices.DataFilterProvider;
	public INodeMenuFactoryProvider NodeMenuFactoryProvider { get; }
	public INodeInfoProvider NodeInfoProvider { get; }
	public IDialogService DialogService { get; }
	public IProgressDialogProvider ProgressDialogProvider { get; }

	public StandardAnalysisFilterNodeBaseServices(
		IAnalysisFilterNodeBaseServices analysisFilterNodeBaseServices,
		INodeMenuFactoryProvider nodeMenuFactoryProvider,
		INodeInfoProvider nodeInfoProvider,
		IDialogService dialogService,
		IProgressDialogProvider progressDialogProvider)
	{
		_analysisFilterNodeBaseServices = analysisFilterNodeBaseServices;
		NodeMenuFactoryProvider = nodeMenuFactoryProvider;
		NodeInfoProvider = nodeInfoProvider;
		DialogService = dialogService;
		ProgressDialogProvider = progressDialogProvider;
	}
}
