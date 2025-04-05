using System;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public class ResourceResolutionException : CustomAnalysisException
{
	public ResourceResolutionException()
	{
	}

	public ResourceResolutionException(string message)
		: base(message)
	{
	}

	public ResourceResolutionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
