using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL expression that truncates a numeric value to a specified precision.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Truncate expression = new(new Column<Order>(nameof(Order.Total)), 2);
/// 
/// SelectBuilder<Order> selectBuilder = new()
/// {
///     Selects = expression.AsSelectTag("Value")
/// };
/// 
/// SqlQuery query = orderGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT ROUND([Order].[Total], @Parameter_1, 1) AS [Value] FROM [Order]
/// ]]></code>
/// </example>
public class Truncate : SqlExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Truncate"/> class that truncates the specified SQL expression to the nearest integer.  
    /// </summary>
    /// <param name="expression">
    /// The SQL expression to truncate.
    /// </param>
    public Truncate(SqlExpression expression) : base([ValidateValue(expression), ValidateParameterValue(0)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Truncate"/> class that truncates the specified SQL expression to the specified number of decimal places.
    /// </summary>
    /// <param name="expression">
    /// The SQL expression to truncate.
    /// </param>
    /// <param name="precision">
    /// The number of decimal places to truncate to.
    /// </param>
    public Truncate(SqlExpression expression, int precision) : base([ValidateValue(expression), ValidateParameterValue(precision)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Truncate"/> class that truncates the specified SQL expression to the specified precision defined by another SQL expression.
    /// </summary>
    /// <param name="expression">
    /// The SQL expression to truncate.
    /// </param>
    /// <param name="precision">
    /// The SQL expression defining the precision to truncate to.
    ///     </param>
    public Truncate(SqlExpression expression, SqlExpression precision) : base([ValidateValue(expression), ValidateValue(precision)])
    {
    }

    /// <summary>
    /// Generates the SQL fragments for the <see cref="Truncate"/> expression based on the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for generating the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable collection of <see cref="ISqlFragment"/> representing the SQL fragments for the <see cref="Truncate"/> expression.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("ROUND(");

        foreach (ISqlFragment sqlFragment in ChildNodes.ElementAt(0).ToSqlFragments(dialect))
            yield return sqlFragment;
        yield return ISqlFragment.CommaSpace;
        foreach (ISqlFragment sqlFragment in ChildNodes.ElementAt(1).ToSqlFragments(dialect))
            yield return sqlFragment;
        yield return ISqlFragment.CommaSpace;
        yield return new SqlFragmentText("1)");
    }

    /// <summary>
    /// Determines whether the <see cref="Truncate"/> expression is an aggregate expression based on its child nodes.
    /// </summary>
    /// <returns>
    /// A boolean value indicating whether the <see cref="Truncate"/> expression is an aggregate expression.
    /// </returns>
    /// <exception cref="AggregateInconsistencyException">
    /// Thrown when there is an inconsistency in the aggregate states of the child nodes of the <see cref="Truncate"/> expression.
    /// </exception>
    public override bool IsAggregate()
    {
        IEnumerable<(SqlExpression Expression, bool IsAggregate)> aggregateCandidates = ChildNodes
            .Select(expression => (Expression: expression, IsAggregate: expression.IsAggregate()))
            .Where(candidate => candidate.IsAggregate || candidate.Expression.HasColumns());

        bool[] aggregateStates = [.. aggregateCandidates.Select(static candidate => candidate.IsAggregate)];

        if (aggregateStates.AllEqual() is false)
            throw new AggregateInconsistencyException();

        return aggregateStates.FirstOrDefault();
    }
}
