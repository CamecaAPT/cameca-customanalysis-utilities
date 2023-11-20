using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.ExtensionMethods;

/// <summary>
/// Three axis unit vectors defining the rotation of an <see cref="IGeometricRegion"/>
/// </summary>
/// <param name="X">Unit vector for the X axis of a rotated <see cref="IGeometricRegion"/></param>
/// <param name="Y">Unit vector for the Y axis of a rotated <see cref="IGeometricRegion"/></param>
/// <param name="Z">Unit vector for the Z axis of a rotated <see cref="IGeometricRegion"/></param>
public record AxisVectors(Vector3 X, Vector3 Y, Vector3 Z);

public static class GeometricRegionExtensions
{
	public static Vector3 GetCenter(this IGeometricRegion geometricRegion)
	{
		if (!Matrix4x4.Decompose(geometricRegion.SrtTransformation, out _, out _, out var translation))
		{
			throw new InvalidOperationException("Expected a Scale,Rotation,Translation matrix but could not decompose.");
		}
		return translation;
	}

	public static Vector3 GetDimensions(this IGeometricRegion geometricRegion)
	{
		if (!Matrix4x4.Decompose(geometricRegion.SrtTransformation, out var scale, out _, out _))
		{
			throw new InvalidOperationException("Expected a Scale,Rotation,Translation matrix but could not decompose.");
		}
		return scale;
	}

	public static AxisVectors GetAxisVectors(this IGeometricRegion geometricRegion)
	{
		if (!Matrix4x4.Decompose(geometricRegion.SrtTransformation, out _, out var rotation, out _))
		{
			throw new InvalidOperationException("Expected a Scale,Rotation,Translation matrix but could not decompose.");
		}
		return new AxisVectors(
			Vector3.Transform(Vector3.UnitX, rotation),
			Vector3.Transform(Vector3.UnitY, rotation),
			Vector3.Transform(Vector3.UnitZ, rotation));
	}
}
