using Cameca.CustomAnalysis.Interface;
using CommunityToolkit.HighPerformance.Buffers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cameca.CustomAnalysis.Utilities.Segmentation;

/// <summary>
///  Data filter node used by <see cref="SegmentedRoiManager{TServices}" /> to filter segmented sets of ions from the parent analysis
/// </summary>
public sealed class SegmentedRoiNode : AnalysisFilterNodeBase<SegmentedRoiProperties>
{
	private static string? uniqueId = null;
	public static string UniqueId
	{
		get => uniqueId ?? throw new InvalidOperationException("RegisterSegmentedRoi was not called in the extentension IModule.RegisterTypes method");
		internal set => uniqueId = value;
	}

	/// <inheritdoc cref="SegmentedRoiNode" />
	public SegmentedRoiNode(IAnalysisFilterNodeBaseServices services, ResourceFactory resourceFactory)
		: base(services, resourceFactory)
	{
	}

	/// <inheritdoc />
	protected override IEnumerable<ReadOnlyMemory<ulong>> GetIndicesDelegate(IIonData ownerIonData, IProgress<double>? progress, CancellationToken token)
	{
		bool hasData = false;
		var dataSectionName = Resources.IonDataOwnerNode.DataSectionName;
		ulong chunkOffset = 0ul;
		if (!ownerIonData.Sections.ContainsKey(dataSectionName))
		{
			DataStateIsError = true;
			yield break;
		}
		foreach (var chunk in ownerIonData.CreateSectionDataEnumerable(dataSectionName))
		{
			var dataMem = chunk.ReadSectionData<byte>(dataSectionName);
			int bufferIndex = 0;
			using var buffer = MemoryOwner<ulong>.Allocate(chunk.Length);
			for (int i = 0; i < chunk.Length; i++)
			{
				if (dataMem.Span[i] == Properties.FilterValue)
				{
					hasData = true;
					buffer.Span[bufferIndex++] = chunkOffset + (ulong)i;
				}
			}
			yield return buffer.Memory[..bufferIndex];
			chunkOffset += (ulong)chunk.Length;
		}
		DataStateIsValid = true;
		DataStateIsError = !hasData;
	}
}
