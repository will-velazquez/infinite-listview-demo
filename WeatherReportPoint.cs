using System;

namespace App1;

internal record struct WeatherReportPoint(DateTime Time, WeatherIcon Forecast, double Temperature);