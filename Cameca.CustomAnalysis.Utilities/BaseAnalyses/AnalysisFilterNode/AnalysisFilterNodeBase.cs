using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cameca.CustomAnalysis.Utilities;

public abstract class AnalysisFilterNodeBase<TProperties> : AnalysisFilterNodeBase<TProperties, IAnalysisFilterNodeBaseServices>
	where TProperties : INotifyPropertyChanged, new()
{
	protected AnalysisFilterNodeBase(IAnalysisFilterNodeBaseServices services, ResourceFactory resourceFactory) : base(services, resourceFactory) { }
}

[NodeType(NodeType.DataFilter)]
public abstract class AnalysisFilterNodeBase<TProperties, TServices> : AnalysisNodeBase<TProperties, TServices>
	where TServices : IAnalysisFilterNodeBaseServices
	where TProperties : INotifyPropertyChanged, new()
{
	protected AnalysisFilterNodeBase(TServices services, ResourceFactory resourceFactory) : base(services, resourceFactory)
	{
	}

	internal override void OnCreatedCore(NodeCreatedEventArgs eventArgs)
	{
		base.OnCreatedCore(eventArgs);
		if (Services.DataFilterProvider.Resolve(Id) is { } dataFilterInterceptor)
		{
			dataFilterInterceptor.FilterDelegate = GetIndicesDelegateAsync;
		}
	}

	[Obsolete("Use FilterProgressMessage")]
	protected virtual string? FilterCalculationMessage => null;

	[Obsolete("Use FilterIsInverted")]
	protected virtual bool IsInverted => false;

	protected string? FilterProgressMessage
	{
		get => Services.DataFilterProvider.Resolve(Id)?.FilterProgressMessage;
		set
		{
			if (Services.DataFilterProvider.Resolve(Id) is { } dataFilterInterceptor)
			{
				dataFilterInterceptor.FilterProgressMessage = value;
			}
		}
	}

	protected bool FilterIsInverted
	{
		get => Services.DataFilterProvider.Resolve(Id)?.IsInverted ?? false;
		set
		{
			if (Services.DataFilterProvider.Resolve(Id) is { } dataFilterInterceptor)
			{
				dataFilterInterceptor.IsInverted = value;
			}
		}
	}

	protected override void OnPropertiesChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertiesChanged(e);
		FilterDataChanged();
	}

#pragma warning disable CS1998
	protected virtual async IAsyncEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegateAsync(IIonData ownerIonData, IProgress<double>? progress, [EnumeratorCancellation]CancellationToken token)
#pragma warning restore CS1998
	{
		foreach (var memory in GetIndicesDelegate(ownerIonData, progress, token))
		{
			yield return memory;
		}
	}

	protected virtual IEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token)
	{
		yield return ReadOnlyMemory<ulong>.Empty;
	}

	protected void FilterDataChanged() => Services.DataFilterProvider.Resolve(Id)?.FilterDataChanged();
}
