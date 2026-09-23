namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Date/time parts shared by PostgreSQL EXTRACT, DATE_TRUNC, and MAKE_INTERVAL in this library.
/// </summary>
public enum SharedDateTimePartEnum
{
    Year,
    Month,
    Week,
    Day,
    Hour,
    Minute,
    Second
}
