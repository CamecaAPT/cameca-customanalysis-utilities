using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public interface INodeResource
{
	Guid Id { get; }
	string Name { get; }
	string Title { get; }
	string TypeId { get; }
	ImageSource? Icon { get; }
	IGeometricRegion? Region { get; }
	INodeResource TopLevelNode { get; }
	INodeResource? Parent { get; }
	IEnumerable<INodeResource> Children { get; }
	IMassSpectrumRangeManager? RangeManager { get; }
	IExportToCsv? ExportToCsv { get; }
	INodeResource IonDataOwnerNode { get; }
	Task<IIonData?> GetIonData(IProgress<double>? progress = null, CancellationToken cancellationToken = default);
	IIonData? GetValidIonData();
	IEnumerable<Type> ProvidedDataTypes { get; }
	T? GetData<T>(IProgress<double>? progress = null, CancellationToken cancellationToken = default) where T : class;
	Task<T?> GetDataAsync<T>(IProgress<double>? progress = null, CancellationToken cancellationToken = default) where T : class;
}
