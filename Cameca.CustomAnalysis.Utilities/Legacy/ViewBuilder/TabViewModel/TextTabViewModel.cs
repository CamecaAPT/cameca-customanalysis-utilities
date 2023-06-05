using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class TextTabViewModel
{
	public string Title { get; }

	public string Content { get; }

	public ScrollBarVisibility HorizontalScrollBarVisibility { get; }

	public ScrollBarVisibility VerticalScrollBarVisibility { get; }

	public TextWrapping TextWrapping { get; }

	public FontFamily FontFamily { get; }

	public double FontSize { get; }

	public TextTabViewModel(string title, string content, TextBoxOptions options)
	{
		Title = title;
		Content = content;
		HorizontalScrollBarVisibility = options.HorizontalScrollBarVisibility;
		VerticalScrollBarVisibility = options.VerticalScrollBarVisibility;
		TextWrapping = options.TextWrapping;
		FontFamily = options.FontFamily;
		FontSize = options.FontSize;
	}
}
