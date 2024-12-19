using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities.ExtensionMethods;

internal static class ContainerRegistryExtensions
{
	public static IContainerRegistry EnsureRegistered<T>(this IContainerRegistry containerRegistry)
	{
		if (!containerRegistry.IsRegistered<T>())
		{
			containerRegistry.Register<T>();
		}
		return containerRegistry;
	}

	public static IContainerRegistry EnsureRegistered<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
	{
		if (!containerRegistry.IsRegistered<TFrom>())
		{
			containerRegistry.Register<TFrom, TTo>();
		}
		return containerRegistry;
	}
}
