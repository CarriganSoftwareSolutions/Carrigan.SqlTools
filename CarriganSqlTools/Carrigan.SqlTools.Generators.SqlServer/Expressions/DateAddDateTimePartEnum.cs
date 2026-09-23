namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Date/time parts accepted by SQL Server DATEADD.
/// </summary>
public enum DateAddDateTimePartEnum
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
    Nanosecond
}
