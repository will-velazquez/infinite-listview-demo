using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace App1;

internal partial class WeatherReportViewModel : ObservableObject
{

    [ObservableProperty]
    private double _currentTemperature;

    [ObservableProperty]
    private double _realFeelTemperature;

    [ObservableProperty]
    private double _highTemperature;

    [ObservableProperty]
    private double _lowTemperature;

    [ObservableProperty]
    private WeatherIcon _weatherStatus;

    [ObservableProperty]
    private string _location = string.Empty;

    [ObservableProperty]
    private string _minuteCastDescription = string.Empty;

    [ObservableProperty]
    private MinuteCastStatus _minuteCastStatus;

    [ObservableProperty]
    private DateTime _sunrise;

    [ObservableProperty]
    private TimeSpan _dayLength;

    [ObservableProperty]
    private WeatherReportPoint[]? _hourlyReport = null;

    [ObservableProperty]
    private WeatherReportDayForecast[]? _forecasts = null;
}
