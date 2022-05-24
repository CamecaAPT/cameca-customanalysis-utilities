using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisViewModelBaseServices : ICoreNodeServices
{
	IInstanceProvider InstanceProvider { get; }
}

internal class AnalysisViewModelBaseServices : IAnalysisViewModelBaseServices
{
	private readonly ICoreNodeServices _coreNodeServices;
	public IEventAggregator EventAggregator => _coreNodeServices.EventAggregator;
	public IIdProvider IdProvider => _coreNodeServices.IdProvider;
	public IInstanceProvider InstanceProvider { get; }

	public AnalysisViewModelBaseServices(
		ICoreNodeServices coreNodeServices,
		IInstanceProvider instanceProvider)
	{
		_coreNodeServices = coreNodeServices;
		InstanceProvider = instanceProvider;
	}
}
