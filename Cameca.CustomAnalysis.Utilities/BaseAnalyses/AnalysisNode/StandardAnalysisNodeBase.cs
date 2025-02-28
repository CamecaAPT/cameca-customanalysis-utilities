using System;
using System.ComponentModel;
using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Commands;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardAnalysisNodeBase<TProperties> : StandardAnalysisNodeBase<TProperties, IStandardAnalysisNodeBaseServices>
	where TProperties : INotifyPropertyChanged, new()
{
	protected StandardAnalysisNodeBase(IStandardAnalysisNodeBaseServices services, ResourceFactory resourceFactory) : base(services, resourceFactory) { }
}


public abstract class StandardAnalysisNodeBase<TProperties, TServices> : AnalysisNodeBase<TProperties, TServices>
	where TProperties : INotifyPropertyChanged, new()
	where TServices : IStandardAnalysisNodeBaseServices
{
	protected StandardAnalysisNodeBase(TServices services, ResourceFactory resourceFactory) : base(services, resourceFactory)
	{
	}

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.NodeMenuFactoryProvider.Resolve(Id) is { } nodeMenuFactory)
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
			Name = Services.NodeInfoProvider.Resolve(Id)?.Name,
			Validate = x => !string.IsNullOrWhiteSpace(x),
		};
		if (Services.DialogService.TryShowEditNameDialog(out string newName, renameParameters))
		{
			Services.EventAggregator.PublishRenameNode(Id, newName);
		}
	}
}
