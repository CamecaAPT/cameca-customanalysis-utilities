using System;
using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities;

// Must be registered as a singleton in the module
public interface IIonDataSectionPersistenceManager
{
	void Load(Guid nodeId, IReadOnlyCollection<PersistedIonDataSection> sections);
	IReadOnlyCollection<PersistedIonDataSection> Save(Guid nodeId, IReadOnlyCollection<PersistedIonDataSection> sections);
	void Register(Guid nodeId, IIonDataSectionWriter sectionWriter);
}
