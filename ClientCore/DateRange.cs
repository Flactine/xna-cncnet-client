#nullable enable
using System;

namespace ClientCore;

/// <summary>
/// A range of dates within a year that ignores the year itself, so that it can be used
/// for checking whether the current date matches a recurring date, such as a holiday.
/// The range may wrap around the new year (for example 12-31 ~ 01-02).
/// </summary>
public readonly record struct DateRange(int StartMonth, int StartDay, int EndMonth, int EndDay)
{
    /// <summary>
    /// Parses a date range from a string in either "MM-DD" or "MM-DD~MM-DD" format.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="range">The parsed date range.</param>
    /// <returns>True if the string was parsed successfully, otherwise false.</returns>
    public static bool TryParse(string? value, out DateRange range)
    {
        range = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        string[] parts = value.Split('~');

        if (parts.Length > 2)
            return false;

        if (!TryParseDate(parts[0], out int startMonth, out int startDay))
            return false;

        int endMonth = startMonth;
        int endDay = startDay;

        if (parts.Length == 2 && !TryParseDate(parts[1], out endMonth, out endDay))
            return false;

        range = new DateRange(startMonth, startDay, endMonth, endDay);
        return true;
    }

    /// <summary>
    /// Checks whether the given date falls within this date range.
    /// </summary>
    public bool Contains(DateTime date)
    {
        int dateValue = date.Month * 100 + date.Day;
        int startValue = StartMonth * 100 + StartDay;
        int endValue = EndMonth * 100 + EndDay;

        if (startValue <= endValue)
        {
            // A regular range that doesn't wrap around the new year, e.g. 12-24 ~ 12-26.
            return dateValue >= startValue && dateValue <= endValue;
        }

        // A range that wraps around the new year, e.g. 12-31 ~ 01-02.
        return dateValue >= startValue || dateValue <= endValue;
    }

    private static bool TryParseDate(string value, out int month, out int day)
    {
        month = 0;
        day = 0;

        string[] parts = value.Trim().Split('-');

        return parts.Length == 2 &&
            int.TryParse(parts[0], out month) &&
            int.TryParse(parts[1], out day) &&
            month >= 1 && month <= 12 &&
            day >= 1 && day <= 31;
    }
}
