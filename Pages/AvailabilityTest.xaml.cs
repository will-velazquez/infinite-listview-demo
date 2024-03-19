using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace App1.Pages;

internal sealed partial class AvailabilityTest : Page
{
	public readonly static DependencyProperty DateProperty = DependencyProperty.Register(
		nameof(Date),
		typeof(DateTimeOffset),
		typeof(AvailabilityTest),
		new PropertyMetadata(new DateTimeOffset(DateTime.Parse("03/13/2024")), (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityTest)d).PART_AvailabilityGrid.Date = DateOnly.FromDateTime(((DateTimeOffset)e.NewValue).Date).DayNumber;
		}));

	public DateTimeOffset Date
	{
		get => (DateTimeOffset)this.GetValue(DateProperty);
		set => this.SetValue(DateProperty, value);
	}


	public readonly static DependencyProperty EventStartTimeProperty = DependencyProperty.Register(
		nameof(EventStartTime),
		typeof(double),
		typeof(AvailabilityTest),
		new PropertyMetadata((double)11, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityTest)d).PART_AvailabilityGrid.EventStartDate = DateTime.Parse("03/13/2024").Date.Add(TimeSpan.FromHours((double)e.NewValue)).ToUniversalTime();
		}));

	public double EventStartTime
	{
		get => (double)this.GetValue(EventStartTimeProperty);
		set => this.SetValue(EventStartTimeProperty, value);
	}

	public readonly static DependencyProperty EventEndTimeProperty = DependencyProperty.Register(
		nameof(EventEndTime),
		typeof(double),
		typeof(AvailabilityTest),
		new PropertyMetadata((double)12, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityTest)d).PART_AvailabilityGrid.EventEndDate = DateTime.Parse("03/13/2024").Date.Add(TimeSpan.FromHours((double)e.NewValue)).ToUniversalTime();
		}));

	public double EventEndTime
	{
		get => (double)this.GetValue(EventEndTimeProperty);
		set => this.SetValue(EventEndTimeProperty, value);
	}

	public AvailabilityTest()
    {
        this.InitializeComponent();
    }
}
