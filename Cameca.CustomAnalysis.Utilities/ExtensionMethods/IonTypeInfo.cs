using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities;

public class IonTypeInfo : IIonTypeInfo
{
	public string Name { get; }

	public IonFormula Formula { get; }

	public double Volume { get; }

	public IonTypeInfo(string name, IonFormula formula, double volume)
	{
		Name = name;
		Formula = formula;
		Volume = volume;
	}

	public static IonTypeInfo CreateUnknown(string name)
		=> CreateUnknown(name, 0d);

	public static IonTypeInfo CreateUnknown(string name, double volume)
		=> new IonTypeInfo(name, IonFormula.Unknown, volume);

	/// <summary>
	/// Parses a formula string into an <see cref="IonTypeInfo"/>,
	/// throwing an <see cref="ArgumentException"/> if the input can not be parsed into a valid formula
	/// </summary>
	/// <param name="formula"></param>
	/// <param name="volume"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"><paramref name="formula"/> is not a valid formula string</exception>
	public static IonTypeInfo Parse(string formula, double volume)
	{
		var ionFormula = IonFormulaEx.Parse(formula);
		return new IonTypeInfo(formula, ionFormula, volume);
	}

	/// <summary>
	/// Parses a formula string into an <see cref="IonTypeInfo"/>,
	/// throwing an <see cref="ArgumentException"/> if the input can not be parsed into a valid formula.
	/// Computes <see cref="IIonTypeInfo.Volume"/> from <see cref="IElementDataSet"/> if provided, else sets volume to <c>0d</c>
	/// </summary>
	/// <param name="formula"></param>
	/// <param name="volume"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"><paramref name="formula"/> is not a valid formula string</exception>
	public static IonTypeInfo Parse(string formula, IElementDataSet? elementDataSet = null)
	{
		var ionFormula = IonFormulaEx.Parse(formula);
		var volume = ionFormula.CalculateAtomicVolume(elementDataSet);
		return new IonTypeInfo(formula, ionFormula, volume);
	}


	/// <summary>
	/// Parses a formula string into an <see cref="IonTypeInfo"/>.
	/// If unable to parse, creates an unknown type instead.
	/// Computes <see cref="IIonTypeInfo.Volume"/> from <see cref="IElementDataSet"/> if provided, else sets volume to <c>0d</c>
	/// </summary>
	/// <param name="formula"></param>
	/// <param name="volume"></param>
	/// <returns></returns>
	public static IonTypeInfo ParseOrUnknown(string formula, IElementDataSet? elementDataSet = null)
	{
		IonFormula ionFormula;
		try
		{
			ionFormula = IonFormulaEx.Parse(formula);
		}
		catch (ArgumentException)
		{
			ionFormula = IonFormula.Unknown;
		}
		var volume = ionFormula.CalculateAtomicVolume(elementDataSet);
		return new IonTypeInfo(formula, ionFormula, volume);
	}
}
