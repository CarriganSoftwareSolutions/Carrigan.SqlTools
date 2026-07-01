using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's logical <c>IS NULL</c> operator,
/// used to test whether a column or expression contains a <c>NULL</c> value
/// within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// IsNull isNull = new(columnName);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = isNull
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer" 
/// WHERE ("Customer"."Name" IS NULL)
/// 
/// --SqlServer
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NULL)
/// ]]></code>
/// </example>
public class IsNull : Predicates
{
    /// <summary>
    /// The predicate expression wrapped by this IsNull predicate.
    /// </summary>
    private readonly SqlExpression _anExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNull"/> class,
    /// representing a predicate that checks whether the specified value
    /// or column is <c>NULL</c>.
    /// </summary>
    /// <param name="anExpression">
    /// The expression to test for null.
    /// Typically a <see cref="ColumnBase{T}"/> instance representing a database column.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="anExpression"/> is <c>null</c>.
    /// </exception>
    public IsNull(SqlExpression anExpression) 
        : base([ValidateSomeValue(anExpression)], $"({anExpression} IS NULL)") =>
        _anExpression = anExpression;

    /// <summary>
    /// Validates that the predicate being wrapped is present.
    /// </summary>
    /// <param name="someValue">The predicate expression to wrap.</param>
    /// <returns><paramref name="someValue"/> after validation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="someValue"/> is <see langword="null"/>.
    /// </exception>
    private static SqlExpression ValidateSomeValue(SqlExpression someValue)
    {
        ArgumentNullException.ThrowIfNull(someValue, nameof(someValue));
        return someValue;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <returns>
    /// A SQL string representing the <c>IS NULL</c> condition.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(");

        foreach (ISqlFragment fragment in _anExpression.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(" IS NULL)");
    }
}
