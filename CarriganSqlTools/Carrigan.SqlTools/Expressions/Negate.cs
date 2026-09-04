using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents unary numeric negation using the SQL <c>-</c> operator.
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
///                 new NumericColumn<Grades>(nameof(Grades.CreditHours))
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
public class Negate : NumericExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Negate"/> class for the supplied numeric expression.
    /// </summary>
    /// <param name="numericExpression">Represents the child numeric expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpression"/> is <c>null</c>.
    /// </exception>
    public Negate(NumericExpression numericExpression) : base([ValidateNumericExpression(numericExpression)], $"(-{numericExpression})")
    {
    }

    private static NumericExpression ValidateNumericExpression(NumericExpression numericExpression)
    {
        ArgumentNullException.ThrowIfNull(numericExpression, nameof(numericExpression));
        return numericExpression;
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
