using System.Collections;
using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public interface IViewBuilder
{
	IChart2DBuilder AddChart2D(string title, string xAxisLabel, string yAxisLabel);
	IChart3DBuilder AddChart3D(string title);
	void AddTable(string title, IEnumerable rows);
	void AddText(string title, string text);
	void AddHistogram2D(string title, string xAxisLabel, string yAxisLabel, Histogram2DContext histogram2DContext);
	IEnumerable<object> Build();
}
