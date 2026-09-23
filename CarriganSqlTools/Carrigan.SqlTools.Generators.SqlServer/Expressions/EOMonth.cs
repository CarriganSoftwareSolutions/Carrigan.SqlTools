namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server EOMONTH function. EOMONTH is available in SQL Server 2012 (11.x) and later.
/// </summary>
public class EOMonth : FunctionalExpression
{
    protected override string FunctionName => "EOMONTH";

    public EOMonth(SqlExpression startDate) : base([ValidateValue(startDate)])
    {
    }

    public EOMonth(SqlExpression startDate, SqlExpression monthToAdd)
        : base([ValidateValue(startDate), ValidateValue(monthToAdd)])
    {
    }
}
