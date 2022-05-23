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
}
