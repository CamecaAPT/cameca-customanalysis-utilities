using System;
using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities;

public static class CollectionExtensions
{
	/// <summary>
	/// Attempt to call <see cref="IDisposable.Dispose"/> on all items before calling <see cref="ICollection{T}.Clear"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="collection"></param>
	public static void DisposeAndClear<T>(this ICollection<T> collection)
	{
		foreach (var item in collection)
		{
			(item as IDisposable)?.Dispose();
		}
		collection.Clear();
	}
}
