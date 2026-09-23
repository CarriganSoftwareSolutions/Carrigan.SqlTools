namespace Carrigan.SqlTools.Expressions;

//IGNORE SPELLING: TRUNC

/// <summary>
/// Date/time parts accepted by SQL Server DATETRUNC.
/// </summary>
public enum DateTruncDateTimePartEnum
{
    Year,
    Quarter,
    Month,
    DayOfYear,
    Day,
    Week,
    IsoWeek,
    Hour,
    Minute,
    Second,
    Millisecond,
    Microsecond
}
