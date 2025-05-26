using System.Numerics;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public static class ErrorDataPointEx
{
	public static ErrorDataPoint Create(this Vector2 point, float plus, float minus, float depth = 0f)
	{
		return new ErrorDataPoint(new Vector3(point.X, depth, point.Y), plus, minus);
	}

	public static ErrorDataPoint Create(float x, float y, float plus, float minus, float depth = 0f)
	{
		return new ErrorDataPoint(new Vector3(x, depth, y), plus, minus);
	}
}
