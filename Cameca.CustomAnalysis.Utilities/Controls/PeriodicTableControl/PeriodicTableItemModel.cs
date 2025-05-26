using CommunityToolkit.Mvvm.ComponentModel;

namespace Cameca.CustomAnalysis.Utilities;

public partial class PeriodicTableItemModel : ObservableObject
{
    public Element Element { get; }

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isEnabled = true;

    public PeriodicTableItemModel(Element element)
    {
        Element = element;
    }
}
