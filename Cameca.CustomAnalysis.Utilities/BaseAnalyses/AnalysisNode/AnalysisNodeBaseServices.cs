using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisNodeBaseServices : ICoreNodeServices
{
	INodeDataStateProvider DataStateProvider { get; }
	INodeSaveInterceptorProvider NodeSaveInterceptorProvider { get; }
}

internal class AnalysisNodeBaseServices : IAnalysisNodeBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;

	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeDataStateProvider DataStateProvider { get; }
	public INodeSaveInterceptorProvider NodeSaveInterceptorProvider { get; }

	public AnalysisNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodeDataStateProvider dataStateProvider,
		INodeSaveInterceptorProvider nodeSaveInterceptorProvider)
	{
		_coreNodeServices = coreNodeServices;
		DataStateProvider = dataStateProvider;
		NodeSaveInterceptorProvider = nodeSaveInterceptorProvider;
	}
}
