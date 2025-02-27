using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cameca.CustomAnalysis.Utilities;

public static class IonFormulaEx
{
	// Matches a single component in an ion formula. Does not validate if the component name group is an actual ion. A count of "1" is considered invalid, should be no value or 2 or greater
	private static readonly Regex componentPattern = new Regex(@"(?<name>[A-Z][a-z]?)(?<count>[2-9]|[1-9][0-9]+)?");
	// In an ideal world, this might be taken from the main AP Suite ion list, but that requires a substantial amount more overhead and realistically this will almost never change.
	// It is much more understandable to simply replicate the valid ion type list here
	private static readonly ISet<string> validIons = new HashSet<string>(new string[]
	{
		"H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg",
		"Al", "Si", "P", "S", "Cl", "Ar", "K", "Ca", "Sc", "Ti", "V", "Cr",
		"Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br",
		"Kr", "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd",
		"Ag", "Cd", "In", "Sn", "Sb", "Te", "I", "Xe", "Cs", "Ba", "La",
		"Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er",
		"Tm", "Yb", "Lu", "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au",
		"Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn", "Fr", "Ra", "Ac", "Th",
		"Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md",
		"No", "Lr", "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn",
		"Nh", "Fl", "Mc", "Lv", "Ts", "Og"
	});

	/// <summary>
	/// Parse a formula string into a <see cref="IonFormula"/> instance
	/// </summary>
	/// <param name="formula"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">argument is not a valid formula string</exception>
	public static IonFormula Parse(string formula)
	{
		// Used to validate that the entire provided formula string was matched into components. If the total sum of matched components is not the length of the original string,
		// then some part of the input was not matched and the formula should be considered invalid
		int parsedLength = 0;

		var components = new List<IonFormula.Component>();
		var componentMatches = componentPattern.Matches(formula);
		var foundAtoms = new List<string>();
		foreach (Match match in componentMatches)
		{
			var name = match.Groups["name"].Value;
			var strCount = match.Groups["count"].Value;
			parsedLength += name.Length;
			parsedLength += strCount.Length;
			// Validate atom names
			if (!validIons.Contains(name))
			{
				throw new ArgumentException($"Invalid formula: \"{name}\" is not a valid component atom name", nameof(formula));
			}
			// Validate for atom duplication in formula
			if (foundAtoms.Contains(name))
			{
				throw new ArgumentException($"Invalid formula: \"{name}\" found more than once in the formula. Each atom should only be used once in a formula", nameof(formula));
			}
			foundAtoms.Add(name);
			components.Add(new IonFormula.Component(name, int.TryParse(strCount, out int count) ? count : 1));
		}

		if (parsedLength != formula.Length)
		{
			throw new ArgumentException("Invalid formula: Input could not be completely parsed into only valid atom and count components", nameof(formula));
		}
		return new IonFormula(components);
	}

	public static bool TryParse(string formula, out IonFormula parsedFormula)
	{
		try
		{
			parsedFormula = Parse(formula);
			return true;
		}
		catch (ArgumentException)
		{
			parsedFormula = IonFormula.Unknown;
			return false;
		}
	}

	/// <summary>
	/// Creates 
	/// </summary>
	/// <param name="formula"></param>
	/// <returns></returns>
	public static string ToStringHillNotation(this IonFormula formula)
	{
		var sb = new StringBuilder();
		// If carbon, then carbon first, then hydrogen, then all other alphabetically
		if (formula.Select(c => c.Key).Contains("C"))
		{
			var carbonComponent = formula.First(c => c.Key == "C");
			AddComponent(sb, carbonComponent.Key, carbonComponent.Value);
			if (formula.TryGetValue("H", out int hCount))
			{
				AddComponent(sb, "H", hCount);
			}
			foreach (var (name, count) in formula.OrderBy(c => c.Key))
			{
				if (name != "C" && name != "H")
				{
					AddComponent(sb, name, count);
				}
			}
		}
		// If no carbon, then all alphabetically, including hydrogen
		else {

			foreach (var (name, count) in formula.OrderBy(c => c.Key))
			{
				AddComponent(sb, name, count);
			}
		}
		return sb.ToString();

		static void AddComponent(StringBuilder sb, string name, int count)
		{
			sb.Append(name);
			if (count > 1)
			{
				sb.Append(count);
			}
		}
	}
}
