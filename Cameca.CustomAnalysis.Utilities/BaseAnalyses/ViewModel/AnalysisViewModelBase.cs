using Cameca.CustomAnalysis.Interface;
using System.Xml.Linq;
using Cameca.CustomAnalysis.Utilities.BaseAnalyses;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisViewModelBase<TNode> : CoreViewModelBase<IAnalysisViewModelBaseServices> where TNode : class
{
	private RequiredSetOnce<TNode> _node = new();
	protected TNode Node => _node;

	protected AnalysisViewModelBase(IAnalysisViewModelBaseServices services) : base(services) { }

	internal override void OnCreatedCore(ViewModelCreatedEventArgs eventArgs)
	{
		_node.Value = (TNode)Services.InstanceProvider.Resolve(eventArgs.OwnerNodeId)!;
		base.OnCreatedCore(eventArgs);
	}
}
