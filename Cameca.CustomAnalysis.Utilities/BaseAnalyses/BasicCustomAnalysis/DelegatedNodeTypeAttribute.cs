using System;
using System.Linq;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public class DelegatedNodeTypeAttribute : NodeTypeAttribute
{
	private static NodeType GetFromDelegatedType(Type delegatedType)
	{
		return delegatedType
			.GetCustomAttributes(typeof(NodeTypeAttribute), true)
			.OfType<NodeTypeAttribute>()
			.Select(x => x.NodeType)
			.FirstOrDefault();
	}

	public DelegatedNodeTypeAttribute(Type delegatedType) : base(GetFromDelegatedType(delegatedType))
	{
	}
}
