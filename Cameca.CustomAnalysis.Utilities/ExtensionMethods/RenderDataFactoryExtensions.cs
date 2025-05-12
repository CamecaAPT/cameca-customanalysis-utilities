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

	public static IValuePointsRenderData CreateValuePoints(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<Vector4> positionsWithValues,
		IColorMap? colorMap = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateValuePoints();
		renderData.PositionsWithValues = positionsWithValues;
		renderData.ColorMap = colorMap;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static ISpheresRenderData CreateSpheres(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<Vector3> positions,
		Color color = default,
		string? name = null,
		bool isVisible = true,
		float? radius = null,
		int? resolution = null)
	{
		var renderData = renderDataFactory.CreateSpheres();
		renderData.Positions = positions;
		renderData.Color = color;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		if (radius.HasValue)
			renderData.Radius = radius.Value;
		if (resolution.HasValue)
			renderData.Resolution = resolution.Value;
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

	public static IHistogram2DRenderData CreateHistogram2D(this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory2D<float> values,
		Vector2 binSize,
		IColorMap? colorMap = null,
		Vector2? min = null,
		double? height = null,
		double? width = null,
		string? name = null,
		bool isVisible = true,
		float? minValue = null,
		float? maxValue = null)
	{
		var renderData = renderDataFactory.CreateHistogram2D();
		renderData.Update(values, binSize, min ?? default, minValue, maxValue);
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

	public static ISelectionWidgetRenderData CreateSelectionWidget(
		this IRenderDataFactory renderDataFactory,
		Vector3 scaling,
		SelectionWidgetController controllers = SelectionWidgetController.All,
		Color color = default,
		Vector3 translation = default,
		float? controllersSize = null,
		float? lineThickness = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateSelectionWidget();
		renderData.Scaling = scaling;
		renderData.Controllers = controllers;
		renderData.ControllersColor = color;
		renderData.LineColor = color;
		renderData.Translation = translation;
		if (controllersSize.HasValue)
			renderData.ControllersSize = controllersSize.Value;
		if (lineThickness.HasValue)
			renderData.LineThickness = lineThickness.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static ISeriesRenderData CreateSeries(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<Vector3> points,
		Color color = default,
		float? thickness = null,
		MarkerShape markerShape = MarkerShape.Circle,
		LineStyle lineStyle = LineStyle.Solid,
		int? markerSize = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateSeries();
		renderData.Positions = points;
		renderData.Color = color;
		renderData.MarkerColor = color;
		if (thickness.HasValue)
			renderData.Thickness = thickness.Value;
		renderData.MarkerShape = markerShape;
		renderData.LineStyle = lineStyle;
		if (markerSize.HasValue)
			renderData.MarkerSize = markerSize.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static IErrorRenderData CreateErrorBars(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<ErrorDataPoint>? data,
		Color color = default,
		int? dashWidth = null,
		float? alpha = null,
		float? lineThickness = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateError();
		renderData.DisplayType = ErrorDisplayType.Bars;
		renderData.Data = data;
		renderData.Color = color;
		if (dashWidth.HasValue)
			renderData.DashWidth = dashWidth.Value;
		if (alpha.HasValue)
			renderData.Alpha = alpha.Value;
		if (lineThickness.HasValue)
			renderData.LineThickness = lineThickness.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}

	public static IErrorRenderData CreateErrorStripes(
		this IRenderDataFactory renderDataFactory,
		ReadOnlyMemory<ErrorDataPoint>? data,
		Color color = default,
		float? alpha = null,
		float? lineThickness = null,
		string? name = null,
		bool isVisible = true)
	{
		var renderData = renderDataFactory.CreateError();
		renderData.DisplayType = ErrorDisplayType.Stripes;
		renderData.Data = data;
		renderData.Data = data;
		renderData.Color = color;
		if (alpha.HasValue)
			renderData.Alpha = alpha.Value;
		if (lineThickness.HasValue)
			renderData.LineThickness = lineThickness.Value;
		if (name is not null)
			renderData.Name = name;
		renderData.IsVisible = isVisible;
		return renderData;
	}
}
