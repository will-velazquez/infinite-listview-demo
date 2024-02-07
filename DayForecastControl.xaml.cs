using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace App1;

internal sealed partial class DayForecastControl : UserControl
{
    public readonly static DependencyProperty DayForecastProperty =
        DependencyProperty.Register(
            nameof(DayForecast),
            typeof(WeatherReportDayForecast),
            typeof(DayForecastControl),
            new PropertyMetadata(default(WeatherReportDayForecast), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                
            }));

    public WeatherReportDayForecast DayForecast
    {
        get => (WeatherReportDayForecast)this.GetValue(DayForecastProperty);
        set => this.SetValue(DayForecastProperty, value);
    }

    public readonly static DependencyProperty TodayBrushProperty =
        DependencyProperty.Register(
            nameof(TodayBrush),
            typeof(Brush),
            typeof(DayForecastControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Blue), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {

            }));

    public Brush TodayBrush
    {
        get => (Brush)this.GetValue(TodayBrushProperty);
        set => this.SetValue(TodayBrushProperty, value);
    }

    public readonly static DependencyProperty SunsetBrushProperty =
        DependencyProperty.Register(
            nameof(SunsetBrush),
            typeof(Brush),
            typeof(DayForecastControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Orange), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {

            }));

    public Brush SunsetBrush
    {
        get => (Brush)this.GetValue(SunsetBrushProperty);
        set => this.SetValue(SunsetBrushProperty, value);
    }

    public DayForecastControl()
    {
        this.InitializeComponent();
    }

    private string GetDayNameString(DateTime date) => date.Date.ToUniversalTime() == DateTime.Today.Date.ToUniversalTime()
        // TODO i8n
        ? "TODAY"
        : date.ToLocalTime().ToString("ddd").ToUpperInvariant();

    private Style GetTodayStyle(DateTime date, Style todayStyle, Style notTodayStyle) => date.Date.ToUniversalTime() == DateTime.Today.Date.ToUniversalTime()
        ? todayStyle
        : notTodayStyle;

    // TODO WV  i8n
    private string GetDateString(DateTime date) => date.ToString("dd MMM");

    private string GetSunriseString(DateTime sunrise) => sunrise.ToLocalTime().ToShortTimeString();

    private string GetSunsetString(DateTime sunrise, TimeSpan dayLength) => sunrise.Add(dayLength).ToLocalTime().ToShortTimeString();
}
