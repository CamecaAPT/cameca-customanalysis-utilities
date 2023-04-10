using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Cameca.CustomAnalysis.Utilities.Controls;

public class ButtonOverlayControl : ContentControl
{
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
		nameof(ButtonContent), typeof(object), typeof(ButtonOverlayControl), new FrameworkPropertyMetadata(default(object?)));

	public object? ButtonContent
	{
		get => (object?)GetValue(ButtonContentProperty);
		set => SetValue(ButtonContentProperty, value);
	}

	public ButtonOverlayControl()
	{
		Template = BuildControlTemplate(this);
	}

	private static ControlTemplate BuildControlTemplate(object bindingSource)
	{
		return new ControlTemplate(typeof(ButtonOverlayControl))
		{
			VisualTree = new FrameworkElementFactory(typeof(Grid))
				.AppendChildFefExt(new FrameworkElementFactory(typeof(ContentPresenter))
					.SetValueFefExt(ContentPresenter.ContentProperty, new TemplateBindingExtension(ButtonOverlayControl.ContentProperty)))
				.AppendChildFefExt(new FrameworkElementFactory(typeof(Grid))
					.SetBindingFefExt(Grid.VisibilityProperty, new Binding
					{
						Source = bindingSource,
						Path = new PropertyPath(nameof(OverlayVisibility)),
					})
					.SetValueFefExt(Panel.ZIndexProperty, int.MaxValue)
					.AppendChildFefExt(new FrameworkElementFactory(typeof(Grid))
						.SetValueFefExt(Grid.OpacityProperty, 0.8d)
						.SetValueFefExt(Panel.BackgroundProperty, new SolidColorBrush(Colors.White)))
					.AppendChildFefExt(new FrameworkElementFactory(typeof(Button))
						.SetValueFefExt(Button.ContentProperty, new TemplateBindingExtension(ButtonOverlayControl.ButtonContentProperty))
						.SetValueFefExt(Button.VerticalAlignmentProperty, VerticalAlignment.Center)
						.SetValueFefExt(Button.HorizontalAlignmentProperty, HorizontalAlignment.Center)
						.SetValueFefExt(Button.PaddingProperty, new Thickness(10d))
						.SetBindingFefExt(ButtonBase.CommandProperty, new Binding
						{
							Source = bindingSource,
							Path = new PropertyPath(nameof(ButtonCommand)),
						}))),
		}.SealFrameworkTemplate();
	}
}
