namespace Carrigan.SqlTools.Expressions;

//IGNORE SPELLING: TRUNC

/// <summary>
/// Date/time precisions accepted by PostgreSQL DATE_TRUNC.
/// </summary>
public enum DateTruncDateTimePartEnum
{
    Microseconds,
    Milliseconds,
    Second,
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Quarter,
    Year,
    Decade,
    Century,
    Millennium
}
