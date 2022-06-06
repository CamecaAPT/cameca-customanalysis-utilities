using System.Windows;
using System.Windows.Input;

namespace Cameca.CustomAnalysis.Utilities.Controls;
/// <summary>
/// Interaction logic for NameDialogView.xaml
/// </summary>
internal partial class EditNameDialogView
{
	public EditNameDialogView()
	{
		InitializeComponent();
	}

	private void EditNameDialogView_OnLoaded(object sender, RoutedEventArgs e)
	{
		Keyboard.Focus(NameTextBox);
		this.Loaded -= EditNameDialogView_OnLoaded;
	}
}
