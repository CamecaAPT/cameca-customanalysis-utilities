using System.Collections.Generic;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class Histogram2DTabViewModel : Chart2DTabViewModel
{
	public Histogram2DTabViewModel(string title, string xAxis, string yAxis, IEnumerable<IRenderData> content)
		: base(title, xAxis, yAxis, content)
	{
	}
}
