using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Fantastical.Core;

public enum DateTimeFormatTokenKind
{
    Day,
    DayName,
    FractionSeconds,
    NonZeroFractionSeconds,
    Era,
    Hours12,
    Hours24,
    TimeZone,
    Minutes,
    Month,
    MonthName,
    Seconds,
    AMPM,
    Year,
    HoursOffset,
    TimeSeparator,
    DateSeparator,
    Literal,
}

/// <summary>
/// Parse a custom date and time format string into a series of tokens
/// Each token is classified according to its part kind (eg hours, months)
/// See https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
/// This is useful in building custom controls that respect the user's regional formatting
/// </summary>
[DebuggerDisplay("{Kind} | {FormatString}")]
public sealed record DateTimeFormatToken
{
    public DateTimeFormatTokenKind Kind { get; private init; }
    public string FormatString { get; private init; }

    /// <summary>
    /// Produces a string suitable for giving to DateTime.ToString or DateOnly.ToString
    /// Single literal characters, like a space, are not allowed
    /// </summary>
    private string SingleFormatString => this.Kind == DateTimeFormatTokenKind.Literal && this.FormatString.Length == 1 ? $"\\{this.FormatString}" : this.FormatString;

    private DateTimeFormatToken(DateTimeFormatTokenKind kind, string formatString)
    {
        this.Kind = kind;
        this.FormatString = formatString;
    }

    public static readonly ReadOnlyDictionary<char, DateTimeFormatTokenKind> CharToSpecifierKind = new Dictionary<char, DateTimeFormatTokenKind>()
    {
        ['d'] = DateTimeFormatTokenKind.Day,
        ['f'] = DateTimeFormatTokenKind.FractionSeconds,
        ['F'] = DateTimeFormatTokenKind.NonZeroFractionSeconds,
        ['g'] = DateTimeFormatTokenKind.Era,
        ['h'] = DateTimeFormatTokenKind.Hours12,
        ['H'] = DateTimeFormatTokenKind.Hours24,
        ['K'] = DateTimeFormatTokenKind.TimeZone,
        ['m'] = DateTimeFormatTokenKind.Minutes,
        ['M'] = DateTimeFormatTokenKind.Month,
        ['s'] = DateTimeFormatTokenKind.Seconds,
        ['t'] = DateTimeFormatTokenKind.AMPM,
        ['y'] = DateTimeFormatTokenKind.Year,
        ['z'] = DateTimeFormatTokenKind.HoursOffset,
        [':'] = DateTimeFormatTokenKind.TimeSeparator,
        ['/'] = DateTimeFormatTokenKind.DateSeparator
    }.AsReadOnly();

