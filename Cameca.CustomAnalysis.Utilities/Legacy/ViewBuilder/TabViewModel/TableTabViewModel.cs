using System.Collections.Generic;

namespace Cameca.CustomAnalysis.Utilities.Legacy;

internal class TableTabViewModel
{
	public string Title { get; }

	public ICollection<object> Items { get; }

	public TableTabViewModel(string title, ICollection<object> items)
	{
		Title = title;
		Items = items;
	}
}
