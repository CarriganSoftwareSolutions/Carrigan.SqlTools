//IGNORE SPELLING: equal

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical NOT operator for logical operations on one boolean predicate values.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// Equal equal = new(columnName, parameterName);
/// Not not = new(equal);
/// SqlQuery query = customerGenerator.Select(null, not, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE (NOT ([Customer].[Name] = @Parameter_Name))
/// ]]></code>
/// </example>
public class Not : PredicatesBase
{
    private readonly PredicatesBase _someValue;
    /// <summary>
    /// This is the constructor for the classes that represents SQL's NOT operators
    /// </summary>
    /// <param name="someValue">should represent something that would be a boolean value in SQL</param>
    public Not(PredicatesBase someValue) => 
        _someValue = someValue;

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter =>
       _someValue.Parameter;

    /// <summary>
    ///  Recursively get all the columns associated with the logic.
    /// </summary>
    internal override IEnumerable<IColumnValue> Column =>
       _someValue.Column;

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
        $"(NOT {_someValue.ToSql(prefix, duplicates)})";

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
        _someValue.GetParameters(prefix, duplicates);
}