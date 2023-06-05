using System;

namespace Cameca.CustomAnalysis.Utilities;

public static class JsonModelMapper
{
	public static IonDataSectionJsonModel? ToJsonModel(IonDataSection? ionDataSection)
	{
		if (ionDataSection is null) return null;
		return new IonDataSectionJsonModel
		{
			Name = ionDataSection.Name,
			TypeFullName = SerializeType(ionDataSection.Type),
			Unit = ionDataSection.Unit,
			ValuesPerRecord = ionDataSection.ValuesPerRecord,
		};
	}

	public static IonDataSection? FromJsonModel(IonDataSectionJsonModel? model)
	{
		if (model is null) return null;
		return new IonDataSection
		{
			Name = model.Name,
			Type = DeserializeType(model.TypeFullName),
			Unit = model.Unit,
			ValuesPerRecord = model.ValuesPerRecord,
		};
	}

	public static PersistedIonDataSectionJsonModel ToJsonModel(PersistedIonDataSection persistedIonDataSection)
	{
		return new PersistedIonDataSectionJsonModel
		{
			IonDataSection = JsonModelMapper.ToJsonModel(persistedIonDataSection.IonDataSection),
			RuntimeTypeFullName = SerializeType(persistedIonDataSection.RuntimeType),
			FileFullName = persistedIonDataSection.FileFullName,
		};
	}

	public static PersistedIonDataSection FromJsonModel(PersistedIonDataSectionJsonModel model)
	{
		return new PersistedIonDataSection
		{
			IonDataSection = JsonModelMapper.FromJsonModel(model.IonDataSection),
			RuntimeType = DeserializeType(model.RuntimeTypeFullName),
			FileFullName = model.FileFullName,
		};
	}

	private static string? SerializeType(Type? type) => type?.FullName;

	private static Type? DeserializeType(string? serializedType) =>
		serializedType is not null ? Type.GetType(serializedType) : null;
}
