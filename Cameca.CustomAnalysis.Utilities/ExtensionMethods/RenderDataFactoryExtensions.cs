using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;
using CommunityToolkit.HighPerformance;

namespace Cameca.CustomAnalysis.Utilities;

/// <summary>
/// Extensions for creating parameterized <see cref="IRenderData"/> instances with a single function call
/// </summary>
public static class RenderDataFactoryExtensions
{
	public static IPointsRenderData CreatePoints(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<Vector3> positions,
		Color color = default,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreatePoints();
		renderData.Positions = positions;
		renderData.Color = color;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static ISurfaceRenderData CreateSurface(
		this IRenderDataFactory renderDataFactory,
		IndexedTriangleArray mesh,
		Color color = default,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateSurface();
		renderData.SurfaceMesh = mesh;
		renderData.Color = color;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static ILineRenderData CreateLine(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<Vector3> points,
		Color color = default,
		float? thickness = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateLine();
		renderData.Positions = points;
		renderData.Color = color;
		if (thickness.HasValue)
			renderData.Thickness = thickness.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static IHistogramRenderData CreateHistogram(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<Vector2> values,
		Color color = default,
		float? thickness = null,
		AveragingType? aggregationType = null,
		IReadOnlyList<IChart2DSlice>? verticalSlices = null,
		int? customStep = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateHistogram();
		renderData.Values = values;
		renderData.Color = color;
		renderData.Color = color;
		if (thickness.HasValue)
			renderData.Thickness = thickness.Value;
		if (aggregationType.HasValue)
			renderData.AggregationType = aggregationType.Value;
		if (verticalSlices is not null)
			renderData.VerticalSlices = verticalSlices;
		if (customStep.HasValue)
			renderData.CustomStep = customStep.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static IHistogram2DRenderData CreateHistogram2D(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory2D<float> values,
		Vector2 binSize,
		IColorMap? colorMap = null,
		Vector2? min = null,
		double? height = null,
		double? width = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateHistogram2D();
		renderData.Update(values, binSize, min ?? default);
		renderData.ColorMap = colorMap;
		if (height.HasValue)
			renderData.Height = height.Value;
		if (width.HasValue)
			renderData.Width = width.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}
}
