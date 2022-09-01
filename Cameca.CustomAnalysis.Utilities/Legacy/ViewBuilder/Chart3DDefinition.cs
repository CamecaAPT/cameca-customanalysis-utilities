using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class Chart3DDefinition : IChart3DBuilder
{
	public readonly string Title;

	public List<object> ChartObjects;

	public Chart3DDefinition(string title)
	{
		Title = title;
		ChartObjects = new List<object>();
	}

	public void AddPoints(float[] xValues, float[] yValues, float[] zValues, Color color, decimal? samplingCoefficient = null)
	{
		var positions = CreateVector3Array(xValues, yValues, zValues);
		ChartObjects.Add(new PointsDefinition(positions, color, ""));
	}

	public void AddSurface(float[] xPosition, float[] yPosition, float[] zPosition, int[] indices, Color color)
	{
		var vertices = CreateVector3Array(xPosition, yPosition, zPosition);
		var indicesCopy = new int[indices.Length];
		Array.Copy(indices, indicesCopy, indices.Length);
		var mesh = new IndexedTriangleArray(vertices, indicesCopy);
		ChartObjects.Add(new SurfaceDefinition(mesh, color, ""));
	}

	private Vector3[] CreateVector3Array(float[] xValues, float[] yValues, float[] zValues)
	{
		// Ensure not out of bounds exceptions if all input array lengths do not match
		int maxValues = new int[] { xValues.Length, yValues.Length, zValues.Length, }.Max();
		var positions = new Vector3[maxValues];
		for (int i = 0; i < maxValues; i++)
		{
			positions[i] = new Vector3(xValues[i], yValues[i], zValues[i]);
		}
		return positions;
	}
}
