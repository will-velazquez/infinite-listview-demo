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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({hasBorder})";
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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({hasTitleBar})";
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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({presenter.IsAlwaysOnTop})";
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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({presenter.IsMaximizable})";
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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({presenter.IsMinimizable})";
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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({presenter.IsModal})";
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
			case RadioButton defaultButton:
				defaultButton.Content = $"default ({presenter.IsResizable})";
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
}
