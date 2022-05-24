using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class StandardDataFilterNodeBase : DataFilterNodeBase
{
	protected StandardDataFilterNodeBase(IDataFilterNodeBaseServices services) : base(services)
	{
	}

	protected sealed override bool InstanceIdFilter(INodeTargetEvent targetEventArgs)
		=> base.InstanceIdFilter(targetEventArgs);
}
