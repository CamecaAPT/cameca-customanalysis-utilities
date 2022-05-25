using Cameca.CustomAnalysis.Interface;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public interface IAnalysisViewModelBaseServices : ICoreServices
{
	IInstanceProvider InstanceProvider { get; }
}

internal class AnalysisViewModelBaseServices : IAnalysisViewModelBaseServices
{
	private readonly ICoreServices _coreServices;
	public IEventAggregator EventAggregator => _coreServices.EventAggregator;
	public IIdProvider IdProvider => _coreServices.IdProvider;
	public IInstanceProvider InstanceProvider { get; }

	public AnalysisViewModelBaseServices(
		ICoreServices coreServices,
		IInstanceProvider instanceProvider)
	{
		_coreServices = coreServices;
		InstanceProvider = instanceProvider;
	}
}
