using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>MIN</c> aggregate function.
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
///             new Min(new Column<Grades>(nameof(Grades.GradePoint))),
///             "MinimumGradePoint"
///         )
///     )
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT MIN("Grades"."GradePoint") AS "MinimumGradePoint" FROM "Grades"
/// 
/// --SqlServer
/// SELECT MIN([Grades].[GradePoint]) AS [MinimumGradePoint] FROM [Grades]
/// 
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
///             new Min(new Column<Grades>(nameof(Grades.GradePoint)), true),
///             "MinimumGradePoint"
///         )
///     )
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT MIN(DISTINCT "Grades"."GradePoint") AS "MinimumGradePoint" FROM "Grades"
/// 
/// --SqlServer
/// SELECT MIN(DISTINCT [Grades].[GradePoint]) AS [MinimumGradePoint] FROM [Grades]
/// 
/// ]]></code>
/// </example>
public sealed class Min : Aggregates
{
    /// <summary>
    /// Initializes a <c>MIN(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    /// <param name="distinct">Indicates whether to count distinct values.</param>
    public Min(SqlExpression expression, bool distinct = false) : base("MIN", distinct, expression)
    {
    }
}
