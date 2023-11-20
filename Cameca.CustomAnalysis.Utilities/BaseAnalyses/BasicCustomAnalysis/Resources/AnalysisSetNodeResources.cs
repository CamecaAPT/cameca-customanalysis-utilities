using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cameca.CustomAnalysis.Utilities;

public class AnalysisSetNodeResources
{
	private readonly Dictionary<Guid, NodeResource> _existingNodes = new();

	private readonly Func<NodeResource> _innerNodeResourceFactory;

	public AnalysisSetNodeResources(Func<NodeResource> innerNodeResourceFactory)
	{
		_innerNodeResourceFactory = innerNodeResourceFactory;
	}

	[return: NotNullIfNotNull("nodeId")]
	public NodeResource? GetOrCreate(Guid? nodeId)
	{
		if (!nodeId.HasValue)
		{
			return null;
		}
		Guid nodeIdValue = nodeId.Value;
		if (_existingNodes.TryGetValue(nodeIdValue, out var nodeResource))
			return nodeResource;
		nodeResource = _innerNodeResourceFactory();
		nodeResource.AnalysisSetNodeResourcesBacking.Value = this;
		nodeResource.Id = nodeIdValue;
		_existingNodes[nodeIdValue] = nodeResource;
		return nodeResource;
	}
}
