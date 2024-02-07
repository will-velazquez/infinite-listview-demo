using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;
using Windows.UI;

namespace App1;

public sealed partial class SunPath : UserControl
{
    public readonly static DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register(
            nameof(CurrentTime),
            typeof(DateTime),
            typeof(SunPath),
            new PropertyMetadata(default(DateTime), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateTime();
            }));

    public DateTime CurrentTime
    {
        get => (DateTime)this.GetValue(CurrentTimeProperty);
        set => this.SetValue(CurrentTimeProperty, value);
    }

    public readonly static DependencyProperty SunriseProperty =
        DependencyProperty.Register(
            nameof(Sunrise),
            typeof(DateTime),
            typeof(SunPath),
            new PropertyMetadata(default(DateTime), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateSunrise();
            }));

    public DateTime Sunrise
    {
        get => (DateTime)this.GetValue(SunriseProperty);
        set => this.SetValue(SunriseProperty, value);
    }

    public readonly static DependencyProperty DayLengthProperty =
        DependencyProperty.Register(
            nameof(DayLength),
            typeof(TimeSpan),
            typeof(SunPath),
            new PropertyMetadata(default(TimeSpan), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateDayLength();
            }));

    public TimeSpan DayLength
    {
        get => (TimeSpan)this.GetValue(DayLengthProperty);
        set => this.SetValue(DayLengthProperty, value);
    }

    public readonly static DependencyProperty SunsetProperty =
        DependencyProperty.Register(
            nameof(Sunset),
            typeof(DateTime),
            typeof(SunPath),
            new PropertyMetadata(default(DateTime)));

    public DateTime Sunset
    {
        get => (DateTime)this.GetValue(SunsetProperty);
        private set => this.SetValue(SunsetProperty, value);
    }

    public readonly static DependencyProperty ArcBeforeBrushProperty =
        DependencyProperty.Register(
            nameof(ArcBeforeBrush),
            typeof(Brush),
            typeof(SunPath),
            new PropertyMetadata(default(Brush), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateTime();
            }));

    public Brush ArcBeforeBrush
    {
        get => (Brush)this.GetValue(ArcBeforeBrushProperty);
        set => this.SetValue(ArcBeforeBrushProperty, value);
    }

    public readonly static DependencyProperty ArcAfterBrushProperty =
        DependencyProperty.Register(
            nameof(ArcAfterBrush),
            typeof(Brush),
            typeof(SunPath),
            new PropertyMetadata(default(Brush), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateTime();
            }));

    public Brush ArcAfterBrush
    {
        get => (Brush)this.GetValue(ArcAfterBrushProperty);
        set => this.SetValue(ArcAfterBrushProperty, value);
    }

    public readonly static DependencyProperty SunriseBrushProperty =
        DependencyProperty.Register(
            nameof(SunriseBrush),
            typeof(Brush),
            typeof(SunPath),
            new PropertyMetadata(default(Brush), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateTime();
            }));

    public Brush SunriseBrush
    {
        get => (Brush)this.GetValue(SunriseBrushProperty);
        set => this.SetValue(SunriseBrushProperty, value);
    }

    public readonly static DependencyProperty SunsetBrushProperty =
        DependencyProperty.Register(
            nameof(SunsetBrush),
            typeof(Brush),
            typeof(SunPath),
            new PropertyMetadata(default(Brush), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as SunPath ?? throw new InvalidOperationException()).UpdateTime();
            }));

    public Brush SunsetBrush
    {
        get => (Brush)this.GetValue(SunsetBrushProperty);
        set => this.SetValue(SunsetBrushProperty, value);
    }

    public SunPath()
    {
        this.CurrentTime = DateTime.Now;
        this.Sunrise = DateTime.Parse("6:00 AM");
        this.DayLength = TimeSpan.FromHours(14);
        this.ArcBeforeBrush = new SolidColorBrush(Colors.Gray);
        this.ArcAfterBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x8E, 0x00));
        this.SunriseBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFB, 0xC8, 0x28));
        this.SunsetBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x8E, 0x00));

        this.InitializeComponent();
    }

    private void CalculateSunset()
    {
        this.Sunset = this.Sunrise + this.DayLength;
    }

    private void UpdateSunrise()
    {
        this.CalculateSunset();
        this.UpdateTime();
    }

    private void UpdateDayLength()
    {
        this.CalculateSunset();
        this.UpdateTime();
    }

    private void UpdateTime()
    {
        if (!this.IsLoaded)
        {
            return;
        }

        double currentTimePerc = Math.Max(0, Math.Min(1, (CurrentTime - Sunrise) / DayLength));

        double gridWidth = this.SunPathGrid.ActualWidth;
        double gridHeight = this.SunPathGrid.ActualHeight;
        this.ArcBeforeClipGeometry.Rect = new Rect(0, 0, Math.Max(0, gridWidth * currentTimePerc - (this.SunIcon.ActualWidth / 1.5)), gridHeight);
        this.ArcAfterClipGeometry.Rect = new Rect(gridWidth * currentTimePerc + (this.SunIcon.ActualWidth / 1.5), 0, Math.Max(0, gridWidth * (1 - currentTimePerc)), gridHeight);

        double sunXPerc = currentTimePerc;
        double sunX = (sunXPerc * gridWidth) - (this.SunIcon.ActualWidth / 2);
        double sunYPerc = (currentTimePerc - .5) * (currentTimePerc - .5) / 0.25;
        double sunY = (sunYPerc * gridHeight) - (this.SunIcon.ActualHeight / 2);

        Canvas.SetLeft(this.SunIcon, sunX);
        Canvas.SetTop(this.SunIcon, sunY);
    }

    private void SunPathGrid_SizeChanged(object sender, SizeChangedEventArgs e) => this.UpdateTime();
}

