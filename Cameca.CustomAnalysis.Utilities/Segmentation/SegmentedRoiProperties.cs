using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cameca.CustomAnalysis.Utilities.Segmentation;

/// <summary>
/// Properties object for <see cref="SegmentedRoiNode" /> to persist the associated value
/// used to filter to a a specific value from the parent data section
/// </summary>
public sealed class SegmentedRoiProperties : ObservableObject
{
	/// <summary>
	/// Value that the <see cref="SegmentedRoiNode" /> will filter the parent data section to
	/// </summary>
	[Display(Name = "Filter Value")]
	[ReadOnly(true)]
	public byte FilterValue { get; set; }
}
