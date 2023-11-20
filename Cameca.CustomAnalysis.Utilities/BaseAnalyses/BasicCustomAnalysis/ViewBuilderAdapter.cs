using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Mvvm;

namespace Cameca.CustomAnalysis.Utilities;

public class ViewBuilderAdapter : BindableBase
{
	public ObservableCollection<object> Tabs { get; } = new();

	private object? _selectedTab;
	public object? SelectedTab
	{
		get => _selectedTab;
		set => SetProperty(ref _selectedTab, value);
	}

	public ICommand UpdateCommand => _vm.UpdateCommand;

	private readonly IBasicCustomAnalysisViewModel _vm;

	public ViewBuilderAdapter(IBasicCustomAnalysisViewModel vm)
	{
		_vm = vm;
	}
}
