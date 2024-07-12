using CommunityToolkit.WinUI.Controls;
using System.Linq;
using Windows.Foundation;

namespace Fantastical.Core;

/// <summary>
/// This is a patch over the Community Toolkit's EqualPanel to prevent a crash in Segmented on certain screen scalings
/// https://github.com/CommunityToolkit/Windows/pull/360
/// </summary>
public class EqualPanel_Patched : EqualPanel
{
    private int _visibleItemsCount
    {
        get => (int)typeof(EqualPanel).GetField(nameof(this._visibleItemsCount), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(this)!;
    }

    private double _maxItemWidth
    {
        get => (double)typeof(EqualPanel).GetField(nameof(this._maxItemWidth), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(this)!;
        set => typeof(EqualPanel).GetField(nameof(this._maxItemWidth), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(this, value);
    }

    private double _maxItemHeight
    {
        get => (double)typeof(EqualPanel).GetField(nameof(this._maxItemHeight), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(this)!;
        set => typeof(EqualPanel).GetField(nameof(this._maxItemHeight), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(this, value);
    }

    public EqualPanel_Patched()
        : base()
    {
        typeof(EqualPanel).GetField("_visibleItemsCount");
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double x = 0;

        // Check if there's more (little) width available - if so, set max item width to the maximum possible as we have an almost perfect height.
        if (finalSize.Width > _visibleItemsCount * _maxItemWidth + (Spacing * (_visibleItemsCount - 1)))
        {
            _maxItemWidth = (finalSize.Width - (Spacing * (_visibleItemsCount - 1))) / _visibleItemsCount;
        }

        var elements = Children.Where(static e => e.Visibility == Microsoft.UI.Xaml.Visibility.Visible);
        foreach (var child in elements)
        {
            child.Arrange(new Rect(x, 0, _maxItemWidth, _maxItemHeight));
            x += _maxItemWidth + Spacing;
        }

        return finalSize;
    }
}