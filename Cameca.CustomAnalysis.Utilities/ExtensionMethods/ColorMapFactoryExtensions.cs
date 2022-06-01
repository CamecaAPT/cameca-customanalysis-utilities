using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

/// <summary>
/// Extensions for creating parameterized <see cref="IColorMap"/> instances with a single function call
/// </summary>
public static class ColorMapFactoryExtensions
{
	public static IColorMap CreateColorMap(
		this IColorMapFactory colorMapFactory,
		Color? bottom = null,
		Color? nanColor = null,
		Color? outOfRangeBottom = null,
		Color? outOfRangeTop = null,
		Color? top = null,
		IEnumerable<IColorStop>? colorStops = null)
	{
		var colorMap = colorMapFactory.CreateColorMap();
		if (bottom.HasValue)
			colorMap.Bottom = bottom.Value;
		if (nanColor.HasValue)
			colorMap.NanColor = nanColor.Value;
		if (outOfRangeBottom.HasValue)
			colorMap.OutOfRangeBottom = outOfRangeBottom.Value;
		if (outOfRangeTop.HasValue)
			colorMap.OutOfRangeTop = outOfRangeTop.Value;
		if (top.HasValue)
			colorMap.Top = top.Value;
		foreach (var colorStop in colorStops ?? Enumerable.Empty<IColorStop>())
			colorMap.ColorStops.Add(colorStop);
		return colorMap;
	}

	public static IColorStop CreateColorStop(
		this IColorMapFactory colorMapFactory,
		float relativePosition,
		Color top,
		Color bottom)
	{
		var colorStop = colorMapFactory.CreateColorStop();
		colorStop.RelativePosition = relativePosition;
		colorStop.TopColor = top;
		colorStop.BottomColor = bottom;
		return colorStop;
	}

	public static IColorStop CreateColorStop(
		this IColorMapFactory colorMapFactory,
		float relativePosition,
		Color color)
	{
		return colorMapFactory.CreateColorStop(relativePosition, color, color);
	}
}
