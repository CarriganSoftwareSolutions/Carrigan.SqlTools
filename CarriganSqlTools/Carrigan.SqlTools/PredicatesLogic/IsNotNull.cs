using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's logical <c>IS NOT NULL</c> operator,
/// used to test whether a column or expression contains a non-null value
/// within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// IsNotNull notNull = new(columnName);
/// SqlQuery query = customerGenerator.Select(null, null, notNull, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Name] IS NOT NULL)
/// ]]></code>
/// </example>
public class IsNotNull : Predicates
{
    private readonly Predicates _someValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNotNull"/> class,
    /// representing a predicate that checks whether the specified value
    /// or column is not <c>NULL</c>.
    /// </summary>
    /// <param name="someValue">
    /// The expression to test for non-null.
    /// Typically a <see cref="ColumnBase{T}"/> instance representing a database column.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="someValue"/> is <c>null</c>.
    /// </exception>
    public IsNotNull(Predicates someValue) : base([ValidateSomeValue(someValue)]) =>
        _someValue = someValue;

    private static Predicates ValidateSomeValue(Predicates someValue)
    {
        ArgumentNullException.ThrowIfNull(someValue, nameof(someValue));
        return someValue;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <returns>
    /// A SQL string representing the <c>IS NOT NULL</c> condition.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(");

        foreach (ISqlFragment fragment in _someValue.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(" IS NOT NULL)");
    }
}
