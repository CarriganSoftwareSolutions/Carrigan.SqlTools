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
///             new Mod
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
/// SELECT ([Grades].[CreditHours] % @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" % @Parameter_1) FROM "Grades"
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
///             new Mod
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
/// SELECT ([Grades].[CreditHours] % @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" % @Parameter_1) FROM "Grades"
/// ]]></code>
/// </example>
/// <summary>
/// Represents the SQL logical <c>-</c> arithmetic operator.
/// </summary>
public class Mod : Modulo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mod"/> class, representing
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to  using <c>%</c>.
    /// </param>
    public Mod(params IEnumerable<NumericExpression> numericExpressions) : base(numericExpressions)
    {
    }
}
