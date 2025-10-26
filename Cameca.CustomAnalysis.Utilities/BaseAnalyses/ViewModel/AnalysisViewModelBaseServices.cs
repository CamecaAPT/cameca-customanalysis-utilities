using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisViewModelBaseServices : ICoreServices
{
}

internal class AnalysisViewModelBaseServices : IAnalysisViewModelBaseServices
{
	private readonly ICoreServices _coreServices;
	public IEventAggregator EventAggregator => _coreServices.EventAggregator;
	public IIdProvider IdProvider => _coreServices.IdProvider;
	public IInstanceProvider InstanceProvider => _coreServices.InstanceProvider;

	public AnalysisViewModelBaseServices(ICoreServices coreServices)
	{
		_coreServices = coreServices;
	}
}
