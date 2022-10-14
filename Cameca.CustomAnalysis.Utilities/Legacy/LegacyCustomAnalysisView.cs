using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Cameca.Extensions.Controls;
using Prism.Mvvm;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class LegacyCustomAnalysisView : UserControl
{
	private const string TabItemTitlePropertyName = "Title";
	private const string RunButtonContent = "Run";

	// nameof(LegacyCustomAnalysisViewModelBase.{property}) inconvenient due to strict generic constraints
	private const string LegacyCustomAnalysisViewModelRunCommand = "RunCommand";
	private const string LegacyCustomAnalysisViewModelOptions = "Options";
	private const string LegacyCustomAnalysisViewModelTabs = "Tabs";
	private const string LegacyCustomAnalysisViewModelSelectedTab = "SelectedTab";

	public LegacyCustomAnalysisView()
	{
		Resources = BuildResources();
	}

	private ResourceDictionary BuildResources() => new ResourceDictionary
	{
		[typeof(TabItem)] = new Style(typeof(TabItem))
		{
			Setters =
			{
				new Setter(HeaderedContentControl.HeaderProperty, new Binding(TabItemTitlePropertyName)),
			},
		},
	};

	private object BuildContent() => new Grid
	{
		ColumnDefinitions =
		{
			new ColumnDefinition { Width = new GridLength(200), },
			new ColumnDefinition { Width = GridLength.Auto, },
			new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), },
		},
		Children =
		{
			new Grid
			{
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star), },
				},
				Children =
				{
					new Button { Content = RunButtonContent }
						.SetGridRow(0)
						.SetBindingEx(ButtonBase.CommandProperty, LegacyCustomAnalysisViewModelRunCommand),
					new PropertyGrid
					{
						ShowCategories = false,
						ShowEditorButtons = false,
						ShowToolPanel = false,
						ShowSearchBox = false,
						ExpandButtonsVisibility = Visibility.Visible,
						ExpandCategoriesWhenSelectedObjectChanged = true,
					}.SetGridRow(1)
						.SetBindingEx(PropertyGrid.SelectedObjectProperty, LegacyCustomAnalysisViewModelOptions),
				},
			}.SetGridColumn(0),
			new GridSplitter
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Stretch,
				ShowsPreview = true,
				Width = 5,
			}.SetGridColumn(1),
			new TabControl
			{
				Resources = new ResourceDictionary
				{
					[typeof(Chart3DTabViewModel)] = new DataTemplate(typeof(Chart3DTabViewModel))
					{
						VisualTree = new FrameworkElementFactory(typeof(Chart3D))
							.SetBindingFefExt(Chart3D.DataSourceProperty, new Binding(nameof(Chart3DTabViewModel.RenderData))),
					},
					[typeof(Chart2DTabViewModel)] = new DataTemplate(typeof(Chart2DTabViewModel))
					{
						VisualTree = new FrameworkElementFactory(typeof(Chart2D))
							.SetBindingFefExt(Chart2D.DataSourceProperty, new Binding(nameof(Chart2DTabViewModel.RenderData)))
							.SetBindingFefExt(Chart2D.AxisXLabelProperty, new Binding(nameof(Chart2DTabViewModel.XAxis)))
							.SetBindingFefExt(Chart2D.AxisYLabelProperty, new Binding(nameof(Chart2DTabViewModel.YAxis))),
					},
					[typeof(Histogram2DTabViewModel)] = new DataTemplate(typeof(Histogram2DTabViewModel))
					{
						VisualTree = new FrameworkElementFactory(typeof(Histogram2D))
							.SetBindingFefExt(Chart2D.DataSourceProperty, new Binding(nameof(Histogram2DTabViewModel.RenderData)))
							.SetBindingFefExt(Chart2D.AxisXLabelProperty, new Binding(nameof(Histogram2DTabViewModel.XAxis)))
							.SetBindingFefExt(Chart2D.AxisYLabelProperty, new Binding(nameof(Histogram2DTabViewModel.YAxis))),
					},
					[typeof(TableTabViewModel)] = new DataTemplate(typeof(TableTabViewModel))
					{
						VisualTree = new FrameworkElementFactory(typeof(Table))
							.SetBindingFefExt(Table.ItemsSourceProperty, new Binding(nameof(TableTabViewModel.Items)))
							.SetValueFefExt(Table.AutoWidthProperty, true),
					},
					[typeof(TextTabViewModel)] = new DataTemplate(typeof(TextTabViewModel))
					{
						VisualTree = new FrameworkElementFactory(typeof(TextBlock))
							.SetBindingFefExt(TextBlock.TextProperty, new Binding(nameof(TextTabViewModel.Content)) { Mode = BindingMode.OneWay })
							.SetValueFefExt(VerticalAlignmentProperty, VerticalAlignment.Top)
							.SetValueFefExt(HorizontalAlignmentProperty, HorizontalAlignment.Left)
							.SetValueFefExt(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow),
					},
				},
			}.SetGridColumn(2)
				.SetBindingEx(ItemsControl.ItemsSourceProperty, LegacyCustomAnalysisViewModelTabs)
				.SetBindingEx(Selector.SelectedItemProperty, LegacyCustomAnalysisViewModelSelectedTab),
		}
	};
}
