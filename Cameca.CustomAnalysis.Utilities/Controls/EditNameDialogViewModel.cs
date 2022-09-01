using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities.Controls;

internal class EditNameDialogViewModel : BindableBase, IDialogAware
{
	public string Title { get; private set; } = "Edit Name";

	private string _name = "";
	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value, () => _okCommand.RaiseCanExecuteChanged());
	}

	private string _nameLabel = "Name";
	public string NameLabel
	{
		get => _nameLabel;
		set => SetProperty(ref _nameLabel, value);
	}

	private Predicate<string>? _validator = null;

	private readonly DelegateCommand _okCommand;
	public ICommand OkCommand => _okCommand;

	public ICommand CancelCommand { get; }

	private string _okButtonCaption = "OK";
	public string OkButtonCaption
	{
		get => _okButtonCaption;
		set => SetProperty(ref _okButtonCaption, value);
	}

	private string _cancelButtonCaption = "Cancel";
	public string CancelButtonCaption
	{
		get => _cancelButtonCaption;
		set => SetProperty(ref _cancelButtonCaption, value);
	}

	public EditNameDialogViewModel()
	{
		_okCommand = new DelegateCommand(OnOkCommand, IsOkayCommandEnabled);
		CancelCommand = new DelegateCommand(OnCancelCommand);
	}

	private void OnOkCommand()
	{
		OnRequestClose(new DialogResult(ButtonResult.OK, new DialogParameters
		{
			{ EditNameDialog.NameParameter, Name },
		}));
	}

	private bool IsOkayCommandEnabled() => _validator?.Invoke(Name) ?? true;

	private void OnCancelCommand() => OnRequestClose(new DialogResult(ButtonResult.Cancel));

	public bool CanCloseDialog() => true;

	public void OnDialogClosed() { }

	public void OnDialogOpened(IDialogParameters parameters)
	{
		if (parameters.TryGetValue(EditNameDialog.NameParameter, out string name))
			Name = name;
		if (parameters.TryGetValue(EditNameDialog.ValidateParameter, out Predicate<string> validator))
			_validator = validator;
		if (parameters.TryGetValue(EditNameDialog.TitleParameter, out string title))
			Title = title;
		if (parameters.TryGetValue(EditNameDialog.OkButtonCaptionParameter, out string okButtonCaption))
			OkButtonCaption = okButtonCaption;
		if (parameters.TryGetValue(EditNameDialog.CancelButtonCaptionParameter, out string cancelButtonCaption))
			CancelButtonCaption = cancelButtonCaption;
		if (parameters.TryGetValue(EditNameDialog.NameLabelParameter, out string nameLabel))
			NameLabel = nameLabel;
	}

	public event Action<IDialogResult>? RequestClose;

	protected void OnRequestClose(IDialogResult dialogResult) => RequestClose?.Invoke(dialogResult);
}