    public static string FormatTokens(DateTimeFormatToken[] tokens, DateOnly dateOnly)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < tokens.Length; ++i)
        {
            DateTimeFormatToken token = tokens[i];

            sb.Append(token.Format(dateOnly));
        }

        return sb.ToString();
    }

    public static string FormatTokens(DateTimeFormatToken[] tokens, DateOnly dateOnly, IFormatProvider? formatProvider)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < tokens.Length; ++i)
        {
            DateTimeFormatToken token = tokens[i];

            sb.Append(token.Format(dateOnly, formatProvider));
        }

        return sb.ToString();
    }

    public static string FormatTokens(DateTimeFormatToken[] tokens, DateTime dateTime)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < tokens.Length; ++i)
        {
            DateTimeFormatToken token = tokens[i];

            sb.Append(token.Format(dateTime));
        }

        return sb.ToString();
    }

    public static string FormatTokens(DateTimeFormatToken[] tokens, DateTime dateTime, IFormatProvider? formatProvider)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < tokens.Length; ++i)
        {
            DateTimeFormatToken token = tokens[i];

            sb.Append(token.Format(dateTime, formatProvider));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Build a custom date & time format string representing each token in tokens
    /// </summary>
    public static string BuildFormatString(DateTimeFormatToken[] tokens)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < tokens.Length; ++i)
        {
            DateTimeFormatToken token = tokens[i];

            sb.Append(token.FormatString);
        }

        return sb.ToString();
    }

    public static DateTimeFormatToken[] ParseTokens(string formatString)
    {
        if (formatString.Length == 0)
        {
            return new DateTimeFormatToken[0];
        }

        List<DateTimeFormatToken> tokens = new List<DateTimeFormatToken>();
        StringBuilder token = new StringBuilder();
        int datePartMaxLen;
        DateTimeFormatTokenKind datePartKind;
        int i = 0;
        char c = formatString[i++];

    parseToken:
        switch (c)
        {
            case 'd':
                datePartMaxLen = 4;
                goto parseDatePart;
            case 'f':
            case 'F':
                datePartMaxLen = 7;
                goto parseDatePart;
            case 'g':
            case 'h':
            case 'H':
                datePartMaxLen = 2;
                goto parseDatePart;
            case 'K':
                datePartMaxLen = 1;
                goto parseDatePart;
            case 'm':
                datePartMaxLen = 2;
                goto parseDatePart;
            case 'M':
                datePartMaxLen = 4;
                goto parseDatePart;
            case 's':
                datePartMaxLen = 2;
                goto parseDatePart;
            case 't':
                datePartMaxLen = 2;
                goto parseDatePart;
            case 'y':
                datePartMaxLen = 5;
                goto parseDatePart;
            case 'z':
                datePartMaxLen = 3;
                goto parseDatePart;
            case ':':
            case '/':
                datePartMaxLen = 1;
                goto parseDatePart;
            case '\'' or '"':
                goto parseStringLiteral;
            case '%':
                goto parseCustomFormatSpecifier;
            case '\\':
                goto parseSingleEscape;
            default:
                token.Append(c);

                if (i >= formatString.Length)
                {
                    tokens.Add(new DateTimeFormatToken(DateTimeFormatTokenKind.Literal, token.ToString()));
                    token.Clear();

                    goto finish;
                }

                c = formatString[i++];

                goto parseToken;
        }

    parseDatePart:
        if (token.Length > 0)
        {
            tokens.Add(new DateTimeFormatToken(DateTimeFormatTokenKind.Literal, token.ToString()));
            token.Clear();
        }

        if (!CharToSpecifierKind.TryGetValue(c, out datePartKind))
        {
            throw new Exception($"Malformed format string '{formatString}', expected custom format specifier at {i}");
        }

    parseDatePartLoop:
        token.Append(c);

        if (i >= formatString.Length)
        {
            tokens.Add(MakeDatePartToken(datePartKind, token.ToString()));
            token.Clear();

            goto finish;
        }

        if (token.Length >= datePartMaxLen || formatString[i] != c)
        {
            tokens.Add(MakeDatePartToken(datePartKind, token.ToString()));
            token.Clear();
            c = formatString[i++];

            goto parseToken;
        }

        ++i;
        goto parseDatePartLoop;

    parseStringLiteral:
        if (token.Length > 0)
        {
            tokens.Add(new DateTimeFormatToken(DateTimeFormatTokenKind.Literal, token.ToString()));
            token.Clear();
        }

        token.Append(c);

        if (i >= formatString.Length)
        {
            throw new Exception($"Malformed format string '{formatString}', expected closing {c} at position {i}");
        }

    parseStringLiteralLoop:

        if (formatString[i] == c)
        {
            token.Append(c);
            tokens.Add(new DateTimeFormatToken(DateTimeFormatTokenKind.Literal, token.ToString()));
            token.Clear();

            ++i;

            if (i >= formatString.Length)
            {
                goto finish;
            }

            c = formatString[i++];

            goto parseToken;
        }

        token.Append(formatString[i++]);

        if (i >= formatString.Length)
        {
            throw new Exception($"Malformed format string '{formatString}', expected closing {c} at position {i}");
        }

        goto parseStringLiteralLoop;

    parseCustomFormatSpecifier:
        if (token.Length > 0)
        {
            tokens.Add(new DateTimeFormatToken(DateTimeFormatTokenKind.Literal, token.ToString()));
            token.Clear();
        }

        if (i >= formatString.Length)
        {
            throw new Exception($"Malformed format string '{formatString}', expected custom format specifier after % at {i}");
        }

        c = formatString[i++];

        if (!CharToSpecifierKind.TryGetValue(c, out datePartKind))
        {
            throw new Exception($"Malformed format string '{formatString}', expected custom format specifier after % at {i}");
        }

        tokens.Add(new DateTimeFormatToken(datePartKind, $"%{c}"));

    parseSingleEscape:
        if (i >= formatString.Length)
        {
            throw new Exception($"Malformed format string '{formatString}', expected character after escape '\\' at {i}");
        }

        token.Append(c);
        token.Append(formatString[i++]);
        c = formatString[i];

        goto parseToken;

    finish:
        return tokens.ToArray();

        static DateTimeFormatToken MakeDatePartToken(DateTimeFormatTokenKind kind, string token)
        {
            DateTimeFormatTokenKind tokenKind = kind;

            if (kind is DateTimeFormatTokenKind.Day)
            {
                if (token is "ddd" or "dddd")
                {
                    tokenKind = DateTimeFormatTokenKind.DayName;
                }
            }
            else if (kind is DateTimeFormatTokenKind.Month)
            {
                if (token is "MMM" or "MMMM")
                {
                    tokenKind = DateTimeFormatTokenKind.MonthName;
                }
            }

            string formatString = token.Length == 1 ? $"%{token}" : token.ToString();

            return new DateTimeFormatToken(tokenKind, formatString);
        }
    }

    /// <summary>
    /// Return a shortened token when possible, such as going from 2 digit day numbers to 1
    /// </summary>
    public DateTimeFormatToken Shortened()
    {
        if (this.Kind == DateTimeFormatTokenKind.Literal
            || this.FormatString.Length < 2
            || this.FormatString[0] == '%'
            || (this.Kind == DateTimeFormatTokenKind.DayName && this.FormatString.Length == 3)
            || (this.Kind == DateTimeFormatTokenKind.MonthName && this.FormatString.Length == 3))
        {
            return this;
        }

        string shortenedToken = this.FormatString[..^1];

        if (shortenedToken.Length == 1
            && CharToSpecifierKind.ContainsKey(shortenedToken[0]))
        {
            shortenedToken = $"%{shortenedToken}";
        }

        return this with { FormatString = shortenedToken };
    }

    public string Format(DateOnly dateOnly)
    {
        return dateOnly.ToString(this.SingleFormatString);
    }

    public string Format(DateOnly dateOnly, IFormatProvider? formatProvider)
    {
        return dateOnly.ToString(this.SingleFormatString, formatProvider);
    }

    public string Format(DateTime dateTime)
    {
        return dateTime.ToString(this.SingleFormatString);
    }

    public string Format(DateTime dateTime, IFormatProvider? formatProvider)
    {
        return dateTime.ToString(this.SingleFormatString, formatProvider);
    }
}
