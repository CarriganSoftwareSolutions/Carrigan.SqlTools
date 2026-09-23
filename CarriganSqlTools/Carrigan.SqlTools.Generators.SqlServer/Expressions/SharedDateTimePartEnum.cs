namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Date/time parts supported by all SQL Server date/time expressions in this library that accept a date-part token.
/// </summary>
public enum SharedDateTimePartEnum
{
    Year,
    Quarter,
    Month,
    DayOfYear,
    Day,
    Week,
    Hour,
    Minute,
    Second,
    Millisecond,
    Microsecond
}
