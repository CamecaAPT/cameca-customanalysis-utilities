using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public interface ICustomAnalysis<in TOptions>
{
	void Run(IIonData ionData, TOptions options, IViewBuilder viewBuilder);
}
