using System.Numerics;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.ExtensionMethods;

public static class ExtentsExtensions
{
	public static Vector3 GetDimensions(this Extents extent) => extent.Max - extent.Min;
}
