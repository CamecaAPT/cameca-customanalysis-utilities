namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class TextTabViewModel
{
	public string Title { get; }

	public string Content { get; }

	public TextTabViewModel(string title, string content)
	{
		Title = title;
		Content = content;
	}
}
