using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities.Segmentation;

/// <summary>
/// Static factory helper for creating <see cref="SegmentedRoiManager{TServices}"/> instances for any <see cref="CoreNodeBase{TServices}"/>
/// </summary>
public static class SegmentedRoiManager
{
	/// <inheritdoc cref="SegmentedRoiManager"/>
	public static SegmentedRoiManager<TServices> Create<TServices>(CoreNodeBase<TServices> ownerNode)
		where TServices : ICoreNodeServices
	{
		return new SegmentedRoiManager<TServices>(ownerNode);
	}
}

/// <summary>
/// Utility to segment a dataset into multiple child filter nodes based on an assigned value per ion
/// </summary>
/// <remarks>
/// <para>
/// This assumes that the owner node passed to this class will create a custom data section using the <see cref="INodeResource.DataSectionName"/>.
/// This data section should be a 1-to-1 section (null record count indicating derivation from number of data points), have 1 value per record,
/// and a data type of <see cref="byte"/>. Each byte represents the segment that the ion belongs to.
/// When <see cref="SegmentedRoiManager{TServices}.Update(Func{byte, string}?, byte[]?, CancellationToken)"/> is called, this first reads the
/// parent data section for all unique values. A child node for each unique value is then created or updated, each filtering to a value.
/// This creates a seperate filter for each segment value in the parent data section.
/// </para>
/// <para>
/// 
/// </para>
/// </remarks>
/// <typeparam name="TServices"></typeparam>
public sealed class SegmentedRoiManager<TServices> where TServices : ICoreNodeServices
{
	private readonly CoreNodeBase<TServices> ownerNode;

	/// <inheritdoc cref="SegmentedRoiManager{TServices}"/>
	internal SegmentedRoiManager(CoreNodeBase<TServices> ownerNode)
	{
		this.ownerNode = ownerNode;
	}

	/// <summary>
	/// Marks all child segment nodes (ignoring any other manually added additional nodes) as invalid, requiring recalculation
	/// </summary>
	/// <remarks>
	/// This should be called if the data that drived the owner data section changes, or if the data section itself is removed
	/// or changed.
	/// </remarks>
	public void InvalidateChildren()
	{
		foreach (var childId in SegmentedRoiIdsEnumerable())
		{
			// Find all child segmented ROI nodes
			if (ownerNode.Services.DataStateProvider.Resolve(childId) is INodeDataState childDataState)
			{
				childDataState.IsValid = false;
			}
		}
	}

	/// <summary>
	/// Removes all child segment nodes  (ignoring any other manually added additional nodes)
	/// </summary>
	/// <remarks>
	/// This should be called if the operation mode of the analysis changed in such as way that is should no longer have segmented child filter nodes.
	/// </remarks>
	/// <param name="prompt">Sets the strategy used for when the confirm delete node prompt should be displayed</param>
	public void RemoveChildren(DeleteChildPrompt prompt = DeleteChildPrompt.Always)
	{
		foreach (var childId in SegmentedRoiIdsEnumerable())
		{
			bool resolvedPrompt = prompt switch
			{
				DeleteChildPrompt.IfNotEmpty => ownerNode.Resources.Children.FirstOrDefault(x => x.Id == childId)?.Children.Any() ?? true,
				DeleteChildPrompt.Never => false,
				_ => true,
			};
			ownerNode.Resources.Events.PublishDeleteNode(childId, resolvedPrompt);
		}
	}

	/// <summary>
	/// Updates the child segmented filter nodes
	/// </summary>
	/// <remarks>
	/// Reconciles sections from the owner data section. Identifies unique values from the data section, removes any given excluded values,
	/// and then creates/removes/renames nodes as necessary to match
	/// </remarks>
	/// <param name="getSegmentTitle">A function called for each child, mapping the <see cref="byte"/> value to a string name. If not provided, a default names is used.</param>
	/// <param name="excludeIds">A collection of values for which no child is created. Useful for excluding background or other unused ions by some fixed excluded value</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task Update(Func<byte, string>? getSegmentTitle = null, byte[]? excludeIds = null, CancellationToken cancellationToken = default)
	{
		if (await ownerNode.Resources.GetIonData(cancellationToken: cancellationToken) is not { } ionData)
		{
			return;
		}
		UpdateImpl(ionData, getSegmentTitle, excludeIds, cancellationToken);
	}

	/// <inheritdoc cref="SegmentedRoiManager{TServices}.Update"/>
	public void UpdateSync(Func<byte, string>? getSegmentTitle = null, byte[]? excludeIds = null, CancellationToken cancellationToken = default)
	{
		if (ownerNode.Resources.GetValidIonData() is not { } ionData)
		{
			return;
		}
		UpdateImpl(ionData, getSegmentTitle, excludeIds, cancellationToken);
	}

