using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Cameca.CustomAnalysis.Utilities.Internal;

internal static class AssemblyLoadContextUtils
{
	public static string? GetAssemblyLoadContextName(this Assembly assembly)
	{
		return AssemblyLoadContext.GetLoadContext(assembly)?.Name;
	}

	public static string GetAclScopedUniqueId<T>(this Assembly assembly) where T : class
	{
		var callingAlcName = assembly.GetAssemblyLoadContextName();
		var baseName = typeof(T).FullName ?? typeof(T).Name;
		return string.Join(".", new string?[] { callingAlcName, baseName }.Where(x => x is not null));
	}
}
