using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Cameca.CustomAnalysis.Utilities;

public class PeriodicTableListBox : ListBox
{
    public static readonly DependencyProperty SelectedElementsProperty = DependencyProperty.Register(
        nameof(SelectedElements),
        typeof(ObservableCollection<Element>),
        typeof(PeriodicTableListBox),
        new PropertyMetadata(new ObservableCollection<Element>(), OnSelectedElementsChanged));

    public ObservableCollection<Element> SelectedElements
    {
        get => (ObservableCollection<Element>)GetValue(SelectedElementsProperty);
        set => SetValue(SelectedElementsProperty, value);
    }

    public static readonly DependencyProperty DisabledElementsProperty = DependencyProperty.Register(
        nameof(DisabledElements),
        typeof(ObservableCollection<Element>),
        typeof(PeriodicTableListBox),
        new PropertyMetadata(new ObservableCollection<Element>(), OnDisabledElementsChanged));

    public ObservableCollection<Element> DisabledElements
    {
        get => (ObservableCollection<Element>)GetValue(DisabledElementsProperty);
        set => SetValue(DisabledElementsProperty, value);
    }

    private static void OnDisabledElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PeriodicTableListBox listBox)
        {
            if (e.OldValue is INotifyCollectionChanged oldValue)
            {
                oldValue.CollectionChanged -= listBox.DisabledElements_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newValue)
            {
                newValue.CollectionChanged += listBox.DisabledElements_CollectionChanged;
            }
            listBox.SyncIsEnabled();
        }
    }

    private static void OnSelectedElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PeriodicTableListBox listBox)
        {
            if (e.OldValue is INotifyCollectionChanged oldValue)
            {
                oldValue.CollectionChanged -= listBox.SelectedElements_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newValue)
            {
                newValue.CollectionChanged += listBox.SelectedElements_CollectionChanged;
            }
            listBox.SyncIsSelected();
        }
    }

    private void DisabledElements_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncIsEnabled();
    }

    private void SelectedElements_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncIsSelected();
    }

    private void SyncIsEnabled()
    {
        if (DisabledElements == null || ItemsSource == null)
            return;

        foreach (var item in ItemsSource)
        {
            if (item is PeriodicTableItemModel model)
            {
                var isEnabled = !DisabledElements.Contains(model.Element);
                if (isEnabled)
                {
                    model.IsEnabled = true;
                }
                else
                {
                    model.IsSelected = false;
                    model.IsEnabled = false;
                }
            }
        }
    }

    private void SyncIsSelected()
    {
        if (SelectedElements == null || ItemsSource == null)
            return;

        foreach (var item in ItemsSource)
        {
            if (item is PeriodicTableItemModel model)
            {
                model.IsSelected = SelectedElements.Contains(model.Element);
            }
        }
    }

    // Native ListBox event called when selection changed
    // Update the bound SelectedElementProperty by building list from selection state
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);

        if (SelectedElements == null)
            return;

        foreach (PeriodicTableItemModel item in e.RemovedItems)
        {
            SelectedElements.Remove(item.Element);
        }

        foreach (PeriodicTableItemModel item in e.AddedItems)
        {
            if (!SelectedElements.Contains(item.Element))
            {
                SelectedElements.Add(item.Element);
            }
        }
    }


    public PeriodicTableListBox()
    {
        Resources = new ResourceDictionary
        {
            ["DefaultItemBorderStyle"] = new Style(typeof(Border))
            {
                Setters =
                {
                    new Setter(BorderBrushProperty, new SolidColorBrush(Colors.Black)),
                    new Setter(BorderThicknessProperty, new Thickness(2)),
                    new Setter(MarginProperty, new Thickness(2)),
                    new Setter(PaddingProperty, new Thickness(4)),
                    new Setter(VerticalAlignmentProperty, VerticalAlignment.Stretch),
                    new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Center),
                    new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                    new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                }
            },
            ["DefaultItemGridStyle"] = new Style(typeof(Grid))
            {
                Setters =
                    {
                        new Setter(VerticalAlignmentProperty, VerticalAlignment.Stretch),
                        new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Center),
                        new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                        new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                    }
            },
            ["ItemContainerStyleBase"] = BuildItemContainerStyle(),
        };
        ItemsSource = Enum.GetValues<Element>().Select(e => new PeriodicTableItemModel(e)).ToList();
        Template = BuildControlTemplate();
        ItemContainerStyle = (Style)Resources["ItemContainerStyleBase"];
        ItemTemplate = BuildDefaultDataTemplate();
    }

    private static readonly DataTemplate DefaultItemInnerTemplate = BuildDefaultItemInnerTemplate();

    public static readonly DependencyProperty ItemInnerTemplateProperty = DependencyProperty.Register(
        nameof(ItemInnerTemplate),
        typeof(DataTemplate),
        typeof(PeriodicTableListBox),
        new FrameworkPropertyMetadata(DefaultItemInnerTemplate));

    public DataTemplate ItemInnerTemplate
    {
        get => (DataTemplate)GetValue(ItemInnerTemplateProperty);
        set => SetValue(ItemInnerTemplateProperty, value);
    }

    private static ControlTemplate BuildControlTemplate()
    {
        var grid = new FrameworkElementFactory(typeof(Grid));
        for (int row = 0; row < PeriodicTableLayoutConsts.RowsCount; row++)
        {
            var rowFact = new FrameworkElementFactory(typeof(RowDefinition));
            rowFact.SetValue(RowDefinition.HeightProperty, row == 7 ? new GridLength(PeriodicTableLayoutConsts.FreeSpaceHeight) : GridLength.Auto);
            grid.AppendChild(rowFact);
        }
        for (int col = 0; col < PeriodicTableLayoutConsts.ColumnsCount; col++)
        {
            var colFact = new FrameworkElementFactory(typeof(ColumnDefinition));
            colFact.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
            grid.AppendChild(colFact);
        }
        grid.SetValue(Panel.IsItemsHostProperty, true);
        var template = new ControlTemplate(typeof(ListBox))
        {
            VisualTree = grid,
        };
        template.Seal();
        return template;
    }

    private static DataTemplate BuildDefaultItemInnerTemplate()
    {
        var grid = new FrameworkElementFactory(typeof(Grid));
        grid.SetValue(WidthProperty, 28d);
        grid.SetValue(HeightProperty, 28d);
        var textBlock = new FrameworkElementFactory(typeof(TextBlock));
        textBlock.SetValue(FontSizeProperty, 14d);
        textBlock.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        textBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(PeriodicTableItemModel.Element)) { Mode=BindingMode.OneTime });
        grid.AppendChild(textBlock);
        var template = new DataTemplate
        {
            VisualTree = grid,
        };
        template.Seal();
        return template;
    }
    private static Style BuildItemContainerStyle()
    {
        var rowBinding = new Binding
        {
            Converter = new RowConverter(),
        };
        var colBinding = new Binding
        {
            Converter = new ColConverter(),
        };
        return new Style(typeof(ListBoxItem))
        {
            Setters =
            {
                new Setter(PaddingProperty, new Thickness(0)),
                new Setter(MarginProperty, new Thickness(0)),
                new Setter(BorderThicknessProperty, new Thickness(0)),
                new Setter(Grid.RowProperty, rowBinding),
                new Setter(Grid.ColumnProperty, colBinding),
                new Setter(IsSelectedProperty, new Binding(nameof(PeriodicTableItemModel.IsSelected)) { Mode=BindingMode.TwoWay }),
                new Setter(IsEnabledProperty, new Binding(nameof(PeriodicTableItemModel.IsEnabled)) { Mode=BindingMode.TwoWay }),
            },
            //Triggers =
            //{
            //    new Trigger
            //    {
            //        Property = ListBoxItem.IsSelectedProperty,
            //        Value = true,
            //        Setters =
            //        {
            //            new Setter(BackgroundProperty, new SolidColorBrush(Colors.DodgerBlue)),
            //        }
            //    },
            //},
        };
    }
    private DataTemplate BuildDefaultDataTemplate()
    {
        var border = new FrameworkElementFactory(typeof(Border));
        var grid = new FrameworkElementFactory(typeof(Grid));
        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
        contentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding(nameof(ContentPresenter.Content)) { RelativeSource = RelativeSource.TemplatedParent });
        contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding(nameof(ItemInnerTemplate))
        {
            RelativeSource = new RelativeSource
            {
                Mode = RelativeSourceMode.FindAncestor,
                AncestorType = typeof(PeriodicTableListBox),
            },
        });
        grid.SetValue(StyleProperty, FindResource("DefaultItemGridStyle"));
        border.SetValue(StyleProperty, FindResource("DefaultItemBorderStyle"));
        grid.AppendChild(contentPresenter);
        border.AppendChild(grid);
        var template = new DataTemplate()
        {
            VisualTree = border,
        };
        template.Seal();
        return template;
    }
}

internal abstract class BaseListBoxItemConverter : IValueConverter
{
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    protected static bool TryGetValidElement(object value, out Element element)
    {
        if (value is PeriodicTableItemModel { Element: Element e} && IsValidElement(e))
        {
            element = e;
            return true;
        }
        element = default;
        return false;
    }

    private static bool IsValidElement(Element element)
    {
        return element >= MinElement && element <= MaxElement;
    }

    private static readonly Element MinElement = Enum.GetValues<Element>().Min();
    private static readonly Element MaxElement = Enum.GetValues<Element>().Max();
}

internal class RowConverter : BaseListBoxItemConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (TryGetValidElement(value, out Element element) && PeriodicTableLayoutConsts.GridPositions.TryGetValue(element, out var rc))
        {
            return rc.row;
        }
        return DependencyProperty.UnsetValue;
    }
}

internal class ColConverter : BaseListBoxItemConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (TryGetValidElement(value, out Element element) && PeriodicTableLayoutConsts.GridPositions.TryGetValue(element, out var rc))
        {
            return rc.column;
        }
        return DependencyProperty.UnsetValue;
    }
}
