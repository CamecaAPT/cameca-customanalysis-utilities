using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisNodeBaseServices : ICoreNodeServices
{
	INodePropertiesProvider NodePropertiesProvider { get; }
}

internal class AnalysisNodeBaseServices : IAnalysisNodeBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;

	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider => _coreNodeServices.MenuFactoryProvider;
	public IIonDataProvider IonDataProvider => _coreNodeServices.IonDataProvider;
	public INodeDataStateProvider DataStateProvider => _coreNodeServices.DataStateProvider;
	public INodePersistenceProvider NodePersistenceProvider => _coreNodeServices.NodePersistenceProvider;
	public ICanSaveStateProvider CanSaveStateProvider => _coreNodeServices.CanSaveStateProvider;
	public INodePropertiesProvider NodePropertiesProvider { get; }

	public AnalysisNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodePropertiesProvider nodePropertiesProvider)
	{
		_coreNodeServices = coreNodeServices;
		NodePropertiesProvider = nodePropertiesProvider;
	}
}
