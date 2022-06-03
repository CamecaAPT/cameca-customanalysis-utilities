using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Ioc;

namespace Cameca.CustomAnalysis.Utilities;

public static class PrismIocExtensions
{
	public static void RegisterCoreServices(this IContainerRegistry containerRegistry)
	{
		containerRegistry.Register<ICoreServices, CoreServices>();
		containerRegistry.Register<ICoreNodeServices, CoreNodeServices>();
		containerRegistry.Register<IAnalysisNodeBaseServices, AnalysisNodeBaseServices>();
		containerRegistry.Register<IDataFilterNodeBaseServices, DataFilterNodeBaseServices>();
		containerRegistry.Register<IAnalysisViewModelBaseServices, AnalysisViewModelBaseServices>();
	}

	public static void RegisterDialogs(this IContainerRegistry containerRegistry)
	{
		containerRegistry.RegisterDialog<EditNameDialogView, EditNameDialogViewModel>(EditNameDialog.EditNameDialogName);
	}
}
