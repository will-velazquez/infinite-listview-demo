using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace App1;

internal class MyCheckbox : Control
{
	// States
	private const string NormalState = "Normal";
	private const string DisabledState = "Disabled";
	private const string CommonStates = "CommonStates";

	public static DependencyProperty IsCheckedProperty = DependencyProperty.Register(
		nameof(IsChecked),
		typeof(bool),
		typeof(MyCheckbox),
		new PropertyMetadata(false, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) =>
		{

		}));

	public bool IsChecked
	{
		get => (bool)GetValue(IsCheckedProperty);
		set => SetValue(IsCheckedProperty, value);
	}

	public MyCheckbox()
	{

	}

	protected override void OnApplyTemplate()
	{
		this.Unloaded += this.MyCheckbox_Unloaded;
		this.IsEnabledChanged += MyCheckbox_IsEnabledChanged;
		base.OnApplyTemplate();
	}

	private void MyCheckbox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		VisualStateManager.GoToState(this, this.IsEnabled ? NormalState : DisabledState, true);
	}

	private void MyCheckbox_Unloaded(object sender, RoutedEventArgs e)
	{
		
	}
}
