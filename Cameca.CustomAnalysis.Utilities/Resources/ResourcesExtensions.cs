using Cameca.CustomAnalysis.Interface;
using System;
using System.Linq;
using System.Windows.Media;

namespace Cameca.CustomAnalysis.Utilities;

public static class ResourcesExtensions
{
	public static Color GetIonColor(this IResources resources, string name, IonFormula? formula = null)
	{
		if (formula is null)
		{
			try
			{
				formula = IonFormulaEx.Parse(name);
			}
			catch (ArgumentException)
			{
				// Name is not a valid formula string: fall back to using Unknown formula
				formula = IonFormula.Unknown;
			}
		}
		var ionTypeInfo = new ResourceIonTypeInfo(name, formula, 0d);
		return resources.IonDisplayInfo.GetColor(ionTypeInfo);
	}

	public static IonTypeInfoRange CreateRange(this IResources resources, string name, double min, double max, IonFormula? formula = null, double? volume = null, Color? color = null)
	{
		formula ??= IonFormulaEx.TryParse(name, out var parsed) ? parsed : IonFormula.Unknown;
		volume ??= resources.ElementData?.Elements.FirstOrDefault(e => e.Symbol == name)?.MolarVolume ?? 0d;
		color ??= resources.GetIonColor(name, formula);
		return new IonTypeInfoRange(name, formula, volume.Value, min, max, color.Value);
	}

	/// <summary>
	///  Creates a child node and return the created node ID
	/// </summary>
	/// <param name="resources"></param>
	/// <param name="analysisNodeName"></param>
	/// <param name="parentNodeId"></param>
	/// <param name="name"></param>
	/// <param name="icon"></param>
	/// <returns></returns>
	public static Guid CreateChildNode(this IResources resources, string analysisNodeName, Guid parentNodeId, string? name = null, ImageSource? icon = null)
	{
		Guid? newNodeId = null;
		void ReturnNewNodeId(NodeCreatedEventArgs e)
		{
			newNodeId = e.NodeId;
		};
		static bool CreateFilter(NodeCreatedEventArgs e) => e.Trigger == EventTrigger.Create;

		using (var token = resources.Events.SubscribeNodeCreated(ReturnNewNodeId, CreateFilter))
		{
			resources.Events.PublishCreateNode(analysisNodeName, parentNodeId, name, icon);
		}
		return newNodeId.HasValue ? newNodeId.Value : throw new InvalidOperationException($"Could not create node of type \"{analysisNodeName}\". Ensure that this node type is registered in the IModule.RegisterTypes implementation. ");
	}

	private record ResourceIonTypeInfo(string Name, IonFormula Formula, double Volume) : IIonTypeInfo;
}
