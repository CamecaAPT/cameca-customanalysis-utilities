using System;
using System.Collections.Generic;
using System.Linq;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public class IonDataSectionPersistencePersistenceManager : IIonDataSectionPersistenceManager
{
	private readonly IIonDataProvider _ionDataProvider;
	private readonly INodeInfoProvider _nodeInfoProvider;

	private readonly object _suppressSaveMutex = new();
	private readonly object _loadedNodesMutex = new();
	private readonly Dictionary<Guid, IReadOnlyCollection<PersistedIonDataSection>> _suppressSaveIds = new();
	private readonly HashSet<Guid> _loadedTopLevelNodes = new();

	private readonly Dictionary<Guid, IIonDataSectionWriter> _registrations = new();

	public IonDataSectionPersistencePersistenceManager(IIonDataProvider ionDataProvider, INodeInfoProvider nodeInfoProvider)
	{
		_ionDataProvider = ionDataProvider;
		_nodeInfoProvider = nodeInfoProvider;
	}

	public void Load(Guid nodeId, IReadOnlyCollection<PersistedIonDataSection> sections)
	{
		if (CheckIsLoaded(nodeId)) return;

		var key = GetKey(nodeId);
		if (!_registrations.TryGetValue(key, out var registration)) return;

		// Perform top-level load
		if (_ionDataProvider.GetResolvedRootIonData(nodeId) is { } rootIonData)
		{
			registration.Load(rootIonData, sections);
		}
	}

	public IReadOnlyCollection<PersistedIonDataSection> Save(Guid nodeId, IReadOnlyCollection<PersistedIonDataSection> sections)
	{
		// I think it may be possible to trigger this for multiple nodes asynchronously. Lock the set manipulation for safety.
		lock (_suppressSaveMutex)
		{
			// Atomically check for and remove node ID from the suppression list
			// A true results means that the save call should be suppress (only once, handled by the removal)
			if (_suppressSaveIds.Remove(nodeId, out var cache))
				return cache;

			var key = GetKey(nodeId);
			if (!_registrations.TryGetValue(key, out var registration))
				return Array.Empty<PersistedIonDataSection>();

			// Perform top-level save
			// Getting root ion data is both critical, and can be assumed to succeed. If we can't get this ion data then we have bigger issues.
			var rootIonData = _ionDataProvider.GetResolvedRootIonData(nodeId) ??
			                  throw new InvalidOperationException("No top level ion data is available");
			cache = registration.Save(rootIonData, sections);

			// A false result means this should be interpreted as a new save for the analysis level.
			// Add all other nodes of the same type to the set to be suppressed
			foreach (var suppressNodeId in GetNodesIdsOfMatchingTypeExclusive(nodeId))
			{
				_suppressSaveIds[suppressNodeId] = cache;
			}
			return cache;
		}
	}

	public void Register(Guid nodeId, IIonDataSectionWriter sectionWriter)
	{
		var key = GetKey(nodeId);
		sectionWriter.AnalysisSetUniquenessToken = key.ToString("N");
		_registrations[key] = sectionWriter;
	}

	private Guid GetKey(Guid nodeId) => _nodeInfoProvider.GetRootNodeContainer(nodeId).NodeId;

	private bool CheckIsLoaded(Guid nodeId)
	{
		// Prune: If not resolved, assume the analysis set no longer exists
		lock (_loadedNodesMutex)
		{
			_loadedTopLevelNodes.RemoveWhere(x => _ionDataProvider.Resolve(x) is not null);
		}

		var rootNodeId = _nodeInfoProvider.GetRootNodeContainer(nodeId).NodeId;
		lock (_loadedNodesMutex)
		{
			if (_loadedTopLevelNodes.Contains(rootNodeId))
			{
				return true;
			}
			_loadedTopLevelNodes.Add(rootNodeId);
		}
		return false;
	}

	private IEnumerable<Guid> GetNodesIdsOfMatchingTypeExclusive(Guid nodeId)
	{
		var targetNode = _nodeInfoProvider.GetNodeContainer(nodeId);
		var rootNode = _nodeInfoProvider.GetRootNodeContainer(nodeId);
		return _nodeInfoProvider.IterateNodeContainers(rootNode.NodeId)
			.Where(x => x.NodeId != targetNode.NodeId)
			.Where(x => x.NodeInfo.TypeId == targetNode.NodeInfo.TypeId)
			.Select(x => x.NodeId);
	}
}
