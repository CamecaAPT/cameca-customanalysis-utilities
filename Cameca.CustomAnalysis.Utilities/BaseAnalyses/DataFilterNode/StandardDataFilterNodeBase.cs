﻿using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Commands;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardDataFilterNodeBase : StandardDataFilterNodeBase<IStandardDataFilterNodeBaseServices>
{
	protected StandardDataFilterNodeBase(IStandardDataFilterNodeBaseServices services) : base(services)
	{
	}
}


public abstract class StandardDataFilterNodeBase<TServices> : DataFilterNodeBase<TServices>
	where TServices : IStandardDataFilterNodeBaseServices
{
	protected StandardDataFilterNodeBase(TServices services) : base(services) { }

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
}
