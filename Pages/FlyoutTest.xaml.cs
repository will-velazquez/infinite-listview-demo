using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using WinRT.Interop;

#nullable enable

namespace App1.Pages;

public sealed partial class FlyoutTest : Page
{
	public readonly static DependencyProperty HeaderProperty = DependencyProperty.Register(
		nameof(Header),
		typeof(UIElement),
		typeof(_BlankPage),
		new PropertyMetadata(null, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
		{
		}));

	public UIElement Header
	{
		get => (UIElement)this.GetValue(HeaderProperty);
		set => this.SetValue(HeaderProperty, value);
	}

	public FlyoutTest()
    {
        this.InitializeComponent();
    }
}
