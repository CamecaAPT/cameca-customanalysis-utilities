using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Cameca.CustomAnalysis.Interface;
using CommunityToolkit.Mvvm.Input;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class BasicCustomAnalysisBase<TProperties> : StandardAnalysisFilterNodeBase<TProperties>
	where TProperties : INotifyPropertyChanged, new()
{
	public AsyncRelayCommand UpdateCommand { get; }

	public virtual bool UpdateCommandCanExecute => !Resources.DataState.IsValid || Resources.DataState.IsErrorState;

	public BasicCustomAnalysisBase(IStandardAnalysisFilterNodeBaseServices services, ResourceFactory resourceFactory) : base(services, resourceFactory)
	{
		UpdateCommand = new AsyncRelayCommand(UpdateImplementationWrapper, () => UpdateCommandCanExecute);
		PropertyChanged += AbstractNode_PropertyChanged;
	}

	private void AbstractNode_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(UpdateCommandCanExecute))
		{
			UpdateCommand.NotifyCanExecuteChanged();
		}
	}

	protected override void OnCreated(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreated(eventArgs);
		Resources.DataState.PropertyChanged += DataState_PropertyChanged;
	}

	private void DataState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(INodeDataState.IsValid) || e.PropertyName == nameof(INodeDataState.IsErrorState))
		{
			OnPropertyChanged(nameof(UpdateCommandCanExecute));
		}
	}

	private async Task UpdateImplementationWrapper(CancellationToken cancellationToken)
	{
		try
		{
			Resources.DataState.IsErrorState = false;

			// Try to call async update with progress dialog if available
			Resources.DataState.IsValid = await Update(cancellationToken);
		}
		catch (OperationCanceledException)
		{
			Resources.DataState.IsValid = false;
		}
		catch
		{
			Resources.DataState.IsErrorState = true;
			throw;
		}
	}

	protected virtual Task<bool> Update(CancellationToken cancellationToken) => Task.FromResult(true);
}
