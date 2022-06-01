using System;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardAnalysisNodeBase : AnalysisNodeBase
{
	protected StandardAnalysisNodeBase(IAnalysisNodeBaseServices services)
		: base(services)
	{
	}

	protected sealed override bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> base.InstanceIdFilter(targetEventArgs);
	
	protected sealed override bool MatchExistingViewPredicate(Type targetType, object viewModel)
		=> base.MatchExistingViewPredicate(targetType, viewModel);
	
	protected sealed override void RequestDisplayViews()
		=> base.RequestDisplayViews();
}
