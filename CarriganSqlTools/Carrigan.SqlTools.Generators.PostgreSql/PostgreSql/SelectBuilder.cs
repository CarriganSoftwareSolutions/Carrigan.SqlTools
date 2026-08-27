using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Builds SELECT query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
/// <example>
/// <para>Select with join example:</para>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// /// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".* FROM "Customer" 
/// INNER JOIN "Order" 
///    ON ("Customer"."Id" = "Order"."CustomerId")
/// ]]></code>
/// </example>
/// <example>
/// <para>Select with join and order by example:</para>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// Note: <see cref="OrderBy{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
/// 
/// OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join,
///     OrderBys = orderByOrderDate
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".* 
/// FROM "Customer" 
/// INNER JOIN "Order"
///    ON ("Customer"."Id" = "Order"."CustomerId")
/// ORDER BY "Order"."OrderDate" ASC
/// ]]></code>
/// </example>
/// <example>
/// <para>Select with join, where, and order by example:</para>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// Note: <see cref="ColumnBase{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// Note: <see cref="OrderBy{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
/// 
/// Column<Order> totalCol = new(nameof(Order.Total));
/// Parameter minTotal = new(500m, "Total");
/// GreaterThan greaterThan = new(totalCol, minTotal);
/// 
/// OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join,
///     Where = greaterThan,
///     OrderBys = orderByOrderDate
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT "Customer".* 
/// FROM "Customer" 
/// INNER JOIN "Order" 
///   ON ("Customer"."Id" = "Order"."CustomerId") 
/// WHERE ("Order"."Total" > $1) 
/// ORDER BY "Order"."OrderDate" ASC
/// ]]></code>
/// </example>
/// <example>
/// <para>Select with aggregate expressions and <c>GROUP BY</c>:</para>
/// <code language="csharp"><![CDATA[
/// Column<Grades> gradePoint = new(nameof(Grades.GradePoint));
/// 
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
///         SelectTagGenerator.Get<Grades>(nameof(Grades.CourseCode)),
///         new SelectTag(new Average(gradePoint), "AverageGradePoint"),
///         new SelectTag(new Sum(gradePoint), "TotalGradePoints"),
///         new SelectTag(new Min(gradePoint), "MinimumGradePoint"),
///         new SelectTag(new Max(gradePoint), "MaximumGradePoint"),
///         new SelectTag(new Count(gradePoint), "GradePointCount")
///     ),
///     GroupBys = GroupBys.New<Grades>(nameof(Grades.StudentId), nameof(Grades.CourseCode))
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT
///     "Grades"."StudentId",
///     "Grades"."CourseCode",
///     AVG("Grades"."GradePoint") AS "AverageGradePoint",
///     SUM("Grades"."GradePoint") AS "TotalGradePoints",
///     MIN("Grades"."GradePoint") AS "MinimumGradePoint",
///     MAX("Grades"."GradePoint") AS "MaximumGradePoint",
///     COUNT("Grades"."GradePoint") AS "GradePointCount"
/// FROM "Grades"
/// GROUP BY 
///     "Grades"."StudentId",
///     "Grades"."CourseCode"
/// ]]></code>
/// </example>
/// <example>
/// <para>Select with aggregate expressions, <c>GROUP BY</c>, and <c>HAVING</c>:</para>
/// <code language="csharp"><![CDATA[
/// Average semesterGpa = new(new Column<Grades>(nameof(Grades.GradePoint)));
/// 
/// SelectBuilder<Grades> selectBuilder = new()
/// {
///     Selects = new SelectTags
///     (
///         SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
///         SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
///         SelectTagGenerator.Get<Grades>(nameof(Grades.SemesterNumber)),
///         new SelectTag(semesterGpa, "SemesterGPA")
///     ),
///     GroupBys = GroupBys.New<Grades>
///     (
///         nameof(Grades.StudentId),
///         nameof(Grades.AcademicYear),
///         nameof(Grades.SemesterNumber)
///     ),
///     Having = new GreaterThan(semesterGpa, new Parameter(3.5, "HonorRollGpa"))
/// };
/// 
/// SqlQuery query = selectBuilder.AsSqlQuery();
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT
///     "Grades"."StudentId",
///     "Grades"."AcademicYear",
///     "Grades"."SemesterNumber",
///     AVG("Grades"."GradePoint") AS "SemesterGPA"
/// FROM 
///     "Grades" 
/// GROUP BY
///     "Grades"."StudentId",
///     "Grades"."AcademicYear",
///     "Grades"."SemesterNumber"
/// HAVING 
///     (AVG("Grades"."GradePoint") > $1)
/// ]]></code>
/// </example>
public sealed record SelectBuilder<T> : QueryBuilders.SelectBuilderBase<T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public SelectBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Select(this);
}
