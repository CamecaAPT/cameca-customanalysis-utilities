using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal abstract class Chart2DDefinitionBase
{
	public readonly string Title;
	public readonly string XAxisLabel;
	public readonly string YAxisLabel;
	public List<object> ChartObjects;

	protected Chart2DDefinitionBase(string title, string xAxisLabel, string yAxisLabel, List<object> chartObjects)
	{
		Title = title;
		XAxisLabel = xAxisLabel;
		YAxisLabel = yAxisLabel;
		ChartObjects = chartObjects;
	}
}
