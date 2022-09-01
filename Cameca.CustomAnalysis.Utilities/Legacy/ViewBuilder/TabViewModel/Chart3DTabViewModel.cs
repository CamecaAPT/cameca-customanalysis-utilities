using System;
using System.Collections.Generic;
using System.Linq;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class Chart3DTabViewModel : IDisposable
{
	public string Title { get; }

	public ICollection<IRenderData> RenderData { get; }

	public Chart3DTabViewModel(string title, IEnumerable<IRenderData> content)
	{
		Title = title;
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
