using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cameca.CustomAnalysis.Utilities;

public static class MemoryBufferUtils
{
	public static async IAsyncEnumerable<ReadOnlyMemory<T>> AsyncEnumerableWrapper<T>(this Task<ReadOnlySequence<T>> sequenceTask)
	{
		var sequence = await sequenceTask;
		SequencePosition position = sequence.Start;
		while (sequence.TryGet(ref position, out var memory))
		{
			yield return memory;
		}
	}

	/// <summary>
	/// Converts an enumerable of <see cref="ReadOnlyMemory{T}" /> instances into a single <see cref="ReadOnlySequence{T}" />
	/// instance by chaining together using <see cref="ReadOnlyMemorySegment{T}" />.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="memoryEnumerable"></param>
	/// <returns></returns>
	public static ReadOnlySequence<T> AsReadOnlySequence<T>(this IEnumerable<ReadOnlyMemory<T>> memoryEnumerable)
	{
		ReadOnlyMemorySegment<T>? startSegment = null;
		ReadOnlyMemorySegment<T>? endSegment = null;
		foreach (var memory in memoryEnumerable)
		{
			if (endSegment is null)
			{
				startSegment = endSegment = new ReadOnlyMemorySegment<T>(memory);
			}
			else
			{
				endSegment = endSegment.Append(memory);
			}
		}
		if (startSegment is not null && endSegment is not null)  // Redundant logically, but compiler null check safety
		{
			return new ReadOnlySequence<T>(startSegment, 0, endSegment, endSegment.Memory.Length);
		}
		return ReadOnlySequence<T>.Empty;

	}
}
