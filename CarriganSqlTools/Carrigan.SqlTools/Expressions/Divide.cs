namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents SQL division using the <c>/</c> arithmetic operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         new SelectTag
///         (
///             new Divide
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
/// SELECT ([Grades].[CreditHours] / @Parameter_1) FROM [Grades]
///
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" / $1) FROM "Grades"
/// ]]></code>
/// </example>
public class Divide : ArithmeticExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Divide"/> class, representing
    /// the SQL <c>/</c> arithmetic operator.
    /// </summary>
    /// <param name="sqlExpressions">
    /// One or more SQL expressions to combine using <c>/</c>. Operand type compatibility is delegated to the SQL database server.
    /// </param>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Throws an <see cref="ArgumentException"/> if no expressions are provided.</description></item>
    /// <item><description>If only one expression is provided, that expression is used directly.</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="sqlExpressions"/> contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="sqlExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    public Divide(params IEnumerable<SqlExpression> sqlExpressions) : base("/", sqlExpressions)
    {
    }
}
