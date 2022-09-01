using System.Collections.Generic;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class HistogramDefinition
{
	public float[] XValues { get; }
	public float[] YValues { get; }
	public Color Color { get; }
	public string Name { get; }
	public float Thickness { get; }
	public ICollection<IChart2DSlice>? VerticalSlices { get; }

	public HistogramDefinition(float[] xValues, float[] yValues, Color color, ICollection<IChart2DSlice>? verticalSlices, string name, float thickness = 1f)
	{
		XValues = xValues;
		YValues = yValues;
		Color = color;
		Name = name;
		Thickness = thickness;
		VerticalSlices = verticalSlices;
	}
}
