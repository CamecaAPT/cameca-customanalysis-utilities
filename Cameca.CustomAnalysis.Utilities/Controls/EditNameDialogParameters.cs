using System;

namespace Cameca.CustomAnalysis.Utilities.Controls;

public sealed record EditNameDialogParameters
{
	public string? Name { get; init; } = null;
	public string? Title { get; init; } = null;
	public Predicate<string>? Validate { get; init; } = null;
	public string? OkButtonCaption { get; init; } = null;
	public string? CancelButtonCaption { get; init; } = null;
	public string? NameLabel { get; init; } = null;

	public EditNameDialogParameters() { }

	public EditNameDialogParameters(string name, Predicate<string>? validate = null, string? title = null) : this()
	{
		Name = name;
		Validate = validate;
		Title = title;
	}
}
