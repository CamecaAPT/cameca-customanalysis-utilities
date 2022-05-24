using System;

namespace Cameca.CustomAnalysis.Utilities;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class DefaultViewAttribute : Attribute
{
	public string ViewModelUniqueId { get; }

	public Type ViewModelType { get; }

	public DefaultViewAttribute(string viewModelUniqueId, Type viewModelType)
	{
		ViewModelUniqueId = viewModelUniqueId;
		ViewModelType = viewModelType;
	}
}
