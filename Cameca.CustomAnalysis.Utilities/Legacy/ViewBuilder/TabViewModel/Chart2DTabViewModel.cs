using System;
using System.Collections.Generic;
using System.Linq;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class Chart2DTabViewModel : IDisposable
{
	public string Title { get; }

	public string XAxis { get; }

	public string YAxis { get; }

	public ICollection<IRenderData> RenderData { get; }

	public Chart2DTabViewModel(string title, string xAxis, string yAxis, IEnumerable<IRenderData> content)
	{
		Title = title;
		XAxis = xAxis;
		YAxis = yAxis;
		RenderData = content.ToList();
	}

	public void Dispose()
	{
		foreach (var item in RenderData)
		{
			item?.Dispose();
		}
	}
}
