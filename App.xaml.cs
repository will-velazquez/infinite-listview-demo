using Microsoft.UI.Xaml;

namespace App1;

public partial class App : Application
{
    public App() => InitializeComponent();

    protected override void OnLaunched(LaunchActivatedEventArgs args) => new MainWindow().Activate();
}

