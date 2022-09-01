using System;
using System.Collections.Generic;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class Chart2DDefinition : Chart2DDefinitionBase, IChart2DBuilder
{
	public Chart2DDefinition(string title, string xAxisLabel, string yAxisLabel)
		: base(title, xAxisLabel, yAxisLabel, new List<object>())
	{
	}

	public void AddLine(float[] xValues, float[] yValues, Color color, string name, float? thickness = null)
	{
		ChartObjects.Add(new LineDefinition(xValues, yValues, color, name, thickness));
	}

	public void AddHistogram(float[] xValues, float[] yValues, Color color, ICollection<IChart2DSlice>? verticalSlices, string name, float thickness = 1)
	{
		// Copy arrays
		var xValuesCopy = new float[xValues.Length];
		var yValuesCopy = new float[yValues.Length];
		Array.Copy(xValues, xValuesCopy, xValues.Length);
		Array.Copy(yValues, yValuesCopy, yValues.Length);
		ChartObjects.Add(new HistogramDefinition(xValuesCopy, yValuesCopy, color, verticalSlices, name, thickness));
	}
}
