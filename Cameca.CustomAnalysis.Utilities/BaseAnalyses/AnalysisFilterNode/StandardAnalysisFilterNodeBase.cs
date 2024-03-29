﻿using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Commands;
using System;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardAnalysisFilterNodeBase : StandardAnalysisFilterNodeBase<IStandardAnalysisFilterNodeBaseServices>
{
	protected StandardAnalysisFilterNodeBase(IStandardAnalysisFilterNodeBaseServices services) : base(services) { }

}

public abstract class StandardAnalysisFilterNodeBase<TServices> : AnalysisFilterNodeBase<TServices>
	where TServices : IStandardAnalysisFilterNodeBaseServices
{
	protected StandardAnalysisFilterNodeBase(TServices services) : base(services)
	{
	}

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
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
			Services.EventAggregator.PublishRenameNode(InstanceId, newName);
		}
	}

	protected sealed override bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> base.InstanceIdFilter(targetEventArgs);

	protected sealed override bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> base.MatchExistingViewPredicate(targetType, viewModel);

	protected sealed override void RequestDisplayViews()
		=> base.RequestDisplayViews();
}
