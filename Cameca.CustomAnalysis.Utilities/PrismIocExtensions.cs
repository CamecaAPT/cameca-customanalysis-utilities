using System;
using Cameca.CustomAnalysis.Utilities;
using Cameca.CustomAnalysis.Utilities.Controls;
using Cameca.CustomAnalysis.Utilities.Legacy;
using Prism.Ioc;

namespace Cameca.CustomAnalysis.Utilities;

public static class PrismIocExtensions
{
	public static void AddCustomAnalysisUtilities(this IContainerRegistry containerRegistry, Action<UtilitiesOptions>? options = null)
	{
		// Create options and apply delegated configuration
		var optionsInstance = new UtilitiesOptions();
		options?.Invoke(optionsInstance);

		// Select features from configured options
		bool registerCoreBaseClasses = false;
		bool registerStandardBaseClasses = false;
		bool registerDialogs = false;
		bool registerLegacy = false;
		bool registerPersistedIonDataSections = false;

		if (optionsInstance.UseBaseClasses)
		{
			registerCoreBaseClasses = true;
		}
		if (optionsInstance.UseStandardBaseClasses)
		{
			registerCoreBaseClasses = true;
			registerStandardBaseClasses = true;
			registerDialogs = true;
		}
		if (optionsInstance.UseDialogs)
		{
			registerDialogs = true;
		}
		if (optionsInstance.UsePersistedIonDataSection)
		{
			registerPersistedIonDataSections = true;
		}
#pragma warning disable CS0618
		// Disable obsolete warning for implementation of obsolete features
		if (optionsInstance.UseLegacy)
#pragma warning restore CS0618
		{
			registerCoreBaseClasses = true;
			registerStandardBaseClasses = true;
			registerDialogs = true;
			registerLegacy = true;
		}

		// Apply registrations of configured options
		if (registerCoreBaseClasses)
		{
			containerRegistry.Register<ICoreServices, CoreServices>();
			containerRegistry.Register<ICoreNodeServices, CoreNodeServices>();
			containerRegistry.Register<IAnalysisNodeBaseServices, AnalysisNodeBaseServices>();
			containerRegistry.Register<IDataFilterNodeBaseServices, DataFilterNodeBaseServices>();
			containerRegistry.Register<IAnalysisViewModelBaseServices, AnalysisViewModelBaseServices>();
			containerRegistry.Register<IAnalysisFilterNodeBaseServices, AnalysisFilterNodeBaseServices>();
		}
		if (registerStandardBaseClasses)
		{
			containerRegistry.Register<IStandardAnalysisNodeBaseServices, StandardAnalysisNodeBaseServices>();
			containerRegistry.Register<IStandardDataFilterNodeBaseServices, StandardDataFilterNodeBaseServices>();
			containerRegistry.Register<IStandardAnalysisFilterNodeBaseServices, StandardAnalysisFilterNodeBaseServices>();
		}
		if (registerDialogs)
		{
			containerRegistry.RegisterDialog<EditNameDialogView, EditNameDialogViewModel>(EditNameDialog.EditNameDialogName);
		}
		if (registerLegacy)
		{
			containerRegistry.Register<IViewBuilder, ViewBuilder>();
		}
		if (registerPersistedIonDataSections)
		{
			containerRegistry.RegisterSingleton<IIonDataSectionPersistenceManager, IonDataSectionPersistencePersistenceManager>();
			containerRegistry.Register<IIonDataSectionWriter, IndividualFilesIonDataSectionWriter>();
		}
	}

	[Obsolete("Use PrismIocExtensions.AddCustomAnalysisUtilities")]
	public static void RegisterCoreServices(this IContainerRegistry containerRegistry)
	{
		containerRegistry.AddCustomAnalysisUtilities();
	}

	public static void RegisterBasicAnalysis(this IContainerRegistry containerRegistry)
	{
		containerRegistry.Register<NodeResource>();
		containerRegistry.Register<AnalysisSetNodeResources>();
		containerRegistry.Register<IResources, BasicAnalysisResources>();
		containerRegistry.Register<ResourceFactory>();
	}
}
