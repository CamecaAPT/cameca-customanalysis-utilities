using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities;

public static class NodeResourcesExtensions
{
	public static INodeResource? GetMassSpectrum(this INodeResource nodeResource)
	{
		if (nodeResource.IonDataOwnerNode is { } dataOwnerNode)
		{

			foreach (var node in dataOwnerNode.Children)
			{

				if (node.TypeId == "MassSpectrumNode")
				{
					return nodeResource;
				}
			}
		}
		return null;
	}

	public static INodeResource? GetGrid(this INodeResource nodeResource)
	{
		if (nodeResource.IonDataOwnerNode is { } dataOwnerNode)
		{
			// Cubes potentially can have custom Grid3D
			if (dataOwnerNode.Region?.Shape == Shape.Cube)
			{
				foreach (var node in dataOwnerNode.Children)
				{
					if (node.TypeId == "GridNode")
					{
						return nodeResource;
					}
				}
			}
		}
		// Otherwise look in the top level nodes
		var topLevelNode = nodeResource.TopLevelNode;
		foreach (var node in topLevelNode.Children)
		{
			if (node.TypeId == "GridNode")
			{
				return nodeResource;
			}
		}
		return null;
	}

	public static IEnumerable<INodeResource> AnalysisTreeNodes(this INodeResource nodeResource)
	{
		var queue = new Queue<INodeResource>();
		queue.Enqueue(nodeResource.TopLevelNode);
		while (queue.TryDequeue(out INodeResource? node))
		{
			foreach (var childNodeId in node.Children)
			{
				queue.Enqueue(childNodeId);
			}
			yield return node;
		}
	}
}
