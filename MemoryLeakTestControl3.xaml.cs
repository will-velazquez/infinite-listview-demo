using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Fantastical.App.Pages;

internal sealed partial class MemoryLeakTestControl3 : UserControl
{
    public static readonly DependencyProperty ClickCountProperty = DependencyProperty.Register(
        nameof(MemoryLeakTestControl3.ClickCount),
        typeof(int),
        typeof(MemoryLeakTestControl3),
        new PropertyMetadata(default(int)));

    /// <summary>
    /// When the side panel is closed, show the full header in the calendar view.
    /// </summary>
    public int ClickCount
    {
        get => (int)this.GetValue(ClickCountProperty);
        set => this.SetValue(ClickCountProperty, value);
    }

    public event EventHandler? ClickCountChanged;

    public MemoryLeakTestControl3()
    {
        this.InitializeComponent();
    }

    ~MemoryLeakTestControl3()
    {
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ++this.ClickCount;

        this.ClickCountChanged?.Invoke(this, EventArgs.Empty);
    }
}
