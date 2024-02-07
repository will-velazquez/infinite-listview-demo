using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace App1;

internal sealed partial class OtherCheckbox : UserControl
{
	public static DependencyProperty IsCheckedProperty = DependencyProperty.Register(
		nameof(IsChecked),
		typeof(bool),
		typeof(OtherCheckbox),
		new PropertyMetadata(false, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) =>
		{
			(dependencyObject as OtherCheckbox ?? throw new InvalidOperationException()).UpdateIsChecked();
		}));

	public bool IsChecked
	{
		get => (bool)GetValue(IsCheckedProperty);
		set => SetValue(IsCheckedProperty, value);
	}

	public OtherCheckbox()
	{
		this.InitializeComponent();

		this.Loaded += OtherCheckbox_Loaded;
	}

	private void OtherCheckbox_Loaded(object sender, RoutedEventArgs e)
	{
		this.Unloaded += OtherCheckbox_Unloaded;

		this.IsEnabledChanged += OtherCheckbox_IsEnabledChanged;

		this.UpdateIsEnabled();
		this.UpdateIsChecked();


		this.Loaded -= OtherCheckbox_Loaded;
	}

	private void OtherCheckbox_Unloaded(object sender, RoutedEventArgs e)
	{
		this.Loaded += OtherCheckbox_Loaded;

		this.IsEnabledChanged -= this.OtherCheckbox_IsEnabledChanged;

		this.Unloaded -= this.OtherCheckbox_Unloaded;
	}

	private void OtherCheckbox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		this.UpdateIsEnabled();
	}

	private void UpdateIsEnabled()
	{
		// I just need to specify "Normal" or "Disabled", not what group they belong to
		VisualStateManager.GoToState(this, this.IsEnabled ? nameof(Normal) : nameof(Disabled), true);
	}

	private void UpdateIsChecked()
	{
		VisualStateManager.GoToState(this, this.IsChecked ? nameof(Checked) : nameof(Unchecked), true);
	}

	private void UserControl_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
	{
		this.IsChecked = !this.IsChecked;
	}
}
