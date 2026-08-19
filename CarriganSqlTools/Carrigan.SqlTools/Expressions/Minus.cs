using System.Collections.Generic;
namespace Carrigan.SqlTools.Expressions;


/// <summary>
/// Represents the SQL logical <c>%</c> arithmetic operator, which performs the modulo operation on numeric expressions.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         new SelectTag
///         (
///             new Minus
///             (
///                 new Column<Grades>(nameof(Grades.CreditHours)),
///                 new Parameter(1)
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
/// SELECT ([Grades].[CreditHours] - @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" - @Parameter_1) FROM "Grades"
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
///             new Minus
///             (
///                 new NumericColumn<Grades>(nameof(Grades.CreditHours)),
///                 new NumericParameter(1)
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
/// SELECT ([Grades].[CreditHours] - @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" - @Parameter_1) FROM "Grades"
/// ]]></code>
/// </example>
/// <summary>
/// Represents the SQL logical <c>-</c> arithmetic operator.
/// </summary>
public class Minus : Subtract
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Minus"/> class, representing
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to subtract using <c>-</c>.
    /// </param>
    public Minus(params IEnumerable<NumericExpression> numericExpressions) : base(numericExpressions)
    {
    }
}
