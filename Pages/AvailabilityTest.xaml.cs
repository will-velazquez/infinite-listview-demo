using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace App1.Pages;

internal sealed partial class AvailabilityTest : Page
{
    public readonly static DependencyProperty StartTimeProperty = DependencyProperty.Register(
		nameof(StartTime),
		typeof(double),
		typeof(AvailabilityTest),
		new PropertyMetadata((double)11, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityTest)d).PART_AvailabilityGrid.EventStartDate = DateTime.Parse("03/13/2024").Date.Add(TimeSpan.FromHours((double)e.NewValue)).ToUniversalTime();
		}));

	public double StartTime
	{
		get => (double)this.GetValue(StartTimeProperty);
		set => this.SetValue(StartTimeProperty, value);
	}

	public readonly static DependencyProperty EndTimeProperty = DependencyProperty.Register(
		nameof(EndTime),
		typeof(double),
		typeof(AvailabilityTest),
		new PropertyMetadata((double)12, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityTest)d).PART_AvailabilityGrid.EventEndDate = DateTime.Parse("03/13/2024").Date.Add(TimeSpan.FromHours((double)e.NewValue)).ToUniversalTime();
		}));

	public double EndTime
	{
		get => (double)this.GetValue(EndTimeProperty);
		set => this.SetValue(EndTimeProperty, value);
	}

	public AvailabilityTest()
    {
        this.InitializeComponent();
    }
}
