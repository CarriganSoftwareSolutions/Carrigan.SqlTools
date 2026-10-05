using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a SQL <c>NOT BETWEEN</c> predicate.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameter minimumTotal = new(100.00m, "MinimumTotal");
/// Parameter maximumTotal = new(500.00m, "MaximumTotal");
/// Column<Order> columnTotal = new(nameof(Order.Total));
/// NotBetween predicate = new(columnTotal, minimumTotal, maximumTotal);
/// SelectBuilder<Order> selectBuilder = new()
/// {
///     Where = predicate
/// };
/// 
/// SqlQuery query = orderGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Order".* FROM "Order" WHERE ("Order"."Total" NOT BETWEEN $1 AND $2)
/// --SqlServer
/// SELECT [Order].* FROM [Order] WHERE ([Order].[Total] NOT BETWEEN @MinimumTotal_1 AND @MaximumTotal_2)
/// ]]></code>
/// </example>
public class NotBetween : Predicates
{
    private static readonly ISqlFragment NotBetweenFragment = new SqlFragmentText(" NOT BETWEEN ");
    private static readonly ISqlFragment AndFragment = new SqlFragmentText(" AND ");
    /// <summary>
    /// Initializes a SQL <c>NOT BETWEEN</c> predicate.
    /// </summary>
    /// <param name="sqlExpression">The expression whose value is tested.</param>
    /// <param name="left">The lower-bound expression.</param>
    /// <param name="right">The upper-bound expression.</param>
    public NotBetween(SqlExpression sqlExpression, SqlExpression left, SqlExpression right) :
        base(ValidateValues(sqlExpression, left, right))
    {
    }



    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return ISqlFragment.OpenParentheses;

        foreach (ISqlFragment fragment in base.ChildNodes.ElementAt(0).ToSqlFragments(dialect))
            yield return fragment;

        yield return NotBetweenFragment;

        foreach (ISqlFragment fragment in base.ChildNodes.ElementAt(1).ToSqlFragments(dialect))
            yield return fragment;

        yield return AndFragment;

        foreach (ISqlFragment fragment in base.ChildNodes.ElementAt(2).ToSqlFragments(dialect))
            yield return fragment;

        yield return ISqlFragment.CloseParentheses;
    }
}
