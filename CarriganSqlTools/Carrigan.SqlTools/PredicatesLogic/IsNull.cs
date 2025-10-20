using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s logical <c>IS NULL</c> operator,
/// used to test whether a column or expression contains a <c>NULL</c> value
/// within <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
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
    public IsNull(Predicates someValue) => 
        _someValue = someValue;

    /// <summary>
    /// Recursively retrieves all parameters associated with this predicate.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
       _someValue.Parameters;

    /// <summary>
    /// Recursively retrieves all columns associated with this predicate.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
       _someValue.Columns;


    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <param name="prefix">
    /// A prefix added to parameter names during recursive traversal of the logic tree,
    /// ensuring that each parameter name remains unique.
    /// </param>
    /// <param name="duplicates">
    /// Tracks user-supplied parameter names that are duplicates, allowing this method
    /// to determine when prefixes should be applied.
    /// </param>
    /// <returns>
    /// A SQL string representing the <c>IS NULL</c> condition.
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        $"({_someValue.ToSql(prefix, duplicates)} IS NULL)";

    /// <summary>
    /// Recursively retrieves all parameters associated with this predicate as key–value pairs.
    /// </summary>
    /// <param name="prefix">
    /// A prefix added to parameter names during recursive traversal of the logic tree,
    /// ensuring that each parameter name remains unique.
    /// </param>
    /// <param name="duplicates">
    /// Tracks user-supplied parameter names that are duplicates, allowing this method
    /// to determine when prefixes should be applied.
    /// </param>
    /// <returns>
    /// A collection of key–value pairs representing parameter tags and their corresponding values.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        _someValue.GetParameters(prefix, duplicates);
}
