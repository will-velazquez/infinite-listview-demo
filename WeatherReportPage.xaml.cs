using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace App1;
public sealed partial class WeatherReportPage : Page
{
    private WeatherReportViewModel ViewModel;

    public WeatherReportPage()
    {
        this.ViewModel = new WeatherReportViewModel()
        {
            CurrentTemperature = 74,
            RealFeelTemperature = 74,
            HighTemperature = 77,
            LowTemperature = 68,
            Location = "Orlando, FL",
            MinuteCastDescription = "Rain starting in 1 min",
            MinuteCastStatus = MinuteCastStatus.Rain,
            Sunrise = DateTime.Parse("6:47 AM"),
            DayLength = DateTime.Parse("5:33 PM") - DateTime.Parse("6:47 AM"),
            HourlyReport = new WeatherReportPoint[]
            {
                new (DateTime.Parse("11:00 AM"),  WeatherIcon.Rain, 74),
                new (DateTime.Parse("12:00 PM"), WeatherIcon.Rain, 75),
                new (DateTime.Parse("3:00 PM"), WeatherIcon.Rain, 75),
                new (DateTime.Parse("6:00 PM"), WeatherIcon.Rain, 72),
                new (DateTime.Parse("9:00 PM"), WeatherIcon.Rain, 71),
                new (DateTime.Parse("12:00 PM").AddHours(12), WeatherIcon.Rain, 70),
            },
            Forecasts = new WeatherReportDayForecast[]
            {
                new WeatherReportDayForecast(DateTime.Today.AddDays(0), 77, 68, WeatherIcon.Rain, DateTime.Parse("6:47 AM"), DateTime.Parse("5:33 PM") - DateTime.Parse("6:47 AM")),
                new WeatherReportDayForecast(DateTime.Today.AddDays(1), 79, 68, WeatherIcon.Thunderstorms, DateTime.Parse("6:48 AM"), DateTime.Parse("5:32 PM") - DateTime.Parse("6:48 AM")),
                new WeatherReportDayForecast(DateTime.Today.AddDays(2), 75, 68, WeatherIcon.Rain, DateTime.Parse("6:49 AM"), DateTime.Parse("5:32 PM") - DateTime.Parse("6:49 AM")),
                new WeatherReportDayForecast(DateTime.Today.AddDays(3), 80, 66, WeatherIcon.PartlyCloudyDay, DateTime.Parse("6:49 AM"), DateTime.Parse("5:31 PM") - DateTime.Parse("6:49 AM")),
                new WeatherReportDayForecast(DateTime.Today.AddDays(4), 79, 66, WeatherIcon.PartlyCloudyDay, DateTime.Parse("6:50 AM"), DateTime.Parse("5:31 PM") - DateTime.Parse("6:50 AM")),
                new WeatherReportDayForecast(DateTime.Today.AddDays(5), 77, 60, WeatherIcon.Rain, DateTime.Parse("6:47 AM"), DateTime.Parse("5:33 PM") - DateTime.Parse("6:47 AM")),
            }
        };

        this.InitializeComponent();
    }

    private Uri? GetWeatherIcon(WeatherIcon weatherIcon) => new Uri($"ms-appx:///Assets/Weather/Partly-cloudy-light.svg.png");

    private Visibility GetHourlyReportVisibility(WeatherReportPoint[]? hourlyReport) =>
        hourlyReport is null ? Visibility.Collapsed : Visibility.Visible;

    private Uri? GetMinuteCastIconSource(MinuteCastStatus minuteCastStatus)
    {
        string? kindName = minuteCastStatus switch
        {
            MinuteCastStatus.Ice => "MinuteCastIce",
            MinuteCastStatus.Rain => "MinuteCastRain",
            MinuteCastStatus.Sleet => "MinuteCastSleet",
            MinuteCastStatus.Snow => "MinuteCastSnow",
            _ => null
        };

        if (kindName is null)
        {
            return null;
        }

        return new Uri($"ms-appx:///Assets/Weather/{kindName}.svg.png");
    }
}

