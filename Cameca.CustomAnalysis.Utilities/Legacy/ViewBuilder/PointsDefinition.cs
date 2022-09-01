using System.Windows.Media;
using System.Numerics;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class PointsDefinition
{
	public Vector3[] Positions { get; }

	public Color Color { get; }

	public string Name { get; }

	public PointsDefinition(Vector3[] positions, Color color, string name)
	{
		Positions = positions;
		Color = color;
		Name = name;
	}
}
