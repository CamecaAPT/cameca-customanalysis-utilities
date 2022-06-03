using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardDataFilterNodeBase : DataFilterNodeBase
{
	private readonly INodeMenuFactoryProvider _nodeMenuFactoryProvider;
	private readonly INodeInfoProvider _nodeInfoProvider;
	private readonly IDialogService _dialogService;

	protected StandardDataFilterNodeBase(IDataFilterNodeBaseServices services,
		INodeMenuFactoryProvider nodeMenuFactoryProvider,
		INodeInfoProvider nodeInfoProvider,
		IDialogService dialogService)
		: base(services)
	{
		_nodeMenuFactoryProvider = nodeMenuFactoryProvider;
		_nodeInfoProvider = nodeInfoProvider;
		_dialogService = dialogService;
	}

	protected override void OnInstantiated(INodeInstantiatedEventArgs eventArgs)
	{
		base.OnInstantiated(eventArgs);
		if (_nodeMenuFactoryProvider.Resolve(InstanceId) is { } nodeMenuFactory)
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
			Name = _nodeInfoProvider.Resolve(InstanceId)?.Name,
			Validate = x => !string.IsNullOrWhiteSpace(x),
		};
		if (_dialogService.TryShowEditNameDialog(out string newName, renameParameters))
		{
			Services.EventAggregator.PublishNodeRename(InstanceId, newName);
		}
	}

	protected sealed override bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> base.InstanceIdFilter(targetEventArgs);
}
