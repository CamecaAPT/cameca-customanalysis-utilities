using Cameca.CustomAnalysis.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Cameca.CustomAnalysis.Utilities;

/// <summary>
/// A section writer that writes and reads each ion data section using a convention based file path with sufficient unique information.
/// The files will be created parallel to the underlying APT file.
/// </summary>
/// <remarks>
/// Convention is to use the base APT filename, then section name, and finally a provided uniqueness token that should be specific to each analysis set.
/// </remarks>
public class IndividualFilesIonDataSectionWriter : IIonDataSectionWriter
{
	protected virtual string Extension { get; } = "bin";

	/// <summary>
	/// Set a string token to be include in file names so that multiple analysis sets will have their own copies of the mutable ion data sections.
	/// </summary>
	public string AnalysisSetUniquenessToken { get; set; } = Guid.NewGuid().ToString("N");

	protected virtual string CreateFileFullName(IIonData ionData, string section)
	{
		var directoryPath = Directory.GetParent(ionData.Filename)?.FullName;
		var reconId = Path.GetFileNameWithoutExtension(ionData.Filename);
		return Path.Join(directoryPath, $"{reconId}_{section}_{AnalysisSetUniquenessToken}.{Extension}");
	}

	public IReadOnlyCollection<PersistedIonDataSection> Save(IIonData ionData, IReadOnlyCollection<PersistedIonDataSection> sections)
	{
		var results = new List<PersistedIonDataSection>();
		foreach (var section in sections)
		{
			results.Add(SaveSection(ionData, section));
		}
		return results;
	}
	public void Load(IIonData ionData, IReadOnlyCollection<PersistedIonDataSection> sections)
	{
		foreach (var section in sections)
		{
			LoadSection(ionData, section);
		}
	}

	private PersistedIonDataSection SaveSection(IIonData ionData, PersistedIonDataSection section)
	{
		if (section is not { IonDataSection: { Name: { } sectionName }, RuntimeType: { } runtimeType })
		{
			return section;
		}
		// We will restrict saving only to virtual sections as we will not be able to load the data anyways, so it's wasted effort
		if (!ionData.Sections.TryGetValue(sectionName, out var sectionInfo) || !sectionInfo.IsVirtual)
		{
			return section;
		}
		var fileFullName = section.FileFullName ?? CreateFileFullName(ionData, sectionName);
		using var file = File.OpenWrite(fileFullName);
		ionData.WriteSectionToStream(sectionName, runtimeType, file);
		return section with
		{
			IonDataSection = section.IonDataSection with
			{
				Type = sectionInfo.Type,
				Unit = sectionInfo.Unit,
				ValuesPerRecord = (int)sectionInfo.ValuesPerRecord,
			},
			FileFullName = fileFullName,
		};
	}

	private void LoadSection(IIonData ionData, PersistedIonDataSection section)
	{

		// Validate
		if (section is not
			{
				IonDataSection:
				{
					Name: { } sectionName,
					Type: { },
					ValuesPerRecord: > 0,
				} ionDataSection,
				RuntimeType: { } runtimeType,
				FileFullName: { } fileFullName
			})
		{
			return;
		}

		if (ionData.Sections.ContainsKey(sectionName))
		{
			return;
		}

		var fileInfo = new FileInfo(fileFullName);
		if (!fileInfo.Exists)
		{
			return;
		}

		// Pre-check file size
		long expectedBytes = (long)ionData.IonCount * Marshal.SizeOf(ionDataSection.Type) * ionDataSection.ValuesPerRecord;
		if (fileInfo.Length != expectedBytes)
		{
			return;
		}

		using var file = fileInfo.OpenRead();
		// Required to create a virtual section
		var sectionDefinition = new IonDataSectionDefinition(sectionName, ionDataSection.Type, ionDataSection.Unit, (uint)ionDataSection.ValuesPerRecord);
		ionData.WriteStreamToSection(sectionName, runtimeType, file, sectionDefinition);
	}
}
