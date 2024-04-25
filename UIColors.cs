using System.Globalization;
using System.Text;
using Windows.UI;

namespace Fantastical.Core;

public static class UIColors
{
    // A bright version of the color
    public static Color BrightenedColor(Color color) => color;

    // A fully saturated but dark version of the color
    public static Color SaturatedDarkColor(Color color) => color;

    // A dark version of the color
    public static Color DarkenedColor(Color color) => color;

    // Brighter version for selected today background, in dark mode
    public static Color DarkenedBrighterColor(Color color) => color;

    // A faded version of the color
    public static Color FadedColor(Color color) => color;

    // Brighter version for selected today background, in light mode
    public static Color FadedBrighterColor(Color color) => color;

    // Light event color
    public static Color LightEventColor(Color color) => color;

    // Background event color
    public static Color EventBackgroundColorForDarkMode(Color color, bool darkMode) => color;

    // Selected background event color
    public static Color EventSelectedBackgroundColorForDarkMode(Color color, bool darkMode) => color;

    // Border color for events
    public static Color EventBorderColor(Color color) => color;

    // Event text color
    public static Color EventTextColorForDarkMode(Color color, bool darkMode) => color;

    // High contrast text color for the events
    public static Color EventHighContrastTextColorForDarkMode(Color color, bool darkMode) => color;

    // Calendar selction color
    public static Color CalendarSelectionColor(Color color) => color;

    // Return a gray color based on -[NSColorSpace deviceGrayColorSpace] or [NSColor grayColor] if generating a color from the deviceGrayColorSpace returns nil
    // Cannot be nil
    public static Color SafeGrayColor(Color color) => color;

    /// <summary>
    /// Parses #rgb, #rgba, #rrggbb, #rrggbbaa color codes
    /// </summary>
    public static Color? ParseColor(string colorCode)
    {
        if (string.IsNullOrEmpty(colorCode) || colorCode[0] != '#' || colorCode.Length < 4)
        {
            return default;
        }

        string hexString = colorCode[1..];

        int len = hexString.Length;
        string hex;
        if (len is 3 or 4)
        {
            StringBuilder str = new();
            for (int i = 0; i < len; i++)
            {
                str.Append(hexString[i], 2);
            }

            hex = str.ToString();
        }
        else
        {
            hex = hexString;
        }

        if (int.TryParse(hex, NumberStyles.HexNumber, null, out int number))
        {
            byte a = (byte)((number >> 0x18) & 0xff);
            byte r = (byte)((number >> 0x10) & 0xff);
            byte g = (byte)((number >> 0x8) & 0xff);
            byte b = (byte)(number & 0xff);
            if (a == 0 && hex.Length <= 6)
            {
                a = 0xff;
            }

            return Color.FromArgb(a, r, g, b);
        }

        return null;
    }

    /// <summary>
    /// The hex string representation of the color (#rrggbb, #aarrggbb, #rgb)
    /// </summary>
    public static string ToHex(this Color color, bool compress = false, bool alphaAlways = false)
    {
        bool wantAlpha = alphaAlways || color.A != 0xff;
        string hex = wantAlpha
            ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
            : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        return compress && !wantAlpha && hex[1] == hex[2] && hex[3] == hex[4] && hex[5] == hex[6]
            ? $"#{hex[1]}{hex[3]}{hex[5]}"
            : hex;
    }
}