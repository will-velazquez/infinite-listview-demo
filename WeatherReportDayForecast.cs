using System;

namespace App1;

internal record WeatherReportDayForecast(
    DateTime Date,
    double HighTemperature,
    double LowTemperature,
    WeatherIcon WeatherStatus,
    DateTime Sunrise,
    TimeSpan DayLength);
