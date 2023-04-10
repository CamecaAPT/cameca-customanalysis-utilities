using System;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

/// <summary>
/// Container that pairs an <see cref="INodeInfo" /> instance with the associated Nodes current InstanceId
/// </summary>
/// <param name="NodeId"></param>
/// <param name="NodeInfo"></param>
public record NodeInfoContainer(Guid NodeId, INodeInfo NodeInfo);
