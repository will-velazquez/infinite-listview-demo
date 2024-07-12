using Fantastical.Core;
using Microsoft.UI.Xaml;
using System;
using System.Globalization;
using WinRT;

namespace Fantastical.App;

public sealed partial class App : Application
{
    [global::System.Runtime.InteropServices.DllImport("Microsoft.ui.xaml.dll")]
    [global::System.Runtime.InteropServices.DefaultDllImportSearchPaths(global::System.Runtime.InteropServices.DllImportSearchPath.SafeDirectories)]

    private static extern void XamlCheckProcessRequirements();
    public App() => InitializeComponent();

    protected override void OnLaunched(LaunchActivatedEventArgs args) => new MainWindow().Activate();

    [STAThread]
    static void Main()
    {
        XamlCheckProcessRequirements();

        ComWrappersSupport.InitializeComWrappers();

        Application.Start((p) =>
        {
            var context = new global::Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(global::Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
            global::System.Threading.SynchronizationContext.SetSynchronizationContext(context);
            (new App()).DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;
        });

        return;
    }
}

