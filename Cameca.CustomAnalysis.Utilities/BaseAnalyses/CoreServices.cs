using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface ICoreServices
{
	IEventAggregator EventAggregator { get; }
	IIdProvider IdProvider { get; }
	IInstanceProvider InstanceProvider { get; }
}

internal class CoreServices : ICoreServices
{
	public IEventAggregator EventAggregator { get; }
	public IIdProvider IdProvider { get; }
	public IInstanceProvider InstanceProvider { get; }

	public CoreServices(
		IEventAggregator eventAggregator,
		IIdProvider idProvider,
		IInstanceProvider instanceProvider)
	{
		EventAggregator = eventAggregator;
		IdProvider = idProvider;
		InstanceProvider = instanceProvider;
	}
}
