using Cameca.CustomAnalysis.Interface;
using CommunityToolkit.HighPerformance;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Cameca.CustomAnalysis.Utilities.ExtensionMethods;

public static class AptFileBuilderExtensions
{
	public static IAptFileBuilder AddPositionSection(this IAptFileBuilder builder, ReadOnlyMemory<float> positions, string? unit = "nm")
		=> AddPositionSection(builder, positions.Cast<float, Vector3>(), unit);

	public static IAptFileBuilder AddPositionSection(this IAptFileBuilder builder, ReadOnlyMemory<Vector3> positions, string? unit = "nm")
	{
		builder.AddSection(
			IonDataSectionName.Position,
			RelationType.OneToOne,
			RecordType.Fixed,
			RecordDataType.Float,
			32,
			12,
			unit,
			((ReadOnlyMemory<float>)GetPositionExtant(positions)).Cast<float, byte>(),
			positions.Cast<Vector3, byte>());
		return builder;
	}

	public static IAptFileBuilder AddMassSection(this IAptFileBuilder builder, ReadOnlyMemory<float> mass, string? unit = "Da")
	{
		builder.AddSection(
			IonDataSectionName.Mass,
			RelationType.OneToOne,
			RecordType.Fixed,
			RecordDataType.Float,
			32,
			4,
			unit,
			null,
			mass.Cast<float, byte>());
		return builder;
	}

	public static IAptFileBuilder AddOneToOneSection<T>(this IAptFileBuilder builder, string sectionName, ReadOnlyMemory<T> data, string? unit = null, ReadOnlyMemory<byte>? extraData = null)
		where T : unmanaged
	{
		int recordSizeBytes = Marshal.SizeOf<T>();
		int dataSizeBits = recordSizeBytes * 8;
		builder.AddSection(
			sectionName,
			RelationType.OneToOne,
			RecordType.Fixed,
			MapRecordType(typeof(T)),
			dataSizeBits,
			recordSizeBytes,
			unit,
			extraData,
			data.Cast<T, byte>());
		return builder;
	}

	public static IAptFileBuilder AddStringSection(this IAptFileBuilder builder, string sectionName, string sectionData, string? unit = null, ReadOnlyMemory<byte>? extraData = null, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;
		if (encoding != Encoding.UTF8) { throw new NotSupportedException("Encoding currently must be Encoding.UTF8"); }
		int dataSizeBits = 8;  // Currently only supports UTF8 until a method to expose the data size from the header is implemented
		var encoded = encoding.GetBytes(sectionData);

		builder.AddSection(
			sectionName,
			RelationType.Unrelated,
			RecordType.Fixed,
			RecordDataType.String,
			dataSizeBits,
			1,
			unit,
			extraData,
			encoded);
		return builder;
	}

	public static IAptFileBuilder AddJsonSerializedSection(this IAptFileBuilder builder, string sectionName, object? sectionDataObject, Type type, string? unit = null, ReadOnlyMemory<byte>? extraData = null, Encoding? encoding = null, JsonSerializerOptions? jsonSerializerOptions = null)
	{
		var serialized = JsonSerializer.Serialize(sectionDataObject, type, jsonSerializerOptions);
		return builder.AddStringSection(sectionName, serialized, unit, extraData, encoding);
	}

	public static IAptFileBuilder AddJsonSerializedSection<T>(this IAptFileBuilder builder, string sectionName, T sectionDataObject, string? unit = null, ReadOnlyMemory<byte>? extraData = null, Encoding? encoding = null, JsonSerializerOptions? jsonSerializerOptions = null)
	{
		return builder.AddJsonSerializedSection(sectionName, sectionDataObject, typeof(T), unit, extraData, encoding, jsonSerializerOptions);
	}

	private static float[] GetPositionExtant(ReadOnlyMemory<Vector3> positions)
	{
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float minZ = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		float maxZ = float.MinValue;
		for (int i = 0; i < positions.Length; i++)
		{
			var position = positions.Span[i];
			if (position.X < minX) { minX = position.X; }
			if (position.Y < minY) { minY = position.Y; }
			if (position.Z < minZ) { minZ = position.Z; }
			if (position.X > maxX) { maxX = position.X; }
			if (position.Y > maxY) { maxY = position.Y; }
			if (position.Z > maxZ) { maxZ = position.Z; }
		}
		return new float[] { minX, maxX, minY, maxY, minZ, maxZ };
	}

	private static RecordDataType MapRecordType(Type type)
	{
		if (type == typeof(byte) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
			return RecordDataType.Uint;
		if (type == typeof(sbyte) || type == typeof(short) || type == typeof(int) || type == typeof(long))
			return RecordDataType.Int;
		if (type == typeof(float) || type == typeof(double))
			return RecordDataType.Float;
		if (type == typeof(string))
			return RecordDataType.String;
		return RecordDataType.Unknown;

	}
}
