using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Cameca.CustomAnalysis.Utilities.Controls;

public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
{
	public bool Invert { get; set; } = false;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool boolValue)
		{
			if (!Invert)
			{
				return boolValue ? Visibility.Visible : Visibility.Collapsed;
			}
			else
			{
				return boolValue ? Visibility.Collapsed : Visibility.Visible;
			}
		}
		return DependencyProperty.UnsetValue;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is Visibility visibility)
		{
			if (visibility == Visibility.Visible)
			{
				return !Invert;
			}
			else if (visibility == Visibility.Collapsed)
			{
				return Invert;
			}
		}
		return DependencyProperty.UnsetValue;
	}

	public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
