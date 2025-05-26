using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;
using System.Globalization;

namespace Cameca.CustomAnalysis.Utilities.Controls;

public class ButtonOverlayControl : ContentControl
{
	public const string ButtonContentDefault = "Update";
	public const string CancelButtonContentDefault = "Cancel";

	public static readonly DependencyProperty OverlayVisibilityProperty = DependencyProperty.Register(
		nameof(OverlayVisibility), typeof(Visibility), typeof(ButtonOverlayControl), new FrameworkPropertyMetadata(default(Visibility)));

	public Visibility OverlayVisibility
	{
		get => (Visibility)GetValue(OverlayVisibilityProperty);
		set => SetValue(OverlayVisibilityProperty, value);
	}

	public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register(
		nameof(ButtonCommand), typeof(ICommand), typeof(ButtonOverlayControl), new PropertyMetadata(default(ICommand)));

	public ICommand ButtonCommand
	{
		get => (ICommand)GetValue(ButtonCommandProperty);
		set => SetValue(ButtonCommandProperty, value);
	}

	public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(
		nameof(ButtonContent), typeof(object), typeof(ButtonOverlayControl), new FrameworkPropertyMetadata(ButtonContentDefault));

	public object? ButtonContent
	{
		get => (object?)GetValue(ButtonContentProperty);
		set => SetValue(ButtonContentProperty, value);
	}

	public static readonly DependencyProperty CancelButtonCommandProperty = DependencyProperty.Register(
		nameof(CancelButtonCommand), typeof(ICommand), typeof(ButtonOverlayControl), new PropertyMetadata(default(ICommand), propertyChangedCallback: OnPropertyChanged));

	private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine(e.NewValue);
	}

	public ICommand CancelButtonCommand
	{
		get => (ICommand)GetValue(CancelButtonCommandProperty);
		set => SetValue(CancelButtonCommandProperty, value);
	}

	public static readonly DependencyProperty CancelButtonContentProperty = DependencyProperty.Register(
		nameof(CancelButtonContent), typeof(object), typeof(ButtonOverlayControl), new FrameworkPropertyMetadata(CancelButtonContentDefault));

	public object? CancelButtonContent
	{
		get => (object?)GetValue(CancelButtonContentProperty);
		set => SetValue(CancelButtonContentProperty, value);
	}

	public ButtonOverlayControl()
	{
		Template = BuildControlTemplate();
	}

	private static ControlTemplate BuildControlTemplate()
	{
		var multiConverter = new OptionalCommandVisiblityConverter();
		var visibilityConverter = new BooleanToVisibilityConverter();
		return new ControlTemplate(typeof(ButtonOverlayControl))
		{
			VisualTree = new FrameworkElementFactory(typeof(Grid))
				.AppendChildFefExt(new FrameworkElementFactory(typeof(ContentPresenter))
					.SetValueFefExt(ContentPresenter.ContentProperty, new TemplateBindingExtension(ContentProperty)))
				.AppendChildFefExt(new FrameworkElementFactory(typeof(Grid))
					.SetBindingFefExt(VisibilityProperty, new Binding
					{
						RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ButtonOverlayControl), 1),
						Path = new PropertyPath(nameof(OverlayVisibility)),
					})
					.SetValueFefExt(Panel.ZIndexProperty, int.MaxValue)
					.AppendChildFefExt(new FrameworkElementFactory(typeof(Grid))
						.SetValueFefExt(OpacityProperty, 0.8d)
						.SetValueFefExt(Panel.BackgroundProperty, new SolidColorBrush(Colors.White)))
					.AppendChildFefExt(new FrameworkElementFactory(typeof(Button))
						.SetValueFefExt(ContentProperty, new TemplateBindingExtension(ButtonContentProperty))
						.SetValueFefExt(VerticalAlignmentProperty, VerticalAlignment.Center)
						.SetValueFefExt(HorizontalAlignmentProperty, HorizontalAlignment.Center)
						.SetValueFefExt(PaddingProperty, new Thickness(10d))
						.SetBindingFefExt(VisibilityProperty, new MultiBinding
						{
							Converter = multiConverter,
							Bindings =
							{
								new Binding
								{
									RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ButtonOverlayControl), 1),
									Path = new PropertyPath(nameof(CancelButtonCommand)),
								},
								new Binding
								{
									RelativeSource = new RelativeSource(RelativeSourceMode.Self),
									Path = new PropertyPath(nameof(IsEnabled)),
								},
							},
						})
						.SetBindingFefExt(ButtonBase.CommandProperty, new Binding
						{
							RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ButtonOverlayControl), 1),
							Path = new PropertyPath(nameof(ButtonCommand)),
						}))
					.AppendChildFefExt(new FrameworkElementFactory(typeof(Button))
						.SetValueFefExt(ContentProperty, new TemplateBindingExtension(CancelButtonContentProperty))
						.SetValueFefExt(VerticalAlignmentProperty, VerticalAlignment.Center)
						.SetValueFefExt(HorizontalAlignmentProperty, HorizontalAlignment.Center)
						.SetValueFefExt(PaddingProperty, new Thickness(10d))
						.SetBindingFefExt(VisibilityProperty, new MultiBinding
						{
							Converter = multiConverter,
							Bindings =
							{
								new Binding
								{
									RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ButtonOverlayControl), 1),
									Path = new PropertyPath(nameof(CancelButtonCommand)),
								},
								new Binding
								{
									RelativeSource = new RelativeSource(RelativeSourceMode.Self),
									Path = new PropertyPath(nameof(IsEnabled)),
								},
							},
							FallbackValue = Visibility.Collapsed,
						})
						.SetBindingFefExt(ButtonBase.CommandProperty, new Binding
						{
							RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ButtonOverlayControl), 1),
							Path = new PropertyPath(nameof(CancelButtonCommand)),
						}))
					),
		}.SealFrameworkTemplate();
	}
}

internal class OptionalCommandVisiblityConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[0] is ICommand command)
		{
			return values[1] is bool enabled && enabled ? Visibility.Visible : Visibility.Collapsed;
		}
		return DependencyProperty.UnsetValue;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
