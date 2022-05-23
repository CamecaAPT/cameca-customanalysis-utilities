using System;
using System.Linq;

namespace Cameca.CustomAnalysis.Utilities;

public static class ExtensionUtils
{
	public static string UniqueId(this Type type)
		=> type.FullName ?? string.Join(".", new[] { type.Namespace, type.Name }.Where(x => x is not null));
}
