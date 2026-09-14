using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents unary negation using the SQL <c>-</c> operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         new SelectTag
///         (
///             new Negate
///             (
///                 new Column<Grades>(nameof(Grades.CreditHours))
///             )
///         )
///     )
/// };
///
/// SqlQuery query = gradesGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT (-[Grades].[CreditHours]) FROM [Grades]
///
/// --PostgreSql
/// SELECT (-"Grades"."CreditHours") FROM "Grades"
/// ]]></code>
/// </example>
public class Negate : SqlExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Negate"/> class for the supplied SQL expression.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to negate. Operand type compatibility is delegated to the SQL database server.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    public Negate(SqlExpression sqlExpression) : base(sqlExpression is not null ? [sqlExpression] : throw new ArgumentNullException(nameof(sqlExpression)))
    {
    }

    /// <summary>
    /// Converts the Negate expression to SQL fragments.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for generating the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> representing the Negate expression in SQL.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(-");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return new SqlFragmentText(")");
    }
}
