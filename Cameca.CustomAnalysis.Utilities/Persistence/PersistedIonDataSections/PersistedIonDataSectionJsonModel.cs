namespace Cameca.CustomAnalysis.Utilities;

public record PersistedIonDataSectionJsonModel
{
	public IonDataSectionJsonModel? IonDataSection { get; init; }
	public string? RuntimeTypeFullName { get; init; }
	public string? FileFullName { get; init; }
}
