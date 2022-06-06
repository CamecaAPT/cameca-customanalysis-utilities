using System.Collections;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class TableDefinition
{
	public string Title { get; }
	public IEnumerable Rows { get; }

	public TableDefinition(string title, IEnumerable rows)
	{
		Title = title;
		Rows = rows;
	}
}
