using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's CONTAINS operator.
/// </summary>
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
    /// Constructor for the CONTAINS operator.
    /// </summary>
    /// <param name="column">The left hand side should be a <see cref="Column{T}"/></param>
    /// <param name="parameter">The right hand side should be a <see cref="Predicates.Parameter"/></param>
    public Contains(Column<T> column, Parameter parameter)
    {
        _column = column;
        _parameter = parameter;
    }

    /// <summary>
    /// Leaf node in recursive logic to get all the parameters associated with the logic.
    /// Since this class doesn't have parameters, just return an empty.
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
    /// Produces the SQL represented by this class.
    /// </summary>
    /// <param name="prefix">
    /// building a prefix as we drill down the logic tree, 
    /// this prefix is added to the names of parameters to ensure that each parameter has a unique name
    /// this is only used with parameters that have duplicate names
    /// </param>
    /// <param name="duplicates">
    /// keep track of all of the user supplied parameter names that are duplicates
    /// this will be use in the leaf parameter node to determine if a prefix is needed or not.
    /// </param>
    /// <returns>Returns a SQL string represented by this class.</returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        $"CONTAINS({_column.ToSql()}, {_parameter.ToSql()})";

    /// <summary>
    /// Recursively get all the parameters associated with the logic, as key value pairs.
    /// </summary>
    /// <param name="prefix">
    /// building a prefix as we drill down the logic tree, 
    /// this prefix is added to the names of parameters to ensure that each parameter has a unique name
    /// this is only used with parameters that have duplicate names
    /// </param>
    /// <param name="duplicates">
    /// keep track of all of the user supplied parameter names that are duplicates
    /// this will be use in the leaf parameter node to determine if a prefix is needed or not.
    /// </param>
    /// <returns>Returns all the parameters associated with the logic, as key value pairs.</returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        _parameter.GetParameters(prefix, duplicates);
}
