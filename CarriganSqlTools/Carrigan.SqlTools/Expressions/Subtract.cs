namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents SQL subtraction using the <c>-</c> arithmetic operator.
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
/// SqlQuery query = gradesGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT ([Grades].[CreditHours] - @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" - $1) FROM "Grades"
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
///             new Subtract
///             (
///                 new NumericColumn<Grades>(nameof(Grades.CreditHours)),
///                 new NumericParameter<int>(1)
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
/// SELECT ([Grades].[CreditHours] - @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" - $1) FROM "Grades"
/// ]]></code>
/// </example>
public class Subtract : ArithmeticExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Subtract"/> class, representing
    /// the SQL <c>-</c> arithmetic operator.
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to combine using <c>-</c>.
    /// </param>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Throws an <see cref="ArgumentException"/> if no numeric expressions are provided.</description></item>
    /// <item><description>If only one numeric expression is provided, that expression is used directly.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="numericExpressions"/> contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    public Subtract(params IEnumerable<NumericExpression> numericExpressions) : base("-", numericExpressions)
    {
    }
}
