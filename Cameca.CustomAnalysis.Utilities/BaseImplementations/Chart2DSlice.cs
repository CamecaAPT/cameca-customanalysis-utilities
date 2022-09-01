using System.Windows.Media;
using Cameca.CustomAnalysis.Interface;
using Prism.Mvvm;

namespace Cameca.CustomAnalysis.Utilities;

public sealed class Chart2DSlice : BindableBase, IChart2DSlice
{
	private float _min;
	public float Min
	{
		get => _min;
		set => SetProperty(ref _min, value);
	}
	
	private float _max;
	public float Max
	{
		get => _max;
		set => SetProperty(ref _max, value);
	}

	private Color _color;
	public Color Color
	{
		get => _color;
		set => SetProperty(ref _color, value);
	}
	
	private bool _isSelected;
	public bool IsSelected
	{
		get => _isSelected;
		set => SetProperty(ref _isSelected, value);
	}

	public Chart2DSlice(float min, float max, Color color, bool isSelected = false)
	{
		_min = min;
		_max = max;
		_color = color;
		_isSelected = isSelected;
	}
}
