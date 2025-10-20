//IGNORE SPELLING: equal
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s logical <c>NOT</c> operator, which inverts the result
/// of a boolean predicate in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
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
    /// <param name="predicate">
    /// The boolean expression to negate. Typically another <see cref="Predicates"/> instance
    /// such as <see cref="Equal"/>, <see cref="GreaterThan"/>, or <see cref="And"/>.
    /// </param>
    public Not(Predicates someValue) => 
        _someValue = someValue;

    /// <summary>
    /// Recursively retrieves all parameters associated with this predicate.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
       _someValue.Parameters;

    /// <summary>
    /// Recursively retrieves all columns referenced by this predicate.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
       _someValue.Columns;

    /// <summary>
    /// Generates the SQL fragment represented by this <c>NOT</c> predicate.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This prefix
    /// is appended to parameter names to maintain uniqueness when duplicates occur.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-specified parameter tags that are duplicates, used to decide
    /// when a prefix must be applied.
    /// </param>
    /// <returns>
    /// A SQL string representing the negated predicate.
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        $"(NOT {_someValue.ToSql(prefix, duplicates)})";

    /// <summary>
    /// Recursively retrieves all parameters associated with this predicate
    /// as key–value pairs.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This prefix
    /// is appended to parameter names to maintain uniqueness when duplicates occur.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-specified parameter tags that are duplicates, used to decide
    /// when a prefix must be applied.
    /// </param>
    /// <returns>
    /// A sequence of parameter tag/value pairs for this predicate.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        _someValue.GetParameters(prefix, duplicates);
}