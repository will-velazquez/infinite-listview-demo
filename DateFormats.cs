using Fantastical.Core;
using System;
using System.Globalization;

namespace Fantastical.App.Models;

/// <summary>
/// Consolidates several date formats for the application, such as `9 PM`, `9:00 PM`, etc
/// </summary>
public static class DateFormats
{
    /// <summary>
    /// Return a short time string, like 9:41 PM, or 9:00 PM, or 21:41 or 21:00
    /// </summary>
    public static string ShortTimeString(DateTime dateTime) => ShortTimeString(dateTime, TimeZoneInfo.Local);

    /// <summary>
    /// Return a short time string, like 9:41 PM, or 9:00 PM
    /// </summary>
    public static string ShortTimeString(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        DateTime timeZoneTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), timeZoneInfo);
        return timeZoneTime.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Return the shortest time string, like 9:41 PM, or 9 PM, or 21:41, or 21
    /// </summary>
    public static string ShortestTimeString(DateTime dateTime) => ShortestTimeString(dateTime, TimeZoneInfo.Local);

    /// <summary>
    /// Return the shortest time string, like 9:41 PM, or 9 PM, or 21:41, or 21
    /// </summary>
    public static string ShortestTimeString(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        DateTime timeZoneTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), timeZoneInfo);

        string format;
        if (timeZoneTime.Minute == 0)
        {
            format = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Replace(":mm", string.Empty).Replace(":m", string.Empty).Trim();
            if (format.Length == 1)
            {
                format = $"%{format}";
            }
        }
        else
        {
            format = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;
        }

        return timeZoneTime.ToString(format, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Return the shortest time string, without any AM/PM, like 9:41, or 9, or 21:41, or 21
    /// </summary>
    public static string ShortestTimeStringWithoutAMPM(DateTime dateTime) => ShortestTimeStringWithoutAMPM(dateTime, TimeZoneInfo.Local);

    /// <summary>
    /// Return the shortest time string, without any AM/PM, like 9:41, or 9, or 21:41, or 21
    /// </summary>
    public static string ShortestTimeStringWithoutAMPM(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        DateTime timeZoneTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), timeZoneInfo);
        string format = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;

        if (timeZoneTime.Minute == 0)
        {
            format = format.Replace(":mm", string.Empty).Replace(":m", string.Empty).Trim();
        }

        format = format.Replace("tt", string.Empty).Trim();

        if (format.Length == 1)
        {
            format = $"%{format}";
        }

        return timeZoneTime.ToString(format, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Return the month/day string, like June 15, or 15 June
    /// </summary>
    public static string MonthDayString(DateTime dateTime) => MonthDayString(dateTime, TimeZoneInfo.Local);

    /// <summary>
    /// Return the month/day string, like June 15, or 15 June
    /// </summary>
    public static string MonthDayString(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        DateTime timeZoneTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), timeZoneInfo);
        return timeZoneTime.ToString("M", CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Return the short month/day string, like Jun 15, or 15 Jun
    /// Notice the abbreviated month name
    /// </summary>
    public static string ShortMonthDayString(DateTime dateTime) => ShortMonthDayString(dateTime, TimeZoneInfo.Local);

    /// <summary>
    /// Return the short month/day string, like Jun 15, or 15 Jun
    /// Notice the abbreviated month name
    /// </summary>
    public static string ShortMonthDayString(DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        string shortMonthDayPattern = CultureInfo.CurrentUICulture.DateTimeFormat.MonthDayPattern.Replace("MMMM", "MMM");
        if (shortMonthDayPattern.Length == 1)
        {
            shortMonthDayPattern = $"%{shortMonthDayPattern}";
        }

        DateTime timeZoneTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), timeZoneInfo);
        return timeZoneTime.ToString(shortMonthDayPattern, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Return the short month/day string, like Jun 15, or 15 Jun
    /// Notice the abbreviated month name
    /// </summary>
    public static string ShortMonthDayString(DateOnly dateOnly) => ShortMonthDayString(dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime(), TimeZoneInfo.Local);

    /// <summary>
    /// Return the short month/day string, like Jun 15, or 15 Jun
    /// Notice the abbreviated month name
    /// </summary>
    public static string ShortMonthDayString(DateOnly dateOnly, TimeZoneInfo timeZoneInfo) => ShortMonthDayString(dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime(), timeZoneInfo);

    /// <summary>
    /// Return the shortest UTC Offset string for timespan, including only hours, and if necessary, minutes
    /// eg +5, or -5:30, or +0
    /// </summary>
    public static string ShortestUtcOffsetString(TimeSpan timeSpan)
    {
        string format;
        if (timeSpan.Minutes == 0)
        {
            format = "%h";
        }
        else
        {
            format = "h\\:mm";
        }

        format = timeSpan < TimeSpan.Zero ? $"\\-{format}" : $"\\+{format}";

        return timeSpan.ToString(format);
    }

    /// <summary>
    /// Returns the localized day name of dateTime, eg "Tuesday"
    /// </summary>
    public static string DayName(DayOfWeek dayOfWeek) => CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(dayOfWeek);

    /// <summary>
    /// Returns the localized day name of dateTime, eg "Tuesday"
    /// </summary>
    public static string DayName(DateTime dateTime) => dateTime.ToLocalTime().ToString("dddd", CultureInfo.CurrentUICulture);

    /// <summary>
    /// Returns the localized short day name of dateTime, eg "Tue"
    /// </summary>
    public static string ShortDayName(DayOfWeek dayOfWeek) => CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);


    /// <summary>
    /// Returns the localized short day name of dateTime, eg "Tue"
    /// </summary>
    public static string ShortDayName(DateTime dateTime) => dateTime.ToLocalTime().ToString("ddd", CultureInfo.CurrentUICulture);

    /// <summary>
    /// Returns the localized short day name of dateTime, eg "Tu"
    /// TODO WV - This is actually returning "T" or "S", not "Tu" or "Su"
    /// </summary>
    public static string ShortestDayName(DayOfWeek dayOfWeek) => CultureInfo.CurrentUICulture.DateTimeFormat.GetShortestDayName(dayOfWeek);

    /// <summary>
    /// Returns the localized short day name of dateTime, eg "Tu"
    /// TODO WV - This is actually returning "T" or "S", not "Tu" or "Su"
    /// </summary>
    public static string ShortestDayName(DateTime dateTime) => CultureInfo.CurrentUICulture.DateTimeFormat.GetShortestDayName(dateTime.ToLocalTime().DayOfWeek);

    /// <summary>
    /// Returns the localized month name of dateTime, eg "November"
    /// </summary>
    public static string MonthName(int month) => CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(month);

    /// <summary>
    /// Returns the localized month name of dateTime, eg "November"
    /// </summary>
    public static string MonthName(DateTime dateTime) => dateTime.ToLocalTime().ToString("MMMM", CultureInfo.CurrentUICulture);

    /// <summary>
    /// Returns the localized short month name of dateTime, eg "Nov"
    /// </summary>
    public static string ShortMonthName(DateTime dateTime) => dateTime.ToLocalTime().ToString("MMM", CultureInfo.CurrentUICulture);
}
