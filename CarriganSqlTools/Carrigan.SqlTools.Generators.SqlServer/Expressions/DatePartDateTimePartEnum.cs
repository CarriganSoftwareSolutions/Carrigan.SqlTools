namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Date/time parts accepted by SQL Server DATEPART.
/// </summary>
public enum DatePartDateTimePartEnum
{
    Year,
    Quarter,
    Month,
    DayOfYear,
    Day,
    Week,
    Weekday,
    Hour,
    Minute,
    Second,
    Millisecond,
    Microsecond,
    Nanosecond,
    IsoWeek,
    TimezoneOffset
}
