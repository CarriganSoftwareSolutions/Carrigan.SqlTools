using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
namespace Carrigan.SqlTools.Expressions;

///<summary>
/// Represents the SQL logical <c>%</c> arithmetic operator, which performs the modulo operation on numeric expressions.
///</summary>
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
/// SqlQuery query = customerGenerator.Select(selectBuilder);
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
/// <summary>
/// Represents a SQL Arithmetic Expression.
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
///                 new NumericColumn<Grades>(nameof(Grades.CreditHours))
///             )
///         )
///     )
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
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
/// <summary>
/// Represents the SQL logical <c>-</c> arithmetic operator.
/// </summary>
public class Negate : NumericExpression
{
    /// <summary>
    /// Negate operator
    /// </summary>
    /// <param name="numericExpression">Represents the child numeric expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpression"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpression"/> contains disallowed <c>null</c> values.
    /// </exception>
    public Negate(NumericExpression numericExpression) : base([numericExpression], $"(-{numericExpression})") =>
        ArgumentNullException.ThrowIfNull(numericExpression, nameof(numericExpression));

    /// <summary>
    /// Converts the Negate expression to SQL fragments.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for generating the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> representing the Negate expression in SQL.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(-");
        foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
            yield return fragment;
        yield return new SqlFragmentText(")");
    }
}
