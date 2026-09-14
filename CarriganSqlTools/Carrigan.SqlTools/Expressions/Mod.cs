using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents SQL modulo using the <c>%</c> arithmetic operator. This class is an alias for <see cref="Modulo"/>.
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
/// SqlQuery query = gradesGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT ([Grades].[CreditHours] % @Parameter_1) FROM [Grades]
/// 
/// --PostgreSql
/// SELECT ("Grades"."CreditHours" % $1) FROM "Grades"
/// ]]></code>
/// </example>
public class Mod : Modulo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mod"/> class.
    /// </summary>
    /// <param name="sqlExpressions">
    /// One or more SQL expressions to treat as numeric expressions and apply modulo using <c>%</c>. No numeric-type validation is performed.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sqlExpressions"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sqlExpressions"/> contains no elements.</exception>
    /// <exception cref="NullReferenceException">Thrown when <paramref name="sqlExpressions"/> contains disallowed <c>null</c> values.</exception>
    public Mod(params IEnumerable<SqlExpression> sqlExpressions) : base(sqlExpressions)
    {
    }
}
