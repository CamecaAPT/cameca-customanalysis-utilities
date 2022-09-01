using System.Numerics;
using Cameca.CustomAnalysis.Interface;
using Microsoft.Toolkit.HighPerformance;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public class Histogram2DContext
{
	public ReadOnlyMemory2D<float> Values { get; }
	public Vector2 BinSize { get; }
	public IColorMap ColorMap { get; }
	public Vector2 Origin { get; }
	public float? MinValue { get; }

	public Histogram2DContext(ReadOnlyMemory2D<float> values, Vector2 binSize, IColorMap colorMap, Vector2 origin = default, float? minValue = null)
	{
		Values = values;
		BinSize = binSize;
		ColorMap = colorMap;
		Origin = origin;
		MinValue = minValue;
	}
}
