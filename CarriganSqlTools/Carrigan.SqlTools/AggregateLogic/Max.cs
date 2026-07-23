using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>MAX</c> aggregate function.
/// </summary>
/// <example>
/// <para>
/// Using Column Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.AggregateLogic;
/// using Carrigan.SqlTools.Base.Tests.Helpers;
/// using Carrigan.SqlTools.Base.Tests.TestEntities;
/// using Carrigan.SqlTools.Expressions;
/// using Carrigan.SqlTools.GroupByClause;
/// using Carrigan.SqlTools.PostgreSql;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.Tags;
///
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         new SelectTag
///         (
///             new Max(new Column<Grades>(nameof(Grades.GradePoint))),
///             "MaximumGradePoint"
///         )
///     )
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT MAX("Grades"."GradePoint") AS "MaximumGradePoint" FROM "Grades"
/// 
/// --SqlServer
/// SELECT MAX([Grades].[GradePoint]) AS [MaximumGradePoint] FROM [Grades]
/// 
/// ]]></code>
/// </example>
public sealed class Max : Aggregates
{
    /// <summary>
    /// Initializes a <c>MAX(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    public Max(SqlExpression expression) : base("MAX", expression)
    {
    }
}
