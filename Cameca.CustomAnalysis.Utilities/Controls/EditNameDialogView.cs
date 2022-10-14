using Prism.Services.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Cameca.CustomAnalysis.Utilities.Controls;

internal class EditNameDialogView : UserControl
{
	private const string NameTextBox = "NameTextBox";
	private const string SharedButtonGroup = "SharedButtonGroup";

	public EditNameDialogView()
	{
		Loaded += EditNameDialogView_OnLoaded;
		Dialog.SetWindowStyle(this, CreateWindowStyle());
		Dialog.SetWindowStartupLocation(this, WindowStartupLocation.CenterScreen);
		Content = BuildContent();
	}

	public Style CreateWindowStyle() => new Style(typeof(Window))
	{
		Setters =
		{
			new Setter(Window.TitleProperty, new Binding(nameof(EditNameDialogViewModel.Title))),
			new Setter(Window.ResizeModeProperty, ResizeMode.NoResize),
			new Setter(Window.ShowInTaskbarProperty, false),
			new Setter(Window.SizeToContentProperty, SizeToContent.WidthAndHeight),
			new Setter(Window.WindowStyleProperty, WindowStyle.ToolWindow),
		},
	};

	public object BuildContent() => new Grid
	{
		RowDefinitions =
		{
			new RowDefinition { },
			new RowDefinition { Height = GridLength.Auto, },
		},
		Children =
		{
			new Grid
			{
				Margin = new Thickness(5),
				ColumnDefinitions =
				{
					new ColumnDefinition { Width = GridLength.Auto, },
					new ColumnDefinition
					{
						MinWidth = 150,
						Width = new GridLength(1, GridUnitType.Star),
					},
				},
				Children =
				{
					new Label
						{
							Margin = new Thickness(0, 0, 5, 0),
						}.SetGridColumn(0)
						.SetBindingEx(ContentProperty, nameof(EditNameDialogViewModel.NameLabel)),
					new TextBox
						{
							Name = NameTextBox,
							AcceptsReturn = false,
						}.SetGridColumn(1)
						.SetBindingEx(TextBox.TextProperty, nameof(EditNameDialogViewModel.Name)),
				},
			}.SetGridRow(0),

			new Grid
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				ColumnDefinitions =
				{
					new ColumnDefinition { SharedSizeGroup = SharedButtonGroup, },
					new ColumnDefinition { SharedSizeGroup = SharedButtonGroup, },
				},
				Children =
				{
					new Button
						{
							IsDefault = true,
							Margin = new Thickness(5),
						}.SetGridColumn(0)
						.SetBindingEx(ContentProperty, nameof(EditNameDialogViewModel.OkButtonCaption))
						.SetBindingEx(ButtonBase.CommandProperty, nameof(EditNameDialogViewModel.OkCommand)),
					new Button
						{
							IsCancel = true,
							Margin = new Thickness(5),
						}.SetGridColumn(1)
						.SetBindingEx(ContentProperty, nameof(EditNameDialogViewModel.CancelButtonCaption))
						.SetBindingEx(ButtonBase.CommandProperty, nameof(EditNameDialogViewModel.CancelCommand)),
				}
			}.SetGridRow(1)
			.SetGridIsSharedSizeScope(true),
		},
	};

	private void EditNameDialogView_OnLoaded(object sender, RoutedEventArgs e)
	{
		if (FindByName(sender, NameTextBox) is TextBox textBox)
		{
			Keyboard.Focus(textBox);
			textBox.Select(0, textBox.Text.Length);
		}
		Loaded -= EditNameDialogView_OnLoaded;
	}

	public static object? FindByName(object? root, string name)
	{
		return root is FrameworkElement frameworkElement
			? frameworkElement.FindName(name)
			: default;
	}
}
