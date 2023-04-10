using System;

namespace Cameca.CustomAnalysis.Utilities;

public record IonDataSectionDefinition(string Name, Type Type, string Unit = "", uint ValuePerRecord = 1);
