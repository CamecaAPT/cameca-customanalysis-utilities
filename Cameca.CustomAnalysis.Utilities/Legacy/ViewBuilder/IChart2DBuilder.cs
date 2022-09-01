using System.Collections.Generic;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public interface IChart2DBuilder
{
	void AddLine(float[] xValues, float[] yValues, Color color, string name, float? thickness = null);
	void AddHistogram(float[] xValues, float[] yValues, Color color, ICollection<IChart2DSlice>? verticalSlices, string name, float thickness = 1f);
}
