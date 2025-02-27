using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Cameca.CustomAnalysis.Utilities;

internal static class ResourceResolutionHelper
{

	[StackTraceHidden]
	[DebuggerStepThrough]
	[return: NotNull]
	public static T ThrowIfUnresolved<T>(this T? resource, [CallerMemberName] string? name = null)
	{
		if (resource is null)
		{
			throw new ResourceResolutionException($"Could not resolve resource: {name}");
		}
		return resource;
	}
}
