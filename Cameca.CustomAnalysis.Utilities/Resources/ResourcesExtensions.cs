using Cameca.CustomAnalysis.Interface;
using System;
using System.Linq;
using System.Windows.Media;

namespace Cameca.CustomAnalysis.Utilities;

public static class ResourcesExtensions
{
	public static Color GetIonColor(this IResources resources, string name, IonFormula? formula = null)
	{
		if (formula is null)
		{
			try
			{
				formula = IonFormulaEx.Parse(name);
			}
			catch (ArgumentException)
			{
				// Name is not a valid formula string: fall back to using Unknown formula
				formula = IonFormula.Unknown;
			}
		}
		var ionTypeInfo = new ResourceIonTypeInfo(name, formula, 0d);
		return resources.IonDisplayInfo.GetColor(ionTypeInfo);
	}

	public static IonTypeInfoRange CreateRange(this IResources resources, string name, double min, double max, IonFormula? formula = null, double? volume = null, Color? color = null)
	{
		formula ??= IonFormulaEx.TryParse(name, out var parsed) ? parsed : IonFormula.Unknown;
		volume ??= resources.ElementData?.Elements.FirstOrDefault(e => e.Symbol == name)?.MolarVolume ?? 0d;
		color ??= resources.GetIonColor(name, formula);
		return new IonTypeInfoRange(name, formula, volume.Value, min, max, color.Value);
	}

	private record ResourceIonTypeInfo(string Name, IonFormula Formula, double Volume) : IIonTypeInfo;
}
