using Cameca.CustomAnalysis.Interface;
using Cameca.Extensions.Controls;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Cameca.CustomAnalysis.Utilities;

public class HistogramMouseWheelCustomStepBehavior : Behavior<Chart2D>
{
	public int MaxCustomStep { get; init; } = 5000;

	public ModifierKeys ModifierKey { get; init; } = ModifierKeys.Control;

	public ModifierKeys MultiplyModifierKey { get; init; } = ModifierKeys.Shift;

	public int MultiplyCoefficient { get; init; } = 10;

	protected override void OnAttached()
	{
		AssociatedObject.PreviewMouseWheel += Chart2D_PreviewMouseWheel;
	}

	protected override void OnDetaching()
	{
		AssociatedObject.PreviewMouseWheel -= Chart2D_PreviewMouseWheel;
	}

	private void Chart2D_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		var histRenderData = new List<IHistogramRenderData>();
		foreach (var item in AssociatedObject.DataSource)
		{
			if (item is IHistogramRenderData histogramData)
			{
				histRenderData.Add(histogramData);
			}
		}
		if (!histRenderData.Any()) { return; }

		if (ModifierKey != ModifierKeys.None && Keyboard.Modifiers.HasFlag(ModifierKey))
		{
			int step = -Math.Sign(e.Delta);
			if (MultiplyModifierKey != ModifierKeys.None && Keyboard.Modifiers.HasFlag(MultiplyModifierKey))
			{
				step *= MultiplyCoefficient;
			}

			foreach (var item in histRenderData)
			{
				item.CustomStep = ClampToValidRange(item.CustomStep + step);
			}
			e.Handled = true;
		}
	}

	private int ClampToValidRange(int step)
	{
		if (step < 1)
		{
			return 1;
		}
		else if (step > MaxCustomStep)
		{
			return MaxCustomStep;
		}
		else
		{
			return step;
		}
	}
}
