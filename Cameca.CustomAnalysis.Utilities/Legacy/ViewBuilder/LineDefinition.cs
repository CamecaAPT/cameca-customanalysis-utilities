using System.Windows.Media;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class LineDefinition
{
	public float[] XValues { get; }
	public float[] YValues { get; }
	public Color Color { get; }
	public string Name { get; }
	public float? Thickness { get; }

	public LineDefinition(float[] xValues, float[] yValues, Color color, string name, float? thickness = null)
	{
		XValues = xValues;
		YValues = yValues;
		Color = color;
		Name = name;
		Thickness = thickness;
	}
}
