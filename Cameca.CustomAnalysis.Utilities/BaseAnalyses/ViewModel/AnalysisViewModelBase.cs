using System.ComponentModel;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisViewModelBase<TNode> : CoreViewModelBase<IAnalysisViewModelBaseServices> where TNode : class
{
	protected TNode? Node { get; private set; } = null;

	protected AnalysisViewModelBase(IAnalysisViewModelBaseServices services) : base(services) { }

	internal override void OnAddedCore(ViewModelAddedEventArgs eventArgs)
	{
		Node = Services.InstanceProvider.Resolve(eventArgs.OwnerNodeId) as TNode;
		base.OnAddedCore(eventArgs);
	}
}
