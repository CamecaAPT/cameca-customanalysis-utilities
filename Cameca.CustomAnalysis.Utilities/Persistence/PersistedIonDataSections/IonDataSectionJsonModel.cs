namespace Cameca.CustomAnalysis.Utilities;

public record IonDataSectionJsonModel
{
	public string? Name { get; init; }
	public string? TypeFullName { get; init; }
	public string Unit { get; init; } = "";
	public int ValuesPerRecord { get; init; } = 1;
}
