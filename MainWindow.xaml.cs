using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinRT.Interop;

namespace App1;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

	private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
	{
        if (args.IsSettingsSelected)
        {

        }
        else
        {
            string pageTypeName = $"{nameof(App1)}.{nameof(Pages)}.{((NavigationViewItem)args.SelectedItem).Tag}";
			Type pageType = Type.GetType(pageTypeName) ?? throw new NullReferenceException();
            this.ContentFrame.Navigate(pageType);
        }
	}
}