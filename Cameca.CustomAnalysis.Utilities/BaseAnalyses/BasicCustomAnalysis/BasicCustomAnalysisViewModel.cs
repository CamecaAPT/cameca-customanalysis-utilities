using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cameca.CustomAnalysis.Interface;
using Cameca.CustomAnalysis.Utilities.Legacy;
using CommunityToolkit.Mvvm.Input;

namespace Cameca.CustomAnalysis.Utilities;

public interface IBasicCustomAnalysisViewModel
{
	ICommand UpdateCommand { get; }
}

public abstract class BasicCustomAnalysisViewModel<TNode, TAnalysis, TProperties> : AnalysisViewModelBase<TNode>, IBasicCustomAnalysisViewModel
	where TNode : BasicCustomAnalysisNodeBase<TAnalysis, TProperties>
	where TAnalysis : IAnalysis<TProperties>
	where TProperties : INotifyPropertyChanged, new()
{
	public ICommand UpdateCommand { get; }

	private bool _requiresUpdate;
	public bool RequiresUpdate
	{
		get => _requiresUpdate;
		set => SetProperty(ref _requiresUpdate, value);
	}

	private object? _rootContent = null;
	public object? RootContent
	{
		get => _rootContent;
		set => SetProperty(ref _rootContent, value);
	}

	protected BasicCustomAnalysisViewModel(IAnalysisViewModelBaseServices services) : base(services)
	{
		UpdateCommand = new AsyncRelayCommand(OnUpdateCommand);
	}

	protected override void OnCreated(ViewModelCreatedEventArgs eventArgs)
	{
		base.OnCreated(eventArgs);
		if (eventArgs.Mode != ViewModelMode.Interactive) return;

		if (Node.DataState is { } dataStateInfo)
		{
			dataStateInfo.IsValid = false;
			RequiresUpdate = !dataStateInfo.IsValid || dataStateInfo.IsErrorState;
			dataStateInfo.PropertyChanged += DataStateOnPropertyChanged;
		}

		UpdateCommand.Execute(null);
	}

	private void DataStateOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		// Don't directly link data state to requires update, analysis update method may want
		// to require additional work and should be the final source to mark no update as requires.
		// Here only mark update as required if data data is change to invalid.
		if (Node.DataState is { } dataStateInfo)
		{
			RequiresUpdate = !dataStateInfo.IsValid || dataStateInfo.IsErrorState;
		}
	}

	private ViewBuilderAdapter? viewBuilderAdapter = null;
	private ViewBuilderTemplate? viewBuilderTemplate = null;

	private async Task OnUpdateCommand()
	{
		var uiContent = await Node.Update();
		if (uiContent is IViewBuilder viewBuilder)
		{
			viewBuilderAdapter ??= new ViewBuilderAdapter(this);
			viewBuilderAdapter.Tabs.DisposeAndClear();
			foreach (var tabViewModel in viewBuilder.Build())
			{
				viewBuilderAdapter.Tabs.Add(tabViewModel);
			}
			// Select first tab if any present
			viewBuilderAdapter.SelectedTab = viewBuilderAdapter.Tabs.FirstOrDefault();
			viewBuilderTemplate ??= new ViewBuilderTemplate() { DataContext = viewBuilderAdapter };
			RootContent = viewBuilderTemplate;
		}
		else
		{
			RootContent = uiContent;
		}
	}
}
