using Microsoft.UI.Xaml.Controls;
using System;

namespace Fantastical.App.Pages;

internal sealed partial class MemoryLeakTest : Page
{
    public MemoryLeakTest()
    {
        this.InitializeComponent();
    }

    private void AddControl1Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.PART_ControlsContainer.Children.Add(new MemoryLeakTestControl1()
        {
        });
    }

    private void AddControl2Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.PART_ControlsContainer.Children.Add(new MemoryLeakTestControl2()
        {
        });
    }

    private void ClearButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        this.PART_ControlsContainer.Children.Clear();
    }

    private void GCButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
