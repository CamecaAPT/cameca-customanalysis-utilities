using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;

namespace Cameca.CustomAnalysis.Utilities;

public class NodeDisplayInfo : INodeDisplayInfo
{
	public string Title { get; }

	public ImageSource? Icon { get; }

	public NodeDisplayInfo(string title, ImageSource? icon = null)
	{
		Title = title;
		Icon = icon;
	}
}
