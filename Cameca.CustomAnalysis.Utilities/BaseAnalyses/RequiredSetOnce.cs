using System;

namespace Cameca.CustomAnalysis.Utilities;

internal class RequiredSetOnce<T>
{
	private T _value = default!;

	public bool IsSet { get; private set; } = false;

	public T Value
	{
		get
		{
			if (!IsSet) throw new InvalidOperationException("Value must be set before accessing");
			return _value;
		}
		set
		{
			if (IsSet) throw new InvalidOperationException("Value may only be set once.");
			_value = value;
			IsSet = true;
		}
	}

	public static implicit operator T(RequiredSetOnce<T> setOnce)
	{
		return setOnce.Value;
	}

	public void EnsureSet(T value)
	{
		if (!IsSet)
		{
			Value = value;
		}
	}
}
