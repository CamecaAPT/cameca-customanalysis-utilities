using Cameca.CustomAnalysis.Interface;
using Prism.Commands;
using Prism.Events;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisMenuFactoryBase : IAnalysisMenuFactory
{
	protected readonly IEventAggregator EventAggregator;

	protected abstract INodeDisplayInfo DisplayInfo { get; }

	protected abstract string NodeUniqueId { get; }

	public abstract AnalysisMenuLocation Location { get; }

	public virtual string? ToolTip { get; } = null;

	public virtual bool IsEnabled { get; } = true;

	protected AnalysisMenuFactoryBase(IEventAggregator eventAggregator)
	{
		EventAggregator = eventAggregator;
	}

	public IMenuItem CreateMenuItem(IAnalysisMenuContext context)
	{
		return new MenuAction(
			DisplayInfo.Title,
			new DelegateCommand(() => EventAggregator.PublishCreateNode(NodeUniqueId, context.NodeId, DisplayInfo.Title, DisplayInfo.Icon)),
			DisplayInfo.Icon,
			IsEnabled,
			ToolTip);
	}
}
