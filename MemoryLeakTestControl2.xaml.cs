using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fantastical.App.Pages;

internal sealed partial class MemoryLeakTestControl2 : UserControl
{
    public static readonly DependencyProperty ClickCountProperty = DependencyProperty.Register(
        nameof(MemoryLeakTestControl2.ClickCount),
        typeof(int),
        typeof(MemoryLeakTestControl2),
        new PropertyMetadata(default(int)));

    /// <summary>
    /// When the side panel is closed, show the full header in the calendar view.
    /// </summary>
    public int ClickCount
    {
        get => (int)this.GetValue(ClickCountProperty);
        set => this.SetValue(ClickCountProperty, value);
    }

    public MemoryLeakTestControl2()
    {
        this.InitializeComponent();
    }

    ~MemoryLeakTestControl2()
    {
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ++this.ClickCount;
    }

    private void MemoryLeakTestControl3_ClickCountChanged(object sender, System.EventArgs e)
    {
    }
}
