using System;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

internal record ExtensionDataStoreKey(string Name, Version Version) : IExtensionDataStoreKey;
