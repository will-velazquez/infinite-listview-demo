using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace App1;

class UniformStackLayout : NonVirtualizingLayout
{
	public readonly static DependencyProperty OrientationProperty = DependencyProperty.Register(
		nameof(Orientation),
		typeof(Orientation),
		typeof(AvailabilityGrid),
		new PropertyMetadata(Orientation.Vertical, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((UniformStackLayout)d).InvalidateMeasure();
		}));

	public Orientation Orientation
	{
		get => (Orientation)this.GetValue(OrientationProperty);
		set => this.SetValue(OrientationProperty, value);
	}

	public readonly static DependencyProperty PaddingProperty = DependencyProperty.Register(
			nameof(Padding),
			typeof(Thickness),
			typeof(AvailabilityGrid),
			new PropertyMetadata(default(Thickness), (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
			{
				((UniformStackLayout)d).InvalidateMeasure();
			}));

	public Thickness Padding
	{
		get => (Thickness)this.GetValue(PaddingProperty);
		set => this.SetValue(PaddingProperty, value);
	}

	protected override void InitializeForContextCore(NonVirtualizingLayoutContext context)
	{
		base.InitializeForContextCore(context);
	}

	protected override void UninitializeForContextCore(NonVirtualizingLayoutContext context)
	{
		base.UninitializeForContextCore(context);
	}

	protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
	{
		Thickness padding = this.Padding;
		double paddingHeight = padding.Top + padding.Bottom;
		double paddingWidth = padding.Left + padding.Right;

		if (this.Orientation == Orientation.Horizontal)
		{
			Size columnSize = new Size((availableSize.Width - paddingWidth) / context.Children.Count, availableSize.Height - paddingHeight);
			double eltWidth = 0;
			double maxHeight = 0;

			foreach (UIElement uiElement in context.Children)
			{
				uiElement.Measure(columnSize);

				Size childDesiredSize = uiElement.DesiredSize;

				eltWidth += childDesiredSize.Width;
				maxHeight = Math.Max(maxHeight, childDesiredSize.Height);
			}

			return new Size(eltWidth, maxHeight);
		}
		else if (this.Orientation == Orientation.Vertical)
		{
			Size rowSize = new Size(availableSize.Width - paddingWidth, (availableSize.Height - paddingHeight) / context.Children.Count);
			double eltHeight = 0;
			double maxWidth = 0;

			foreach (UIElement uiElement in context.Children)
			{
				uiElement.Measure(rowSize);

				Size childDesiredSize = uiElement.DesiredSize;

				eltHeight += childDesiredSize.Height;
				maxWidth = Math.Max(maxWidth, childDesiredSize.Width);
			}

			return new Size(maxWidth, eltHeight);
		}
		else
		{
			throw new NotImplementedException();
		}
	}

	protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
	{
		Thickness padding = this.Padding;
		double paddingHeight = padding.Top + padding.Bottom;
		double paddingWidth = padding.Left + padding.Right;

		if (this.Orientation == Orientation.Horizontal)
		{
			double columnWidth = (finalSize.Width - paddingWidth) / context.Children.Count;
			double columnHeight = finalSize.Height - paddingHeight;
			double left = padding.Left;
			double top = padding.Top;

			foreach (UIElement uiElement in context.Children)
			{
				Rect finalRect = new Rect(left, top, columnWidth, columnHeight);
				uiElement.Arrange(finalRect);

				left += columnWidth;
			}

			return finalSize;
		}
		else if (this.Orientation == Orientation.Vertical)
		{
			double rowHeight = (finalSize.Height - padding.Top - padding.Bottom) / context.Children.Count;
			double rowWidth = finalSize.Width - paddingWidth;
			double top = padding.Top;
			double left = padding.Left;

			foreach (UIElement uiElement in context.Children)
			{
				Rect finalRect = new Rect(left, top, rowWidth, rowHeight);
				uiElement.Arrange(finalRect);

				top += rowHeight;
			}

			return finalSize;
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}
