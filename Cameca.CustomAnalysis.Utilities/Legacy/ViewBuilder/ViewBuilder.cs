using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class ViewBuilder : IViewBuilder
{
	private readonly IRenderDataFactory _renderDataFactory;
	private readonly List<object> _tabViewModelDefinitions;

	public ViewBuilder(IRenderDataFactory renderDataFactory)
	{
		_renderDataFactory = renderDataFactory;
		_tabViewModelDefinitions = new List<object>();
	}

	public IChart2DBuilder AddChart2D(string title, string xAxisLabel, string yAxisLabel)
	{
		var chart2dDefinition = new Chart2DDefinition(title, xAxisLabel, yAxisLabel);
		_tabViewModelDefinitions.Add(chart2dDefinition);
		return chart2dDefinition;
	}

	public IChart3DBuilder AddChart3D(string title)
	{
		var chart3dDefinition = new Chart3DDefinition(title);
		_tabViewModelDefinitions.Add(chart3dDefinition);
		return chart3dDefinition;
	}

	public void AddHistogram2D(string title, string xAxisLabel, string yAxisLabel, Histogram2DContext histogram2DContext)
	{
		var histogram2dDefinition = new Histogram2DDefinition(
			histogram2DContext.Values,
			histogram2DContext.BinSize,
			histogram2DContext.ColorMap,
			histogram2DContext.Origin,
			histogram2DContext.MinValue);
			_tabViewModelDefinitions.Add(new HistogramChart2DDefinition(
			title,
			xAxisLabel,
			yAxisLabel,
			histogram2dDefinition));
	}

	public void AddTable(string title, IEnumerable rows)
	{
		_tabViewModelDefinitions.Add(new TableDefinition(title, rows));
	}

	public void AddText(string title, string text)
	{
		_tabViewModelDefinitions.Add(new TextTabViewModel(title, text, new TextBoxOptions()));
	}

	public void AddText(string title, string text, TextBoxOptions options)
	{
		_tabViewModelDefinitions.Add(new TextTabViewModel(title, text, options));
	}

	public IEnumerable<object> Build()
	{
		foreach (var viewModelDefinition in _tabViewModelDefinitions)
		{
			switch (viewModelDefinition)
			{
				case Chart3DDefinition chart3D:
					yield return new Chart3DTabViewModel(
						chart3D.Title,
						chart3D.ChartObjects.Select(BuildRenderData));
					break;
				case Chart2DDefinition chart2D:
					yield return new Chart2DTabViewModel(
						chart2D.Title,
						chart2D.XAxisLabel,
						chart2D.YAxisLabel,
						chart2D.ChartObjects.Select(BuildRenderData));
					break;
				case TableDefinition table:
					yield return new TableTabViewModel(
						table.Title,
						table.Rows.OfType<object>().ToList());
					break;
				case HistogramChart2DDefinition histogram2D:
					yield return new Histogram2DTabViewModel(
						histogram2D.Title,
						histogram2D.XAxisLabel,
						histogram2D.YAxisLabel,
						histogram2D.ChartObjects.Select(BuildRenderData));
					break;
				default:
					yield return viewModelDefinition;
					break;
			}
		}
	}

	private IRenderData BuildRenderData(object definition)
	{
		switch (definition)
		{
			case PointsDefinition points:
				return _renderDataFactory.CreatePoints(
					points.Positions,
					points.Color,
					name: points.Name);
			case SurfaceDefinition surface:
				return _renderDataFactory.CreateSurface(
					surface.Mesh,
					surface.Color,
					name: surface.Name);
			case LineDefinition line:
				return _renderDataFactory.CreateLine(
					// Lines can be used in either 2D or 3D chart, therefor accept a Vector3. If displaying in 2D chart, Z is vertical axis
					line.XValues.Zip(line.YValues).Select(p => new Vector3(p.First, 0f, p.Second)).ToArray(),
					line.Color,
					line.Thickness,
					name: line.Name);
			case HistogramDefinition histogram:
				return _renderDataFactory.CreateHistogram(
					histogram.XValues.Zip(histogram.YValues).Select(p => new Vector2(p.First, p.Second)).ToArray(),
					histogram.Color,
					verticalSlices: histogram.VerticalSlices?.ToList(),
					thickness: histogram.Thickness,
					name: histogram.Name);
			case Histogram2DDefinition histogram2D:
				var renderData = _renderDataFactory.CreateHistogram2D();
				renderData.Update(histogram2D.Values, histogram2D.BinSize, histogram2D.Origin, histogram2D.MinValue);
				renderData.ColorMap = histogram2D.ColorMap;
				return renderData;
		}
		throw new NotImplementedException("No conversion to chart data has been implemented");
	}
}
