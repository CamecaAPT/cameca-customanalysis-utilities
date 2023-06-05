using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities;

public static class NodeInfoProviderExtensions
{
	private const string ResolutionExceptionMessage = "Could not resolve node information for NodeID = {0:B}";

	public static NodeInfoContainer GetNodeContainer(this INodeInfoProvider nodeInfoProvider, Guid nodeId)
	{
		INodeInfo ptrNodeInfo = nodeInfoProvider.Resolve(nodeId) ?? throw new CustomAnalysisException(string.Format(ResolutionExceptionMessage, nodeId));
		return new NodeInfoContainer(nodeId, ptrNodeInfo);
	}

	public static NodeInfoContainer GetRootNodeContainer(this INodeInfoProvider nodeInfoProvider, Guid nodeId)
	{
		// INodeInfo ptrNodeInfo = sourceNodeInfo;
		Guid ptrNodeId = nodeId;
		INodeInfo ptrNodeInfo = nodeInfoProvider.Resolve(ptrNodeId) ?? throw new CustomAnalysisException(string.Format(ResolutionExceptionMessage, nodeId));
		while (ptrNodeInfo.Parent.HasValue && ptrNodeInfo.Parent.Value != Guid.Empty)
		{
			ptrNodeId = ptrNodeInfo.Parent.Value;
			ptrNodeInfo = nodeInfoProvider.Resolve(ptrNodeId) ?? throw new CustomAnalysisException(string.Format(ResolutionExceptionMessage, nodeId));
		}
		return new NodeInfoContainer(ptrNodeId, ptrNodeInfo);
	}

	public static IEnumerable<NodeInfoContainer> IterateNodeContainers(this INodeInfoProvider nodeInfoProvider, Guid rootNode)
	{
		var rootNodeId = GetRootNodeContainer(nodeInfoProvider, rootNode);
		var queue = new Queue<Guid>();
		queue.Enqueue(rootNodeId.NodeId);
		while (queue.TryDequeue(out Guid nodeId))
		{
			if (nodeInfoProvider.Resolve(nodeId) is not { } nodeInfo)
				continue;
			foreach (var childNodeId in nodeInfo.Children)
			{
				queue.Enqueue(childNodeId);
			}
			yield return new NodeInfoContainer(nodeId, nodeInfo);
		}
	}
}
