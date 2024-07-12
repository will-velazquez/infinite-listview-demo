using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace Fantastical.App.Pages;

internal sealed partial class WindowContentDialogTest : Page
{
    public WindowContentDialogTest()
    {
        this.InitializeComponent();
    }

    private async void ShowDialog_Click(object sender, RoutedEventArgs e)
    {
        WindowedContentDialog contentDialog = new WindowedContentDialog()
        {
            RequestedTheme = this.ActualTheme,
        };

        contentDialog.Title = this.Title.Text;
        contentDialog.DialogContent = this.Content.Text;
        contentDialog.PrimaryButtonText = this.PrimaryButtonText.Text;
        contentDialog.IsPrimaryButtonEnabled = this.IsPrimaryButtonEnabled.IsChecked ?? false;
        contentDialog.SecondaryButtonText = this.SecondaryButtonText.Text;
        contentDialog.IsSecondaryButtonEnabled = this.IsSecondaryButtonEnabled.IsChecked ?? false;
        contentDialog.CloseButtonText = this.CloseButtonText.Text;
        contentDialog.DefaultButton = this.DefaultButtonRadio.SelectedItem switch
        {
            RadioButton { Content: "Primary" } => ContentDialogButton.Primary,
            RadioButton { Content: "Secondary" } => ContentDialogButton.Secondary,
            RadioButton { Content: "Close" } => ContentDialogButton.Close,
            _ => ContentDialogButton.None,
        };

        if (this.SetOwnerWindow.IsChecked ?? false)
        {
            WindowId ownerWindowId = this.XamlRoot.ContentIslandEnvironment.AppWindowId;

            _ = contentDialog.ShowAsync(ownerWindowId);
        }
        else
        {
            await contentDialog.ShowAsync();
        }
    }

    ContentDialog? _builtinContentDialog;

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape
            && this._builtinContentDialog is not null)
        {
            e.Handled = true;
            this._builtinContentDialog.Hide();

            return;
        }

        base.OnKeyDown(e);
    }

    private void ShowBuiltinDialog_Click(object sender, RoutedEventArgs e)
    {
        if (this._builtinContentDialog is not null)
        {
            this._builtinContentDialog.Hide();
            this._builtinContentDialog = null;
        }

        ContentDialog contentDialog = new ContentDialog()
        {
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme,
        };

        contentDialog.Title = this.Title.Text;
        contentDialog.Content = this.Content.Text;
        contentDialog.PrimaryButtonText = this.PrimaryButtonText.Text;
        contentDialog.IsPrimaryButtonEnabled = this.IsPrimaryButtonEnabled.IsChecked ?? false;
        contentDialog.SecondaryButtonText = this.SecondaryButtonText.Text;
        contentDialog.IsSecondaryButtonEnabled = this.IsSecondaryButtonEnabled.IsChecked ?? false;
        contentDialog.CloseButtonText = this.CloseButtonText.Text;
        contentDialog.DefaultButton = this.DefaultButtonRadio.SelectedItem switch
        {
            RadioButton { Content: "Primary" } => ContentDialogButton.Primary,
            RadioButton { Content: "Secondary" } => ContentDialogButton.Secondary,
            RadioButton { Content: "Close" } => ContentDialogButton.Close,
            _ => ContentDialogButton.None,
        };

        this._builtinContentDialog = contentDialog;

        _ = contentDialog.ShowAsync().AsTask().ContinueWith(_ =>
        {
            this._builtinContentDialog = null;
        });
    }
}
