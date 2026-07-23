using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>SUM</c> aggregate function.
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
///             new Sum(new Column<Grades>(nameof(Grades.GradePoint))),
///             "TotalGradePoints"
///         )
///     )
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT SUM("Grades"."GradePoint") AS "TotalGradePoints" FROM "Grades"
/// 
/// --SqlServer
/// SELECT SUM([Grades].[GradePoint]) AS [TotalGradePoints] FROM [Grades]
/// 
/// ]]></code>
/// </example>
public sealed class Sum : Aggregates
{
    /// <summary>
    /// Initializes a <c>SUM(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to sum.</param>
    public Sum(SqlExpression expression) : base("SUM", expression)
    {
    }
}
