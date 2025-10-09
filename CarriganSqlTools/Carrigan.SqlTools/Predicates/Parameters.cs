using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's parameter and the corresponding value of that parameter.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, equalName, null, null);
/// ]]></code>
/// </example>
public class Parameters : PredicatesBase
{
    /// <summary>
    /// Value of parameter
    /// </summary>
    internal readonly object? Value;
    /// <summary>
    /// Name of the parameter
    /// </summary>
    internal readonly ParameterTag Name;

    /// <summary>
    /// Constructor for Parameter
    /// Note: a prefix maybe added to the final parameter name
    /// </summary>
    /// <param name="parameter">The name you want the parameter to have</param>
    /// <param name="value">The value of the parameter</param>
    public Parameters(ParameterTag parameter, object? value)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        Name = parameter;
        Value = value;
    }

    /// <summary>
    /// Constructor for Parameter
    /// Note: a prefix maybe added to the final parameter name
    /// </summary>
    /// <param name="parameter">The name you want the parameter to have</param>
    /// <param name="value">The value of the parameter</param>
    /// <exception cref="InvalidSqlIdentifierException">The parameter name was empty or contained an invalid character</exception>
    [ExternalOnly]
    public Parameters(string parameter, object? value)
    {
        Name = new ParameterTag(null, parameter, null);
        Value = value;
    }

    /// <summary>
    /// Leaf node in recursive logic to get all the parameters associated with the logic.
    /// Note: In this case, it will be just this parameter name inserted into an enumeration.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter =>
       [this];

    /// <summary>
    /// Leaf node in recursive logic to get all the Columns associated within the logic.
    /// Since this class doesn't have Column, just return an empty.
    /// </summary>
    internal override IEnumerable<IColumns> Column =>
       [];

    /// <summary>
    /// Get the SQL recursively, while also renaming the parameters to ensure they are unique and match the names from GetParameters.
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
    /// <returns>partially completed sql string</returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        GetFinalParameterName(prefix, duplicates);

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
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates)
    {
        ParameterTag key = GetFinalParameterName(prefix, duplicates);
        object value = Value ?? DBNull.Value;
        KeyValuePair<ParameterTag, object> keyValuePair = new(key, value);
        return (new KeyValuePair<ParameterTag, object>[] { keyValuePair }).AsEnumerable();
    }
    /// <summary>
    /// Leaf node for Recursive logic to get all the parameters associated within clause, as key value pairs..
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
    private ParameterTag GetFinalParameterName(string prefix, IEnumerable<ParameterTag> duplicates) =>
        duplicates.Contains(Name) ? Name.PrefixPrepend($"@Parameter{prefix}") : Name.PrefixPrepend($"@Parameter");
}
