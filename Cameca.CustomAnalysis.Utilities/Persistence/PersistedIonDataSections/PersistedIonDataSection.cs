using System;

namespace Cameca.CustomAnalysis.Utilities;

public record PersistedIonDataSection
{
	public IonDataSection? IonDataSection { get; init; }
	public Type? RuntimeType { get; init; }
	public string? FileFullName { get; init; }

	/// <summary>
	/// Create a minimal <see cref="PersistedIonDataSection" /> definition with only section name and runtime type.
	/// This is all that is required to save a section to disk with the <see cref="IIonDataSectionWriter" />.
	/// </summary>
	/// <param name="sectionName"></param>
	/// <param name="runtimeType">Type (unmanaged) used to read and write ion data. Should map to one instance per ion. Not necessarily the same as the APT section type if a struct or complex type.</param>
	/// <returns></returns>
	public static PersistedIonDataSection CreateDefinition(string sectionName, Type runtimeType)
	{
		return new PersistedIonDataSection
		{
			IonDataSection = new IonDataSection { Name = sectionName },
			RuntimeType = runtimeType,
		};
	}
}
