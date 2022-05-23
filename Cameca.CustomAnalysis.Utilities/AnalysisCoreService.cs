using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisCoreServices
{
	IEventAggregator EventAggregator { get; }
	IIdProvider IdProvider { get; }
}

internal class AnalysisCoreServices : IAnalysisCoreServices
{
	public IEventAggregator EventAggregator { get; }

	public IIdProvider IdProvider { get; }

	public AnalysisCoreServices(
		IEventAggregator eventAggregator,
		IIdProvider idProvider)
	{
		EventAggregator = eventAggregator;
		IdProvider = idProvider;
	}
}
