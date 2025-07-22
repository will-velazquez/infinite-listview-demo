using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace Fantastical.App.Pages;

internal sealed partial class ViewportTest : Page
{
    public ViewportTest()
    {
        this.InitializeComponent();
    }

    private void Rectangle_EffectiveViewportChanged(Microsoft.UI.Xaml.FrameworkElement sender, Microsoft.UI.Xaml.EffectiveViewportChangedEventArgs args)
    {
        this.EffectiveViewportRun.Text = args.EffectiveViewport.ToString();
        this.MaxViewportRun.Text = args.MaxViewport.ToString();
        this.BringIntoViewDistanceXRun.Text = args.BringIntoViewDistanceX.ToString();
        this.BringIntoViewDistanceYRun.Text = args.BringIntoViewDistanceY.ToString();

        if (args.EffectiveViewport.IsEmpty)
        {
        }

        if (args.BringIntoViewDistanceX != 0
            || args.BringIntoViewDistanceY != 0
            || args.EffectiveViewport.Width < sender.ActualWidth
            || args.EffectiveViewport.Height < sender.ActualHeight)
        {
            VisualStateManager.GoToState(this, nameof(this.STATE_Occluded), true);
        }
        else
        {
            VisualStateManager.GoToState(this, nameof(this.STATE_Normal), true);
        }

        string? prev = this.PART_StatusText.Text;

        if (!string.IsNullOrEmpty(prev))
        {
            prev = prev.Split('\n').LastOrDefault() ?? string.Empty;
        }

        string str = $"{args.EffectiveViewport.Width}/{args.EffectiveViewport.Height}";

        if (!string.IsNullOrEmpty(prev))
        {
            str = $"{prev}\n{str}";
        }

        this.PART_StatusText.Text = str;
    }
}
