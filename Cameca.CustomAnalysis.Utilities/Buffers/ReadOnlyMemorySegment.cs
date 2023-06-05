using System;
using System.Buffers;

namespace Cameca.CustomAnalysis.Utilities;

/// <summary>
/// Basic ReadOnlySequenceSegment implementation for building a ReadOnlySequence from multiple ReadOnlyMemory instances
/// </summary>
/// <remarks>https://www.stevejgordon.co.uk/creating-a-readonlysequence-from-array-data-in-dotnet</remarks>
/// <typeparam name="T"></typeparam>
internal class ReadOnlyMemorySegment<T> : ReadOnlySequenceSegment<T>
{
	public ReadOnlyMemorySegment(ReadOnlyMemory<T> memory)
	{
		Memory = memory;
	}

	public ReadOnlyMemorySegment<T> Append(ReadOnlyMemory<T> memory)
	{
		var segment = new ReadOnlyMemorySegment<T>(memory)
		{
			RunningIndex = this.RunningIndex + this.Memory.Length,
		};
		Next = segment;
		return segment;
	}
}
