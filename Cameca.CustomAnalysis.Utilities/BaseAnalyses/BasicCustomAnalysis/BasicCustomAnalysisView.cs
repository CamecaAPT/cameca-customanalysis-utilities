using Cameca.CustomAnalysis.Utilities.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cameca.CustomAnalysis.Utilities;

public class BasicCustomAnalysisView : UserControl
{
	public BasicCustomAnalysisView()
	{
		Resources = BuildResources();
		Content = WrapContent();
	}

	private ResourceDictionary BuildResources() => new ResourceDictionary
	{
		["BooleanToVisibilityConverter"] = new BooleanToVisibilityConverter(),
	};

	private object WrapContent() => new ButtonOverlayControl
	{
		ButtonContent = "Update",
		Content = BuildContent(),
	}
		.SetBindingEx(ButtonOverlayControl.ButtonCommandProperty, "UpdateCommand")
		.SetBindingEx(ButtonOverlayControl.OverlayVisibilityProperty, new Binding("RequiresUpdate")
		{
			Converter = Resources["BooleanToVisibilityConverter"] as IValueConverter,
		});

	private object? BuildContent() => new ContentControl().SetBindingEx(ContentProperty, "RootContent");
}
