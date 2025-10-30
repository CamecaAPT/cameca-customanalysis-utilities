namespace Cameca.CustomAnalysis.Utilities.Segmentation;

/// <summary>
/// Prompt action options for when a node is deleted.
/// Extends a simple boolean option with an options to only show a prompt if the node to be deleted has any children itself.
/// </summary>
public enum DeleteChildPrompt
{
	/// <summary>
	/// Always display the delete confirmation prompt
	/// </summary>
	Always = 0,

	/// <summary>
	/// Never display the delete confirmation prompt
	/// </summary>
	Never = 1,

	/// <summary>
	/// Display the delete confirmation prompt only if the node to be deleted has any children
	/// </summary>
	IfNotEmpty = 2,
}
