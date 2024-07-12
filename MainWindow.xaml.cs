using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace Fantastical.App;

public sealed partial class MainWindow : Window
{
    sealed class FooEx : Exception
    {

    }

    public MainWindow()
    {
        this.Closed += MainWindow_Closed;
        this.InitializeComponent();

        bool yeah = false;

        try
        {
            throw new FooEx();
        }
        catch (FooEx ex) when (yeah)
        {
            Debug.WriteLine($"Here");
            throw;
        }
        catch (Exception ex)
        {

        }
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        Application.Current.Exit();
    }

    private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {

        }
        else
        {
            string pageTypeName = $"{nameof(Fantastical.App)}.{nameof(Pages)}.{((NavigationViewItem)args.SelectedItem).Tag}";
            Type pageType = Type.GetType(pageTypeName) ?? throw new NullReferenceException();
            this.ContentFrame.Navigate(pageType);
        }
    }
}