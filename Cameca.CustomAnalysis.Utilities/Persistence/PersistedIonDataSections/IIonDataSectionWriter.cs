using Cameca.CustomAnalysis.Interface;
using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities;

public interface IIonDataSectionWriter
{
	string AnalysisSetUniquenessToken { get; set; }
	IReadOnlyCollection<PersistedIonDataSection> Save(IIonData ionData, IReadOnlyCollection<PersistedIonDataSection> sections);
	void Load(IIonData ionData, IReadOnlyCollection<PersistedIonDataSection> sections);
}
