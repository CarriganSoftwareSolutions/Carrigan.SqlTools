namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL MAKE_DATE function.
/// </summary>
public class MakeDate : FunctionalExpression
{
    protected override string FunctionName => "MAKE_DATE";

    public MakeDate(SqlExpression year, SqlExpression month, SqlExpression day)
        : base([ValidateValue(year), ValidateValue(month), ValidateValue(day)])
    {
    }
}
