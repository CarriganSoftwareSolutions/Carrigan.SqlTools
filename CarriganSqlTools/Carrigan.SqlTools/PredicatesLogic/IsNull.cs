using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's logical <c>IS NULL</c> operator,
/// used to test whether a column or expression contains a <c>NULL</c> value
/// within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// IsNull isNull = new(columnName);
/// SqlQuery query = customerGenerator.Select(null, null, isNull, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NULL)
/// ]]></code>
/// </example>
public class IsNull : Predicates
{
    private readonly Predicates _someValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNull"/> class,
    /// representing a predicate that checks whether the specified value
    /// or column is <c>NULL</c>.
    /// </summary>
    /// <param name="someValue">
    /// The expression to test for null.
    /// Typically a <see cref="Column{T}"/> instance representing a database column.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="someValue"/> is <c>null</c>.
    /// </exception>
    public IsNull(Predicates someValue) : base([ValidateSomeValue(someValue)]) =>
        _someValue = someValue;

    private static Predicates ValidateSomeValue(Predicates someValue)
    {
        ArgumentNullException.ThrowIfNull(someValue, nameof(someValue));
        return someValue;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <param name="prefix">
    /// A prefix added to parameter names during recursive traversal of the logic tree,
    /// ensuring that each parameter name remains unique.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all of the parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// Tracks user-supplied parameter names that are duplicates, allowing this method
    /// to determine when prefixes should be applied.
    /// </param>
    /// <returns>
    /// A SQL string representing the <c>IS NULL</c> condition.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prefix"/> or <paramref name="branchName"/> or <paramref name="duplicates"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<SqlFragment> ToSqlFragments()
    {
        yield return new SqlFragmentText("(");

        foreach (SqlFragment fragment in _someValue.ToSqlFragments())
            yield return fragment;

        yield return new SqlFragmentText(" IS NULL)");
    }
}
