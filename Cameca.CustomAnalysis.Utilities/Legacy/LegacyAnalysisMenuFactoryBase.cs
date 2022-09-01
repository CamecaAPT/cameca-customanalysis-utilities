using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public abstract class LegacyAnalysisMenuFactoryBase : IAnalysisMenuFactory
{
	protected abstract INodeDisplayInfo DisplayInfo { get; }
	protected abstract string NodeUniqueId { get; }

	protected readonly IEventAggregator EventAggregator;
	protected readonly IDialogService DialogService;

	protected LegacyAnalysisMenuFactoryBase(IEventAggregator eventAggregator, IDialogService dialogService)
	{
		EventAggregator = eventAggregator;
		DialogService = dialogService;
	}

	public IMenuItem CreateMenuItem(IAnalysisMenuContext context)
	{
		return new MenuAction(
			DisplayInfo.Title,
			new DelegateCommand(TryCreateAnalysis),
			DisplayInfo.Icon);

		void TryCreateAnalysis()
		{
			var parameters = new EditNameDialogParameters
			{
				Title = "Create Analysis",
				Name = DisplayInfo.Title,
				Validate = x => !string.IsNullOrWhiteSpace(x),
			};
			if (DialogService.TryShowEditNameDialog(out string nodeName, parameters))
			{
				EventAggregator.PublishCreateNode(NodeUniqueId, context.NodeId, nodeName, DisplayInfo.Icon);
			}
		}
	}

	public abstract AnalysisMenuLocation Location { get; }
}
