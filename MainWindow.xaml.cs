using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace Fantastical.App;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.Closed += MainWindow_Closed;
        this.InitializeComponent();
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        Application.Current.Exit();
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {

        }
        else
        {
            string pageTypeName = $"{nameof(Fantastical)}.{nameof(Fantastical.App)}.{nameof(Pages)}.{((NavigationViewItem)args.SelectedItem).Content}";
            Type pageType = Type.GetType(pageTypeName) ?? throw new NullReferenceException();

            this.ContentFrame.Navigate(pageType);
        }
    }
}