using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface ICoreNodeServices : ICoreServices
{
	INodeMenuFactoryProvider MenuFactoryProvider { get; }
}

internal class CoreNodeServices : ICoreNodeServices
{
	private readonly ICoreServices _coreServices;
	public IEventAggregator EventAggregator => _coreServices.EventAggregator;
	public IIdProvider IdProvider => _coreServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider { get; }

	public CoreNodeServices(
		ICoreServices coreServices,
		INodeMenuFactoryProvider menuFactoryProvider)
	{
		_coreServices = coreServices;
		MenuFactoryProvider = menuFactoryProvider;
	}
}
