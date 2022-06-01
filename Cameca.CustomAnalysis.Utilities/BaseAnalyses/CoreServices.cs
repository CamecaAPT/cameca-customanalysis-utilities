using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface ICoreServices
{
	IEventAggregator EventAggregator { get; }
	IIdProvider IdProvider { get; }
}

internal class CoreServices : ICoreServices
{
	public IEventAggregator EventAggregator { get; }
	public IIdProvider IdProvider { get; }

	public CoreServices(
		IEventAggregator eventAggregator,
		IIdProvider idProvider)
	{
		EventAggregator = eventAggregator;
		IdProvider = idProvider;
	}
}
