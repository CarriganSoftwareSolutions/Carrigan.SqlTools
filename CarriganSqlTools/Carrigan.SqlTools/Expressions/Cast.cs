using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents SQL's <c>CAST</c> expression, which converts an expression from one data type to another.
/// </summary>
//TODO: Examples
public class Cast : SqlExpression
{
    /// <summary>
    /// Initializes a <c>CAST(expression AS type)</c> expression.
    /// </summary>
    public SqlExpression SqlExpression { get; init; }
    /// <summary>
    /// Initializes the properties of the field to which the expression is being cast.
    /// </summary>
    public FieldProperties FieldProperties { get; init; }

    /// <summary>
    /// Initializes a <c>CAST(expression AS type)</c> expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to cast.
    /// </param>
    /// <param name="fieldProperties">
    /// The properties of the field to which the expression is being cast.
    /// </param>
    public Cast(SqlExpression sqlExpression, FieldProperties fieldProperties) : base ([sqlExpression], GetDialectNeutralString(sqlExpression, fieldProperties))
    {
        SqlExpression = sqlExpression;
        FieldProperties = fieldProperties;
    }

    /// <summary>
    /// Generates a dialect-neutral SQL string for the <c>CAST</c> expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to cast.
    /// </param>
    /// <param name="fieldProperties">
    /// The properties of the field to which the expression is being cast.
    /// </param>
    /// <returns>
    /// A string representing the <c>CAST</c> expression in a dialect-neutral format.
    /// </returns>
    private static string GetDialectNeutralString(SqlExpression sqlExpression, FieldProperties fieldProperties) =>
        $"CAST({sqlExpression} AS {fieldProperties.BaseType})";

    /// <summary>
    /// Determines whether the cast expression is valid in an aggregate SELECT list.
    /// </summary>
    /// <param name="groupBys">
    /// The optional <c>GROUP BY</c> clause used to validate grouped column expressions.
    /// </param>
    /// <returns>
    /// The aggregate status of the expression being cast.
    /// </returns>
    public override bool IsAggregate(GroupBysBase? groupBys) =>
        SqlExpression.IsAggregate(groupBys);

    /// <summary>
    /// Generates the SQL fragments for the <c>CAST</c> expression based on the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the <c>CAST</c> expression.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> representing the <c>CAST</c> expression in the specified SQL dialect.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("CAST(");
        foreach(ISqlFragment sqlFragment in SqlExpression.ToSqlFragments(dialect))
        {
            yield return sqlFragment;
        }
        yield return new SqlFragmentText($" AS {dialect.RenderCastType(FieldProperties)}");
        yield return new SqlFragmentText(")");
    }
}
