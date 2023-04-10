using System;

namespace Cameca.CustomAnalysis.Utilities;

public sealed class UtilitiesOptions
{
	public bool UseBaseClasses { get; set; } = true;
	public bool UseStandardBaseClasses { get; set; } = false;
	public bool UseDialogs { get; set; } = false;
	public bool UsePersistedIonDataSection { get; set; } = false;
	[Obsolete("Recommended only for simplified conversion of legacy custom analyses")]
	public bool UseLegacy { get; set; } = false;
}
