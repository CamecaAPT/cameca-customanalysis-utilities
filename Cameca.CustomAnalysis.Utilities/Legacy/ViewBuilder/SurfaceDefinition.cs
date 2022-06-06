using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class SurfaceDefinition
{
	public IndexedTriangleArray Mesh { get; }

	public Color Color { get; }

	public string Name { get; }

	public SurfaceDefinition(IndexedTriangleArray mesh, Color color, string name)
	{
		Mesh = mesh;
		Color = color;
		Name = name;
	}
}
