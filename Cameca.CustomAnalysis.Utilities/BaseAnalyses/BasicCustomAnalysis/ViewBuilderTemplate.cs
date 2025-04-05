using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Cameca.CustomAnalysis.Utilities.Legacy;
using Cameca.Extensions.Controls;

namespace Cameca.CustomAnalysis.Utilities;

public class ViewBuilderTemplate : UserControl
{
	private const string TabItemTitlePropertyName = "Title";
	private const string RunButtonContent = "Update";

	// nameof(LegacyCustomAnalysisViewModelBase.{property}) inconvenient due to strict generic constraints
	private const string LegacyCustomAnalysisViewModelRunCommand = "UpdateCommand";
	//private const string LegacyCustomAnalysisViewModelOptions = "Options";
	private const string LegacyCustomAnalysisViewModelTabs = "Tabs";
	private const string LegacyCustomAnalysisViewModelSelectedTab = "SelectedTab";

	public ViewBuilderTemplate()
	{
		Resources = BuildResources();
		Content = BuildContent();
	}

	private static ResourceDictionary BuildResources() => new ResourceDictionary
	{
		[typeof(TabItem)] = new Style(typeof(TabItem))
		{
			Setters =
			{
				new Setter(HeaderedContentControl.HeaderProperty, new Binding(TabItemTitlePropertyName)),
			},
		},
	};

	private static object BuildContent() => new TabControl
	{
		Resources = new ResourceDictionary
		{
			[new DataTemplateKey(typeof(Chart3DTabViewModel))] = new DataTemplate(typeof(Chart3DTabViewModel))
			{
				VisualTree = new FrameworkElementFactory(typeof(Chart3D))
								.SetBindingFefExt(Chart3D.DataSourceProperty, new Binding(nameof(Chart3DTabViewModel.RenderData))),
			}.SealFrameworkTemplate(),
			[new DataTemplateKey(typeof(Chart2DTabViewModel))] = new DataTemplate(typeof(Chart2DTabViewModel))
			{
				VisualTree = new FrameworkElementFactory(typeof(Chart2D))
								.SetBindingFefExt(Chart2D.DataSourceProperty, new Binding(nameof(Chart2DTabViewModel.RenderData)))
								.SetBindingFefExt(Chart2D.AxisXLabelProperty, new Binding(nameof(Chart2DTabViewModel.XAxis)))
								.SetBindingFefExt(Chart2D.AxisYLabelProperty, new Binding(nameof(Chart2DTabViewModel.YAxis))),
			}.SealFrameworkTemplate(),
			[new DataTemplateKey(typeof(Histogram2DTabViewModel))] = new DataTemplate(typeof(Histogram2DTabViewModel))
			{
				VisualTree = new FrameworkElementFactory(typeof(Histogram2D))
								.SetBindingFefExt(Chart2D.DataSourceProperty, new Binding(nameof(Histogram2DTabViewModel.RenderData)))
								.SetBindingFefExt(Chart2D.AxisXLabelProperty, new Binding(nameof(Histogram2DTabViewModel.XAxis)))
								.SetBindingFefExt(Chart2D.AxisYLabelProperty, new Binding(nameof(Histogram2DTabViewModel.YAxis))),
			}.SealFrameworkTemplate(),
			[new DataTemplateKey(typeof(TableTabViewModel))] = new DataTemplate(typeof(TableTabViewModel))
			{
				VisualTree = new FrameworkElementFactory(typeof(Table))
								.SetBindingFefExt(Table.ItemsSourceProperty, new Binding(nameof(TableTabViewModel.Items)))
								.SetValueFefExt(Table.AutoWidthProperty, true),
			}.SealFrameworkTemplate(),
			[new DataTemplateKey(typeof(TextTabViewModel))] = new DataTemplate(typeof(TextTabViewModel))
			{
				VisualTree = new FrameworkElementFactory(typeof(TextBox))
								.SetBindingFefExt(TextBox.TextProperty, new Binding(nameof(TextTabViewModel.Content)) { Mode = BindingMode.OneTime })
								.SetValueFefExt(VerticalAlignmentProperty, VerticalAlignment.Stretch)
								.SetValueFefExt(HorizontalAlignmentProperty, HorizontalAlignment.Stretch)
								.SetValueFefExt(TextBoxBase.IsReadOnlyProperty, true)
								.SetValueFefExt(BackgroundProperty, new SolidColorBrush(Colors.Transparent))
								.SetBindingFefExt(TextBoxBase.HorizontalScrollBarVisibilityProperty, new Binding(nameof(TextTabViewModel.HorizontalScrollBarVisibility)) { Mode = BindingMode.OneTime })
								.SetBindingFefExt(TextBoxBase.VerticalScrollBarVisibilityProperty, new Binding(nameof(TextTabViewModel.VerticalScrollBarVisibility)) { Mode = BindingMode.OneTime })
								.SetBindingFefExt(TextBox.TextWrappingProperty, new Binding(nameof(TextTabViewModel.TextWrapping)) { Mode = BindingMode.OneTime })
								.SetBindingFefExt(FontFamilyProperty, new Binding(nameof(TextTabViewModel.FontFamily)) { Mode = BindingMode.OneTime })
								.SetBindingFefExt(FontSizeProperty, new Binding(nameof(TextTabViewModel.FontSize)) { Mode = BindingMode.OneTime }),
			}.SealFrameworkTemplate(),
		},
	}
	.SetBindingEx(ItemsControl.ItemsSourceProperty, LegacyCustomAnalysisViewModelTabs)
	.SetBindingEx(Selector.SelectedItemProperty, LegacyCustomAnalysisViewModelSelectedTab);
}
