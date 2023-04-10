using System;

namespace Cameca.CustomAnalysis.Utilities;

public record IonDataSection
{
	public string? Name { get; init; }
	public Type? Type { get; init; }
	public string Unit { get; init; } = "";
	public int ValuesPerRecord { get; init; } = 1;
}
