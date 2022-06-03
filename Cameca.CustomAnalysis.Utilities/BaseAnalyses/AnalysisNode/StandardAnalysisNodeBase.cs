﻿using System;
using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardAnalysisNodeBase : AnalysisNodeBase<IStandardAnalysisNodeBaseServices>
{
	protected StandardAnalysisNodeBase(IStandardAnalysisNodeBaseServices services) : base(services) { }
}


public abstract class StandardAnalysisNodeBase<TServices> : AnalysisNodeBase<TServices>
	where TServices : IStandardAnalysisNodeBaseServices
{
	protected StandardAnalysisNodeBase(TServices services) : base(services)
	{
	}

	protected override void OnInstantiated(INodeInstantiatedEventArgs eventArgs)
	{
		base.OnInstantiated(eventArgs);
		if (Services.NodeMenuFactoryProvider.Resolve(InstanceId) is { } nodeMenuFactory)
		{
			nodeMenuFactory.ContextMenuItems = new[]
			{
				new MenuAction("Rename", new DelegateCommand(RenameCommand)),
			};
		}
	}

	protected virtual void RenameCommand()
	{
		var renameParameters = new EditNameDialogParameters
		{
			Title = "Rename",
			Name = Services.NodeInfoProvider.Resolve(InstanceId)?.Name,
			Validate = x => !string.IsNullOrWhiteSpace(x),
		};
		if (Services.DialogService.TryShowEditNameDialog(out string newName, renameParameters))
		{
			Services.EventAggregator.PublishNodeRename(InstanceId, newName);
		}
	}

	protected sealed override bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> base.InstanceIdFilter(targetEventArgs);
	
	protected sealed override bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> base.MatchExistingViewPredicate(targetType, viewModel);
	
	protected sealed override void RequestDisplayViews()
		=> base.RequestDisplayViews();
}
