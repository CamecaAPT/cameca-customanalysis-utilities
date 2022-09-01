using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class HistogramChart2DDefinition : Chart2DDefinitionBase
{
	public HistogramChart2DDefinition(string title, string xAxisLabel, string yAxisLabel, object chartObject)
		: base(title, xAxisLabel, yAxisLabel, new List<object> { chartObject })
	{
	}
}
