using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Globalization;
using System.Numerics;
using System.Text;
using Windows.Foundation;
using Windows.UI;

namespace Fantastical.Core;

internal sealed class MeasureArrangeControl : Panel
{
    private readonly TextBlock _textBlock;

    public MeasureArrangeControl()
    {
        this._textBlock = new TextBlock()
        {
            Text = "Hello, world!"
        };

        this.Children.Add(this._textBlock);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        this._textBlock.Measure(availableSize);

        return this._textBlock.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        this._textBlock.Arrange(new Rect(new Point(0, 0), finalSize));

        return this._textBlock.ActualSize.ToSize();
    }
}
