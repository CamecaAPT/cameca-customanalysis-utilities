using Prism.Ioc;

namespace Cameca.CustomAnalysis.Utilities;

public static class PrismIocExtensions
{
	public static void RegisterCoreServices(this IContainerRegistry containerRegistry)
	{
		containerRegistry.Register<ICoreNodeServices, CoreNodeServices>();
		containerRegistry.Register<IAnalysisNodeBaseServices, AnalysisNodeBaseServices>();
		containerRegistry.Register<IDataFilterNodeBaseServices, DataFilterNodeBaseServices>();
		containerRegistry.Register<IAnalysisViewModelBaseServices, AnalysisViewModelBaseServices>();
	}
}
