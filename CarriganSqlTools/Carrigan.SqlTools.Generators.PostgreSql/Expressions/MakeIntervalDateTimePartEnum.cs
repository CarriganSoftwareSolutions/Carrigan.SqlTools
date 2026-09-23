namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Named interval components accepted by PostgreSQL MAKE_INTERVAL.
/// </summary>
public enum MakeIntervalDateTimePartEnum
{
    Year,
    Month,
    Week,
    Day,
    Hour,
    Minute,
    Second
}
