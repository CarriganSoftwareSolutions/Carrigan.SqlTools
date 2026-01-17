//IGNORE SPELLING: equal
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's logical <c>NOT</c> operator, which inverts the result
/// of a boolean predicate in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equal = new(columnName, parameterName);
/// Not not = new(equal);
/// SqlQuery query = customerGenerator.Select(null, null, not, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE (NOT ([Customer].[Name] = @Parameter_Name))
/// ]]></code>
/// </example>
public class Not : Predicates
{
    private readonly Predicates _someValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Not"/> class,
    /// representing the SQL <c>NOT</c> operator.
    /// </summary>
    /// <param name="someValue">
    /// The boolean expression to negate. Typically another <see cref="Predicates"/> instance
    /// such as <see cref="Equal"/>, <see cref="GreaterThan"/>, or <see cref="And"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="someValue"/> is <c>null</c>.
    /// </exception>
    public Not(Predicates someValue) : base([ValidateSomeValue(someValue)]) =>
        _someValue = someValue;

    private static Predicates ValidateSomeValue(Predicates someValue)
    {
        ArgumentNullException.ThrowIfNull(someValue, nameof(someValue));
        return someValue;
    }

    /// <summary>
    /// Generates the SQL fragment represented by this <c>NOT</c> predicate.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This prefix
    /// is appended to parameter names to maintain uniqueness when duplicates occur.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all of the parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-specified parameter tags that are duplicates, used to decide
    /// when a prefix must be applied.
    /// </param>
    /// <returns>
    /// A SQL string representing the negated predicate.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prefix"/> or <paramref name="branchName"/> or <paramref name="duplicates"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates)
    {
        ArgumentNullException.ThrowIfNull(prefix, nameof(prefix));
        ArgumentNullException.ThrowIfNull(branchName, nameof(branchName));
        ArgumentNullException.ThrowIfNull(duplicates, nameof(duplicates));

        yield return new SqlFragmentText("(NOT ");

        foreach (SqlFragment fragment in _someValue.ToSql(prefix, branchName, duplicates))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}
