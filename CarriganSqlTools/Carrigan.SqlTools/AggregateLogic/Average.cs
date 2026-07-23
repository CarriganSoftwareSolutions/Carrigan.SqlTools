using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Convenience alias for SQL's <c>AVG</c> aggregate function.
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
///             new Average(new Column<Grades>(nameof(Grades.GradePoint))),
///             "OverallAverageGradePoint"
///         )
///     )
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// 
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT AVG("Grades"."GradePoint") AS "OverallAvgGradePoint" FROM "Grades"
/// 
/// --SqlServer
/// SELECT AVG([Grades].[GradePoint]) AS [OverallAverageGradePoint] FROM [Grades] 
/// ]]></code>
/// </example>
public sealed class Average : Avg
{
    /// <summary>
    /// Initializes an <c>AVG(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to average.</param>
    public Average(SqlExpression expression) : base(expression)
    {
    }
}
