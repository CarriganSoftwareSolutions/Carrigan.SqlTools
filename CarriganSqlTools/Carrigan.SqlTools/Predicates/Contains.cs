using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's CONTAINS operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Contains<T> : PredicatesBase
{
    private Columns<T> _column;
    private Parameters _parameter;
    /// <summary>
    /// Constructor for the CONTAINS operator.
    /// </summary>
    /// <param name="column">The left hand side should be a <see cref="Columns{T}"/></param>
    /// <param name="parameter">The right hand side should be a <see cref="Parameters"/></param>
    public Contains(Columns<T> column, Parameters parameter)
    {
        _column = column;
        _parameter = parameter;
    }

    /// <summary>
    /// Leaf node in recursive logic to get all the parameters associated with the logic.
    /// Since this class doesn't have parameters, just return an empty.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter =>
       [_parameter];

    /// <summary>
    /// Leaf node in recursive logic to get all the Columns associated with the logic.
    /// Since this there will be only this Column, return it as an enumerable.
    /// </summary>
    internal override IEnumerable<IColumnValue> Column =>
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
    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
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
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _parameter.GetParameters(prefix, duplicates);
}
