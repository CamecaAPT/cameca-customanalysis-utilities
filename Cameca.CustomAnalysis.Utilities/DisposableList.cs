using System;
using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities;

public interface IDisposableCollection<T> : ICollection<T>, IDisposable where T : IDisposable { }

public static class DisposableCollectionExtensions
{
	public static void DisposeAndClear<T>(this IDisposableCollection<T> collection) where T : IDisposable
	{
		collection.Dispose();
		collection.Clear();
	}
}

public class DisposableList<T> : List<T>, IDisposableCollection<T> where T : IDisposable
{
	public void Dispose()
	{
		foreach (T item in this)
		{
			item.Dispose();
		}
	}
}