	private void UpdateImpl(IIonData ionData, Func<byte, string>? getSegmentTitle, byte[]? excludeIds, CancellationToken cancellationToken)
	{
		excludeIds ??= Array.Empty<byte>();
		var resources = ownerNode.Resources;

		// If the section doesn't exist, delete everything
		ISet<byte> segmentationValues = new HashSet<byte>();
		if (ionData.Sections.ContainsKey(resources.DataSectionName))
		{
			ValidateSegmentationDataSection(ionData, resources.DataSectionName);
			segmentationValues = GetUniqueValues(ionData, resources.DataSectionName, excludeIds);
		}

		foreach (var child in resources.Children)
		{
			if (ownerNode.Services.InstanceProvider.Resolve(child.Id) is not SegmentedRoiNode roiNode)
			{
				continue;
			}

			// if the child matches the desired ID, rename to ensure name remains accurate as names for a given filter value could change
			// then pop from the copy so we don't consider again
			if (segmentationValues.Contains(roiNode.Properties.FilterValue))
			{
				var title = ResolveTitle(getSegmentTitle, roiNode.Properties.FilterValue);
				resources.Events.PublishRenameNode(child.Id, title);
				segmentationValues.Remove(roiNode.Properties.FilterValue);
			}
			// the child has a filter ID that no longer is included: delete it
			else if (!segmentationValues.Contains(roiNode.Properties.FilterValue))
			{
				if (ownerNode.Services.DataStateProvider.Resolve(child.Id) is INodeDataState childDataState)
				{
					childDataState.IsErrorState = true;
				}
				resources.Events.PublishDeleteNode(child.Id, child.Children.Any());
			}
		}
		// for everything remaining in the copy at this point, it didn't match so it needs to be creaetd
		foreach (var newFilterId in segmentationValues)
		{
			var title = ResolveTitle(getSegmentTitle, newFilterId);
			var newId = resources.CreateChildNode(SegmentedRoiNode.UniqueId, ownerNode.Id, title);
			if (ownerNode.Services.InstanceProvider.Resolve(newId) is SegmentedRoiNode childFilterNode)
			{
				childFilterNode.Properties.FilterValue = newFilterId;
			}
		}
	}

	private static string ResolveTitle(Func<byte, string>? getSegmentTitle, byte filterId)
	{

		return getSegmentTitle?.Invoke(filterId) ?? $"Segment {filterId}";
	}

	private static ISet<byte> GetUniqueValues(IIonData ionData, string sectionName, params byte[] exclude)
	{
		var uniqueValues = new HashSet<byte>();
		ulong chunkOffset = 0ul;
		foreach (var chunk in ionData.CreateSectionDataEnumerable(sectionName))
		{
			var segmentationData = chunk.ReadSectionData<byte>(sectionName).Span;
			for (int i = 0; i < chunk.Length; i++)
			{
				uniqueValues.Add(segmentationData[i]);
			}
			chunkOffset += (ulong)chunk.Length;
		}
		// Remove any of the optional excluded values if present
		uniqueValues.ExceptWith(exclude);
		return uniqueValues;
	}

	private static void ValidateSegmentationDataSection(IIonData ionData, string sectionName)
	{
		if (!ionData.Sections.ContainsKey(sectionName))
		{
			throw new InvalidOperationException($"Invalid segmentation data section: `{sectionName}` does not exist");
		}
		var section = ionData.Sections[sectionName];
		// Check if OneToOne (duck test - if the section count equals the IIonData count, then it can be used as OneToOne regardless of actual relationship type)
		if (ionData.IonCount != section.RecordCount)
		{
			throw new InvalidOperationException($"Invalid segmentation data section: data section relationship must be one-to-one: IIonData.IonCount({ionData.IonCount}) != ISectionIofo.RecordCount({section.RecordCount})");
		}
		if (section.Type != typeof(byte))
		{
			throw new InvalidOperationException($"Invalid segmentation data section: expected `byte` type but found {(section.Type?.Name ?? "Unknown")}");
		}
		if (section.ValuesPerRecord != 1)
		{
			throw new InvalidOperationException($"Invalid segmentation data section: section must only one have one value per record but got ValuesPerRecrod = {section.ValuesPerRecord}");
		}
	}

	private IEnumerable<Guid> SegmentedRoiIdsEnumerable()
	{
		var resources = ownerNode.Resources;
		foreach (var child in resources.Children)
		{
			// Find all child segmented ROI nodes
			if (ownerNode.Services.InstanceProvider.Resolve(child.Id) is SegmentedRoiNode roiNode)
			{
				yield return child.Id;
			}
		}
	}
}
