using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Fantastical.App.Pages;

public sealed partial class WindowTest : Page
{
    private bool _setOwnerWindow;
    private bool _modalOwnerWindowOverride;

    public WindowTest()
    {
        this.InitializeComponent();

        this._setOwnerWindow = this.Button1_Option_SetOwnerWindow.IsChecked ?? false;

        CompactOverlayPresenter presenter = CompactOverlayPresenter.Create();

        Button1_Option_DefaultButton_InitialSize.Content = $"default ({presenter.InitialSize})";
    }

    private void Button1_Option_IsModal_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Button1_Option_IsModal.SelectedItem is RadioButton { Content: "true" })
        {
            this._setOwnerWindow = this.Button1_Option_SetOwnerWindow.IsChecked ?? false;
            this._modalOwnerWindowOverride = true;
            this.Button1_Option_SetOwnerWindow.IsChecked = true;
            this.Button1_Option_SetOwnerWindow.IsEnabled = false;
        }
        else
        {
            this.Button1_Option_SetOwnerWindow.IsChecked = this._setOwnerWindow;
            this._modalOwnerWindowOverride = false;
            this.Button1_Option_SetOwnerWindow.IsEnabled = true;
        }
    }

    private void Button1_Option_SetOwnerWindow_Checked(object sender, RoutedEventArgs e)
    {
        if (!this._modalOwnerWindowOverride)
        {
            this._setOwnerWindow = this.Button1_Option_SetOwnerWindow.IsChecked ?? false;
        }
    }

    private void Button1_Option_SetOwnerWindow_Unchecked(object sender, RoutedEventArgs e)
    {
        if (!this._modalOwnerWindowOverride)
        {
            this._setOwnerWindow = this.Button1_Option_SetOwnerWindow.IsChecked ?? false;
        }
    }

    private void Button1_Click(object sender, RoutedEventArgs e)
    {
        RadioButton rb = (RadioButton)Button1_RadioButtons.SelectedItem;
        OverlappedPresenter presenter = rb.Tag switch
        {
            "Create" => OverlappedPresenter.Create(),
            "CreateForToolWindow" => OverlappedPresenter.CreateForToolWindow(),
            "CreateForContextMenu" => OverlappedPresenter.CreateForContextMenu(),
            "CreateForDialog" => OverlappedPresenter.CreateForDialog(),
            _ => throw new NotImplementedException()
        };

        bool hasBorder = presenter.HasBorder;

        switch (this.Button1_Option_HasBorder.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                hasBorder = false;
                break;
            case RadioButton { Content: "true" }:
                hasBorder = true;
                break;
        }

        bool hasTitleBar = presenter.HasTitleBar;

        switch (this.Button1_Option_HasTitleBar.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                hasTitleBar = false;
                break;
            case RadioButton { Content: "true" }:
                hasTitleBar = true;
                break;
        }

        presenter.SetBorderAndTitleBar(hasBorder, hasTitleBar);

        switch (this.Button1_Option_IsAlwaysOnTop.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                presenter.IsAlwaysOnTop = false;
                break;
            case RadioButton { Content: "true" }:
                presenter.IsAlwaysOnTop = true;
                break;
        }

        switch (this.Button1_Option_IsMaximizable.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                presenter.IsMaximizable = false;
                break;
            case RadioButton { Content: "true" }:
                presenter.IsMaximizable = true;
                break;
        }

        switch (this.Button1_Option_IsMinimizable.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                presenter.IsMinimizable = false;
                break;
            case RadioButton { Content: "true" }:
                presenter.IsMinimizable = true;
                break;
        }

        switch (this.Button1_Option_IsModal.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                presenter.IsModal = false;
                break;
            case RadioButton { Content: "true" }:
                presenter.IsModal = true;
                break;
        }

        switch (this.Button1_Option_IsResizable.SelectedItem)
        {
            case RadioButton { Content: "false" }:
                presenter.IsResizable = false;
                break;
            case RadioButton { Content: "true" }:
                presenter.IsResizable = true;
                break;
        }

        bool setOwnerWindow = this.Button1_Option_SetOwnerWindow.IsChecked ?? false;
        AppWindow appWindow = setOwnerWindow ? AppWindow.Create(presenter, this.XamlRoot.ContentIslandEnvironment.AppWindowId) : AppWindow.Create(presenter);

        appWindow.Show(ActivateOnCreateCheckbox.IsChecked ?? false);
    }

	private void Button2_Click(object sender, RoutedEventArgs e)
	{
		CompactOverlayPresenter presenter = CompactOverlayPresenter.Create();

		AppWindow.Create(presenter).Show(ActivateOnCreateCheckbox.IsChecked ?? false);
	}

    private void Button1_RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RadioButton? rb = (RadioButton?)Button1_RadioButtons.SelectedItem;

        if (rb is null)
        {
            Button1_Option_DefaultButton_HasBorder.Content = $"default (null)";
            Button1_Option_DefaultButton_HasTitleBar.Content = $"default (null)";
            Button1_Option_DefaultButton_IsAlwaysOnTop.Content = $"default (null)";
            Button1_Option_DefaultButton_IsMaximizable.Content = $"default (null)";
            Button1_Option_DefaultButton_IsMinimizable.Content = $"default (null)";
            Button1_Option_DefaultButton_IsModal.Content = $"default (null)";
            Button1_Option_DefaultButton_IsResizable.Content = $"default (null)";

            return;
        }

        OverlappedPresenter presenter = rb.Tag switch
        {
            "Create" => OverlappedPresenter.Create(),
            "CreateForToolWindow" => OverlappedPresenter.CreateForToolWindow(),
            "CreateForContextMenu" => OverlappedPresenter.CreateForContextMenu(),
            "CreateForDialog" => OverlappedPresenter.CreateForDialog(),
            _ => throw new NotImplementedException()
        };

        Button1_Option_DefaultButton_HasBorder.Content = $"default ({presenter.HasBorder})";
        Button1_Option_DefaultButton_HasTitleBar.Content = $"default ({presenter.HasTitleBar})";
        Button1_Option_DefaultButton_IsAlwaysOnTop.Content = $"default ({presenter.IsAlwaysOnTop})";
        Button1_Option_DefaultButton_IsMaximizable.Content = $"default ({presenter.IsMaximizable})";
        Button1_Option_DefaultButton_IsMinimizable.Content = $"default ({presenter.IsMinimizable})";
        Button1_Option_DefaultButton_IsModal.Content = $"default ({presenter.IsModal})";
        Button1_Option_DefaultButton_IsResizable.Content = $"default ({presenter.IsResizable})";
    }
}
