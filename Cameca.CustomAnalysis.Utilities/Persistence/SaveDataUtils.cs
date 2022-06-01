using System.Text;

namespace Cameca.CustomAnalysis.Utilities;

public static class SaveDataUtils
{
	public static byte[]? ToBytes(string? data) => data is not null ? Encoding.UTF8.GetBytes(data) : null;

	public static string? ToString(byte[]? data) => data is not null ? Encoding.UTF8.GetString(data) : null;
}
