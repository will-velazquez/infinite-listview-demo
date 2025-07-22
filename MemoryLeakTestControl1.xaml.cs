using Fantastical.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace Fantastical.App.Pages;

internal sealed partial class MemoryLeakTestControl1 : MemoryLeakTestControl
{
    private int _clickCount;

    public int ClickCount
    {
        get => this._clickCount;
        set
        {
            if (this._clickCount != value)
            {
                this._clickCount = value;
                base.RaisePropertyChanged(nameof(this.ClickCount));
            }
        }
    }

    private Visibility GetMyVisibilityBasedOnClickCount(int clickCount) => clickCount % 2 == 0 ? Visibility.Visible : Visibility.Collapsed;

    public MemoryLeakTestControl1()
    {
        this.InitializeComponent();
    }

    ~MemoryLeakTestControl1()
    {
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ++this.ClickCount;
        ++AppTheme.Current.SharedClickCount;
    }

    private void CheckAvailability_Click(object sender, RoutedEventArgs e)
    {

    }
}
