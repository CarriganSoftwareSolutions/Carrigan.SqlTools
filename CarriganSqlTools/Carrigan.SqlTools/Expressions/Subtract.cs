using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL logical <c>-</c> arithmetic operator, which performs subtraction on numeric expressions.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         new SelectTag
///         (
///             new Subtract
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
///             new Subtract
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
public class Subtract : ArithmeticExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Subtract"/> class, representing
    /// the SQL logical <c>-</c> arithmetic operator.
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to  using <c>-</c>.
    /// </param>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Throws an <see cref="ArgumentNullException"/> if no numeric expressions are provided.</description></item>
    /// <item><description>If only one numeric expressions is provided, that predicate is used directly.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpressions"/> is <c>null</c> or contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    public Subtract(params IEnumerable<NumericExpression> numericExpressions) : base("-", numericExpressions)
    {
    }
}
