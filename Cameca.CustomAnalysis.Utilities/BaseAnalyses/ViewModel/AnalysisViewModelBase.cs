using System.ComponentModel;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisViewModelBase<TNode> : CoreViewModelBase<IAnalysisViewModelBaseServices> where TNode : class
{
	protected TNode? Node { get; private set; } = null;

	protected AnalysisViewModelBase(IAnalysisViewModelBaseServices services) : base(services) { }

	internal override void OnCreatedCore(ViewModelCreatedEventArgs eventArgs)
	{
		Node = Services.InstanceProvider.Resolve(eventArgs.NodeId) as TNode;
		base.OnCreatedCore(eventArgs);
	}
}
