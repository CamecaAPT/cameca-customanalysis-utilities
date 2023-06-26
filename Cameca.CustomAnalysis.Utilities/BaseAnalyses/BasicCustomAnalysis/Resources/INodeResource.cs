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
	INodeResource? Parent { get; }
	IEnumerable<INodeResource> Children { get; }
	IMassSpectrumRangeManager RangeManager { get; }
	IExportToCsv? ExportToCsv { get; }
	INodeResource IonDataOwnerNode { get; }
	Task<IIonData?> GetIonData(IProgress<double>? progress = null, CancellationToken cancellationToken = default);
	IIonData? GetValidIonData();
}
