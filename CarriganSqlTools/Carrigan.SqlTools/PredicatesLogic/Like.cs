using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's <c>LIKE</c> comparison operator, used for pattern matching
/// in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterEmail = new("%@example.com", "Email");
/// Column<Customer> columnEmail = new(nameof(Customer.Email));
/// Like predicate = new(columnEmail, parameterEmail);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = predicate
/// };
///
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".*
/// FROM "Customer"
/// WHERE ("Customer"."Email" LIKE $1)
///
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Email]
/// LIKE @Email_1)
/// ]]></code>
/// </example>
public class Like : DialectOperator
{
    /// <summary>
    /// Indicates whether the dialect should render a case-sensitive or case-insensitive LIKE comparison when it supports that distinction.
    /// </summary>
    private readonly bool? IsCaseSensitive;

    /// <summary>
    /// Initializes a new instance of the <see cref="Like"/> class,
    /// representing a predicate that performs a pattern match using SQL's
    /// <c>LIKE</c> operator.
    /// </summary>
    /// <param name="left">
    /// The left-hand operand of the comparison, typically a <see cref="ColumnBase{T}"/> instance.
    /// </param>
    /// <param name="right">
    /// The right-hand operand of the comparison, typically a <see cref="Parameter"/> or another <see cref="Predicates"/> expression.
    /// </param>
    /// <param name="isCaseSensitive">
    /// An optional boolean indicating whether the pattern match should be case-sensitive.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// When <c>isCaseSensitive</c> is not specified, the dialect's default comparison behavior is used.
    /// For SQL Server, case sensitivity is determined by the applicable server, database, column, or
    /// expression-level collation.
    /// For PostgreSQL, the default is <c>LIKE</c>, which performs a case-sensitive comparison.
    /// </remarks>
    public Like(SqlExpression left, SqlExpression right, bool? isCaseSensitive = null)
        : base(left, right, $"({left} {GetDialectNeutralStringOperator(isCaseSensitive)} {right})") =>
        IsCaseSensitive = isCaseSensitive;

    private static string GetDialectNeutralStringOperator(bool? isCaseSensitive = null) =>
        isCaseSensitive switch
        {
            null => "LIKE",
            true => "CASE SENSITIVE LIKE",
            false => "CASE INSENSITIVE LIKE"
        };

    /// <summary>
    /// Produces the SQL fragment represented by this Dialect operator and its operands.
    /// </summary>
    /// <param name="prefix">
    /// A disambiguation prefix accumulated during predicate-tree traversal.
    /// Used to ensure parameter names are unique when duplicates exist.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied <see cref="ParameterTag"/> values detected as duplicates.
    /// Leaf nodes use this to decide if the <paramref name="prefix"/> should be applied.
    /// </param>
    /// <returns>
    /// A SQL fragment in the form <c>(&lt;left-sql&gt; OP &lt;right-sql&gt;)</c>, e.g.,
    /// <c>([T].[Col] = @Parameter_Col)</c>.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(");

        foreach (ISqlFragment fragment in _left.ToSqlFragments(dialect))
            yield return fragment;

        yield return ISqlFragment.Space;

        yield return dialect.GetDialectLike(IsCaseSensitive);

        yield return ISqlFragment.Space;

        foreach (ISqlFragment fragment in _right.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}
