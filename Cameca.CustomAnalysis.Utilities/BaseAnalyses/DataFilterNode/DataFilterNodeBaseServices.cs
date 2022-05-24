using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IDataFilterNodeBaseServices : ICoreNodeServices
{
	INodeDataFilterInterceptorProvider DataFilterInterceptorProvider { get; }
	INodeDataStateProvider DataStateProvider { get; }
	INodeSaveInterceptorProvider NodeSaveInterceptorProvider { get; }
}

internal class DataFilterNodeBaseServices : IDataFilterNodeBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;
	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public INodeDataStateProvider DataStateProvider { get; }
	public INodeSaveInterceptorProvider NodeSaveInterceptorProvider { get; }
	public INodeDataFilterInterceptorProvider DataFilterInterceptorProvider { get; }

	public DataFilterNodeBaseServices(
		ICoreNodeServices coreNodeServices,
		INodeDataStateProvider dataStateProvider,
		INodeSaveInterceptorProvider nodeSaveInterceptorProvider,
		INodeDataFilterInterceptorProvider dataFilterInterceptorProvider)
	{
		_coreNodeServices = coreNodeServices;
		DataStateProvider = dataStateProvider;
		NodeSaveInterceptorProvider = nodeSaveInterceptorProvider;
		DataFilterInterceptorProvider = dataFilterInterceptorProvider;
	}
}
