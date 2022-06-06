using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

public abstract class LegacyCustomAnalysisViewModelBase<TNode, TAnalysis, TOptions> : AnalysisViewModelBase<TNode>
	where TNode : LegacyCustomAnalysisNodeBase<TAnalysis, TOptions>
	where TAnalysis : ICustomAnalysis<TOptions>
	where TOptions : INotifyPropertyChanged, new()
{
	protected readonly Func<IViewBuilder> ViewBuilderFactory;
	protected bool OptionsChanged = false;

	private readonly AsyncCommand _runCommand;
	public ICommand RunCommand => _runCommand;

	public ObservableCollection<object> Tabs { get; } = new();

	private object? _selectedTab;
	public object? SelectedTab
	{
		get => _selectedTab;
		set => SetProperty(ref _selectedTab, value);
	}

	public TOptions Options => Node is not null ? Node.Options : new();

	protected LegacyCustomAnalysisViewModelBase(
		IAnalysisViewModelBaseServices services,
		Func<IViewBuilder> viewBuilderFactory)
		: base(services)
	{
		ViewBuilderFactory = viewBuilderFactory;
		_runCommand = new AsyncCommand(OnRun, RunCommandEnabled);
	}

	protected virtual async Task OnRun()
	{
		if (Node is null) return;

		// Some tab views may implement IDisposable if managing chart data. Ensure dispose before clear.
		Tabs.DisposeAndClear();

		// This is not a recommended pattern. We are creating a passing a view builder object
		// simply to avoid modifying the existing custom analysis code as much as possible.
		// The recommended technique for future analyses it to define a class containing analysis
		// results, returning an instance from an analysis function, and using those results to
		// update the view
		var viewBuilder = ViewBuilderFactory();
		await Node.Run(viewBuilder);

		foreach (var tabViewModel in viewBuilder.Build())
		{
			Tabs.Add(tabViewModel);
		}
		// Select first tab if any present
		SelectedTab = Tabs.FirstOrDefault();
	}

	protected override void OnCreated(ViewModelCreatedEventArgs eventArgs)
	{
		base.OnCreated(eventArgs);
		if (Node is { } node)
		{
			node.Options.PropertyChanged += OptionsOnPropertyChanged;
		}
	}

	private void OptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		OptionsChanged = true;
		_runCommand.RaiseCanExecuteChanged();
	}

	private bool RunCommandEnabled(object? _) => !Tabs.Any() || OptionsChanged;

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			Tabs.DisposeAndClear();
		}
		base.Dispose(disposing);
	}
}
