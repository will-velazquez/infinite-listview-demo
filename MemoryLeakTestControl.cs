using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

namespace Fantastical.Core;

internal abstract class MemoryLeakTestControl : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public MemoryLeakTestControl()
    {
    }

    protected void RaisePropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


internal class AppTheme : INotifyPropertyChanged
{
    public static AppTheme Current { get; } = new AppTheme();

    private int _sharedClickCount;

    public int SharedClickCount
    {
        get => this._sharedClickCount;
        set
        {
            if (this._sharedClickCount != value)
            {
                this._sharedClickCount = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SharedClickCount)));
            }
        }
    }

    public Uri Logo => new Uri("ms-appx:///Assets/StoreLogo.png");

    public event PropertyChangedEventHandler? PropertyChanged;
}