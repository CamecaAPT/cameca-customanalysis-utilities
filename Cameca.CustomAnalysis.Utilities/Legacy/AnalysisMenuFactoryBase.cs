using Cameca.CustomAnalysis.Interface;
using Prism.Commands;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public abstract class AnalysisMenuFactoryBase : IAnalysisMenuFactory
{
	protected abstract INodeDisplayInfo DisplayInfo { get; }
	protected abstract string NodeUniqueId { get; }

	protected readonly IEventAggregator EventAggregator;

	protected AnalysisMenuFactoryBase(IEventAggregator eventAggregator)
	{
		EventAggregator = eventAggregator;
	}

	public IMenuItem CreateMenuItem(IAnalysisMenuContext context)
	{
		return new MenuAction(
			DisplayInfo.Title,
			new DelegateCommand(()
				=> EventAggregator.PublishCreateNode(NodeUniqueId, context.NodeId, DisplayInfo.Title, DisplayInfo.Icon)),
			DisplayInfo.Icon);
	}

	public abstract AnalysisMenuLocation Location { get; }
}
