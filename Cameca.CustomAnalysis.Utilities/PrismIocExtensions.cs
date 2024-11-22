﻿using System;
using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities;
using Cameca.CustomAnalysis.Utilities.Controls;
using Cameca.CustomAnalysis.Utilities.ExtensionMethods;
using Cameca.CustomAnalysis.Utilities.Legacy;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;

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
		// Ensure all inner dependencies to create these objects are registered
		containerRegistry.EnsureRegistered<IAnalysisSetInfoProvider>();
		containerRegistry.EnsureRegistered<ICanSaveStateProvider>();
		containerRegistry.EnsureRegistered<IColorMapFactory>();
		containerRegistry.EnsureRegistered<IDialogService>();
		containerRegistry.EnsureRegistered<IEventAggregator>();
		containerRegistry.EnsureRegistered<IExperimentInfoProvider>();
		containerRegistry.EnsureRegistered<IExportToCsvProvider>();
		containerRegistry.EnsureRegistered<IIonDataProvider>();
		containerRegistry.EnsureRegistered<IIonDisplayInfoProvider>();
		containerRegistry.EnsureRegistered<IMainChartProvider>();
		containerRegistry.EnsureRegistered<IMassSpectrumRangeManagerProvider>();
		containerRegistry.EnsureRegistered<INodeDataStateProvider>();
		containerRegistry.EnsureRegistered<INodeInfoProvider>();
		containerRegistry.EnsureRegistered<INodeMenuFactoryProvider>();
		containerRegistry.EnsureRegistered<INodeVisibilityControlProvider>();
		containerRegistry.EnsureRegistered<IProgressDialogProvider>();
		containerRegistry.EnsureRegistered<IReconstructionSectionsProvider>();
		containerRegistry.EnsureRegistered<IRenderDataFactory>();
		containerRegistry.EnsureRegistered<IViewBuilder, ViewBuilder>();
	}
}
