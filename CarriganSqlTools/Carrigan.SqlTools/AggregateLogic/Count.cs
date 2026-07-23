using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>COUNT</c> aggregate function.
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
///             new Count(new Column<Grades>(nameof(Grades.GradePoint))),
///             "GradePointCount"
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
/// SELECT COUNT("Grades"."GradePoint") AS "GradePointCount" FROM "Grades"
/// 
/// --SqlServer
/// SELECT COUNT([Grades].[GradePoint]) AS [GradePointCount] FROM [Grades]
/// ]]></code>
/// </example>
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
///             new Count(),
///             "GradeRecordCount"
///         )
///     )
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT COUNT(*) AS "GradeRecordCount" FROM "Grades"
/// 
/// --SqlServer
/// SELECT COUNT(*) AS [GradeRecordCount] FROM [Grades]
/// ]]></code>
/// </example>
public sealed class Count : Aggregates
{
    /// <summary>
    /// Initializes a <c>COUNT(*)</c> expression.
    /// </summary>
    public Count() : base("COUNT")
    {
    }

    /// <summary>
    /// Initializes a <c>COUNT(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to count.</param>
    public Count(SqlExpression expression) : base("COUNT", expression)
    {
    }
}
