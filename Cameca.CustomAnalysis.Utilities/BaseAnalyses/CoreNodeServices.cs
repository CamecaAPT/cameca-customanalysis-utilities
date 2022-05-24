using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface ICoreNodeServices
{
	IEventAggregator EventAggregator { get; }
	IIdProvider IdProvider { get; }
}

internal class CoreNodeServices : ICoreNodeServices
{
	public IEventAggregator EventAggregator { get; }

	public IIdProvider IdProvider { get; }

	public CoreNodeServices(
		IEventAggregator eventAggregator,
		IIdProvider idProvider)
	{
		EventAggregator = eventAggregator;
		IdProvider = idProvider;
	}
}
