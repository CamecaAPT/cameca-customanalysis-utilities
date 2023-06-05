using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public class TextBoxOptions
{
	private const ScrollBarVisibility DefaultHorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
	private const ScrollBarVisibility DefaultVerticalScrollBarVisibility = ScrollBarVisibility.Auto;
	private const TextWrapping DefaultTextWrapping = TextWrapping.NoWrap;
	private static readonly FontFamily DefaultFontFamily = new("Segoe UI");
	private const double DefaultFontSize = 12d;

	public ScrollBarVisibility HorizontalScrollBarVisibility { get; init; } = DefaultHorizontalScrollBarVisibility;

	public ScrollBarVisibility VerticalScrollBarVisibility { get; init; } = DefaultVerticalScrollBarVisibility;

	public TextWrapping TextWrapping { get; init; } = DefaultTextWrapping;

	public FontFamily FontFamily { get; init; } = DefaultFontFamily;

	public double FontSize { get; init; } = DefaultFontSize;
}
