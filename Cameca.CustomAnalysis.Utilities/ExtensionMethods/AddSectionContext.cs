using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Cameca.CustomAnalysis.Utilities;

public sealed class AddSectionContext
{
	public static readonly IReadOnlyDictionary<Encoding, int> SupportedStringEncodings = new Dictionary<Encoding, int>()
	{
		{ Encoding.UTF8, 8 },
		{ Encoding.Unicode, 16 }
	};

	private AddSectionContext(string name, Type? type, ulong? recordCount, uint dataTypeSizeBits, string unit, uint valuesPerRecord, bool isVirtual)
	{
		Name = name;
		Type = type;
		RecordCount = recordCount;
		DataTypeBits = dataTypeSizeBits;
		Unit = unit;
		ValuesPerRecord = valuesPerRecord;
		IsVirtual = isVirtual;
	}

	public static AddSectionContext CreateOneToOne<T>(string name, string unit = "", int valuesPerRecord = 1, bool isVirtual = false)
		where T : unmanaged
	{
		return CreateOneToOne(name, typeof(T), unit, valuesPerRecord, isVirtual);
	}

	public static AddSectionContext CreateOneToOne(string name, Type type, string unit = "", int valuesPerRecord = 1, bool isVirtual = false)
	{
		int typeSize = Marshal.SizeOf(type);
		int dataTypeSizeBits = typeSize * 8;
		return new AddSectionContext(name, type, null, (uint)dataTypeSizeBits, unit, (uint)valuesPerRecord, isVirtual);
	}

	public static AddSectionContext CreateOneToOne(string name, int dataTypeSizeBits, string unit = "", int valuesPerRecord = 1, bool isVirtual = false)
	{
		return new AddSectionContext(name, null, null, (uint)dataTypeSizeBits, unit, (uint)valuesPerRecord, isVirtual);
	}

	public static AddSectionContext CreateUnrelated<T>(string name, long recordCount, string unit = "", int valuesPerRecord = 1, bool isVirtual = false)
		where T : unmanaged
	{
		return CreateUnrelated(name, typeof(T), recordCount, unit, valuesPerRecord, isVirtual);
	}

	public static AddSectionContext CreateUnrelated(string name, Type type, long recordCount, string unit = "", int valuesPerRecord = 1, bool isVirtual = false)
	{
		int typeSize = Marshal.SizeOf(type);
		int dataTypeSizeBits = typeSize * 8;
		return new AddSectionContext(name, type, (ulong)recordCount, (uint)dataTypeSizeBits, unit, (uint)valuesPerRecord, isVirtual);
	}

	public static AddSectionContext CreateUnrelated(string name, long recordCount, int dataTypeSizeBits, string unit = "", int valuesPerRecord = 1, bool isVirtual = false)
	{
		return new AddSectionContext(name, null, (ulong)recordCount, (uint)dataTypeSizeBits, unit, (uint)valuesPerRecord, isVirtual);
	}

	public static AddSectionContext CreateString(string name, int dataTypeSizeBits = 8, int length = 0, string unit = "", bool isVirtual = false)
	{
		if (dataTypeSizeBits != 8 && dataTypeSizeBits != 16)
			throw new ArgumentException("String data type size must be 8 or 16 bits", nameof(dataTypeSizeBits));

		return new AddSectionContext(name, typeof(string), (ulong)length, (uint)dataTypeSizeBits, unit, 1, isVirtual);
	}

	public static AddSectionContext CreateString(string name, Encoding encoding, int length = 0, string unit = "", bool isVirtual = false)
	{
		if (!SupportedStringEncodings.TryGetValue(encoding, out int dataSizeBits))
		{
			throw new NotSupportedException($"Encoding {encoding.EncodingName} is not supported. Supported encodings: {(string.Join(", ", SupportedStringEncodings.Keys.Select(x => x.EncodingName)))}");
		}

		return new AddSectionContext(name, typeof(string), (ulong)length, (uint)dataSizeBits, unit, 1, isVirtual);
	}

	public string Name { get; }

	public Type? Type { get; }

	public ulong? RecordCount { get; }

	public uint DataTypeBits { get; }

	public string Unit { get; }

	public uint ValuesPerRecord { get; }

	public bool IsVirtual { get; }
}
