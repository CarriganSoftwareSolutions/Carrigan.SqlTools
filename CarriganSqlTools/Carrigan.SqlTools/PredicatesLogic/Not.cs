//IGNORE SPELLING: equal
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL's logical <c>NOT</c> operator, which inverts the result
/// of a boolean predicate in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
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
    /// <returns>
    /// A SQL string representing the negated predicate.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(NOT ");

        foreach (ISqlFragment fragment in _someValue.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}
