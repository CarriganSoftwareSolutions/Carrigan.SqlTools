using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicate for SQL Server’s full-text <c>CONTAINS</c> operator.
/// Combines a single column and a single parameter (search term) into a
/// <c>CONTAINS(column, parameter)</c> expression for use in <c>WHERE</c> or <c>JOIN</c> conditions.
/// </summary>
/// <typeparam name="T">
/// The model type that maps to the table containing the target column.
/// </typeparam>
/// <example>
/// <para>
/// <see cref = "Column{T}" /> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterEmail = new("Email", "@example.");
/// Column<Customer> columnEmail = new(nameof(Customer.Email));
/// Contains<Customer> predicate = new(columnEmail, parameterEmail);
/// SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE CONTAINS([Customer].[Email], @Parameter_Email)
/// ]]></code>
/// </example>
public class Contains<T> : Predicates
{
    private readonly Column<T> _column;
    private readonly Parameter _parameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Contains{T}"/> predicate.
    /// </summary>
    /// <param name="column">
    /// The left-hand operand, representing the full-text indexed column
    /// (<see cref="Column{T}"/>).
    /// </param>
    /// <param name="parameter">
    /// The right-hand operand, representing the search term parameter
    /// (<see cref="Predicates.Parameter"/>).
    /// </param>
    public Contains(Column<T> column, Parameter parameter)
    {
        _column = column;
        _parameter = parameter;
    }

    /// <summary>
    /// Recursively retrieves all <see cref="ColumnBase"/> instances referenced by this predicate.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
       [_parameter];

    /// <summary>
    /// Leaf node in recursive logic to get all the Columns associated with the logic.
    /// Since this there will be only this Column, return it as an enumerable.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
       [_column];

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
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
    /// A SQL fragment of the form <c>CONTAINS(&lt;column&gt;, &lt;parameter&gt;)</c>.
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        $"CONTAINS({_column.ToSql()}, {_parameter.ToSql()})";

    /// <summary>
    /// Recursively retrieves all parameters associated with this predicate as key/value pairs.
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
    /// A sequence mapping each <see cref="ParameterTag"/> to its bound value.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        _parameter.GetParameters(prefix, duplicates);
}
