using System.Windows.Media;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public interface IChart3DBuilder
{
	void AddPoints(float[] xValues, float[] yValues, float[] zValues, Color color, decimal? samplingCoefficient = null);
	void AddSurface(float[] xPosition, float[] yPosition, float[] zPosition, int[] indices, Color color);
}
