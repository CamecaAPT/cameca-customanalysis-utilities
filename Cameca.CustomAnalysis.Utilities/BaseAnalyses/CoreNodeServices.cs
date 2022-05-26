using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface ICoreNodeServices : ICoreServices
{
	INodeMenuFactoryProvider MenuFactoryProvider { get; }
	IIonDataProvider IonDataProvider { get; }
}

internal class CoreNodeServices : ICoreNodeServices
{
	private readonly ICoreServices _coreServices;
	public IEventAggregator EventAggregator => _coreServices.EventAggregator;
	public IIdProvider IdProvider => _coreServices.IdProvider;
	public INodeMenuFactoryProvider MenuFactoryProvider { get; }
	public IIonDataProvider IonDataProvider { get; }

	public CoreNodeServices(
		ICoreServices coreServices,
		INodeMenuFactoryProvider menuFactoryProvider,
		IIonDataProvider ionDataProvider)
	{
		_coreServices = coreServices;
		MenuFactoryProvider = menuFactoryProvider;
		IonDataProvider = ionDataProvider;
	}
}
