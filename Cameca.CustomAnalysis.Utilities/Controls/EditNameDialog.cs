using Prism.Services.Dialogs;

namespace Cameca.CustomAnalysis.Utilities.Controls;

public static class EditNameDialog
{
	internal const string EditNameDialogName = "Cameca.CustomAnalysis.Utilities.Controls.EditNameDialog";

	internal const string NameParameter = "Name";
	internal const string TitleParameter = "Title";
	internal const string ValidateParameter = "Validate";
	internal const string OkButtonCaptionParameter = "OkButtonCaption";
	internal const string CancelButtonCaptionParameter = "CancelButtonCaption";
	internal const string NameLabelParameter = "NameLabel";

	/// <summary>
	/// Create and show the edit name dialog window.
	/// </summary>
	/// <param name="dialogService"></param>
	/// <param name="dialogParameters"></param>
	/// <returns></returns>
	public static EditNameDialogResults ShowEditNameDialog(this IDialogService dialogService, EditNameDialogParameters? dialogParameters = null)
	{
		IDialogResult? dialogResults = null;
		void SetResultsCallback(IDialogResult results) => dialogResults = results;
		var parameters = new DialogParameters();
		if (dialogParameters is not null)
		{
			if (dialogParameters.Name is not null)
				parameters.Add(NameParameter, dialogParameters.Name);
			if (dialogParameters.Title is not null)
				parameters.Add(TitleParameter, dialogParameters.Title);
			if (dialogParameters.Validate is not null)
				parameters.Add(ValidateParameter, dialogParameters.Validate);
			if (dialogParameters.OkButtonCaption is not null)
				parameters.Add(OkButtonCaptionParameter, dialogParameters.OkButtonCaption);
			if (dialogParameters.CancelButtonCaption is not null)
				parameters.Add(CancelButtonCaptionParameter, dialogParameters.CancelButtonCaption);
			if (dialogParameters.NameLabel is not null)
				parameters.Add(NameLabelParameter, dialogParameters.NameLabel);
		}
		dialogService.ShowDialog(EditNameDialogName, parameters, SetResultsCallback);
		return dialogResults is not null
			? new EditNameDialogResults(dialogResults.Result, dialogResults.Parameters.GetValue<string>(NameParameter))
			: new EditNameDialogResults(ButtonResult.None, dialogParameters?.Name ?? "");
	}

	/// <summary>
	/// Create and show the edit name dialog window. Sets value to out parameter. Returns true only if OK button was selected.
	/// </summary>
	/// <param name="dialogService"></param>
	/// <param name="name"></param>
	/// <param name="dialogParameters"></param>
	/// <returns></returns>
	public static bool TryShowEditNameDialog(this IDialogService dialogService, out string name, EditNameDialogParameters? dialogParameters = null)
	{
		var results = dialogService.ShowEditNameDialog(dialogParameters);
		name = results.Name;
		return results.ButtonResult == ButtonResult.OK;
	}
}
