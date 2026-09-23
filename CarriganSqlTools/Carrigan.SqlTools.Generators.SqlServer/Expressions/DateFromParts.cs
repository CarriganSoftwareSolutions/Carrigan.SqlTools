namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server DATEFROMPARTS function. DATEFROMPARTS is available in SQL Server 2012 (11.x) and later.
/// </summary>
public class DateFromParts : FunctionalExpression
{
    protected override string FunctionName => "DATEFROMPARTS";

    public DateFromParts(SqlExpression year, SqlExpression month, SqlExpression day)
        : base([ValidateValue(year), ValidateValue(month), ValidateValue(day)])
    {
    }
}
