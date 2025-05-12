using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities;

public static class ElementExtensions
{
	/// <summary>
	/// Avogadro constant
	/// </summary>
	/// <remarks>
	/// The number of constituent particles in one mole
	/// </remarks>
	public const double AvogadroConstant = 6.02214076e23;

	/// <summary>
	/// (1e7)^3: Coefficient to convert cm^3 to nm^3
	/// </summary>
	public const double Cm3ToNm3Coefficient = 1e21;

	/// <summary>
	/// Coefficient to convert from molecular volume to atomic volume
	/// </summary>
	/// <remarks>
	/// Converts from units: vol/mol in cm^3 to vol/atom in nm^3
	/// </remarks>
	public const double AtomicVolumeConversionCoefficient = Cm3ToNm3Coefficient / AvogadroConstant;

	/// <summary>
	/// Returns atomic volume of a single atom of the element in nm^3
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	public static double AtomicVolume(this IElement element)
	{
		return element.MolarVolume * AtomicVolumeConversionCoefficient;
	}
}
