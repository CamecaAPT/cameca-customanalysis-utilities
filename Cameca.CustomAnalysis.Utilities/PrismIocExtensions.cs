using System;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Ioc;

namespace Cameca.CustomAnalysis.Utilities;

public static class PrismIocExtensions
{
	public static void AddCustomAnalysisUtilities(this IContainerRegistry containerRegistry, Action<UtilitiesOptions>? options = null)
	{
		var optionsInstance = new UtilitiesOptions();
		options?.Invoke(optionsInstance);
		if (optionsInstance.UseBaseClasses || optionsInstance.UseStandardBaseClasses)
		{
			containerRegistry.Register<ICoreServices, CoreServices>();
			containerRegistry.Register<ICoreNodeServices, CoreNodeServices>();
			containerRegistry.Register<IAnalysisNodeBaseServices, AnalysisNodeBaseServices>();
			containerRegistry.Register<IDataFilterNodeBaseServices, DataFilterNodeBaseServices>();
			containerRegistry.Register<IAnalysisViewModelBaseServices, AnalysisViewModelBaseServices>();
		}
		if (optionsInstance.UseStandardBaseClasses)
		{
			containerRegistry.Register<IStandardAnalysisNodeBaseServices, StandardAnalysisNodeBaseServices>();
			containerRegistry.Register<IStandardDataFilterNodeBaseServices, StandardDataFilterNodeBaseServices>();
		}
		if (optionsInstance.UseDialogs || optionsInstance.UseStandardBaseClasses)
		{
			containerRegistry.RegisterDialog<EditNameDialogView, EditNameDialogViewModel>(EditNameDialog.EditNameDialogName);
		}
	}
}
