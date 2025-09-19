using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the base class that represents SQL's predicates.
/// </summary>
public abstract class PredicatesBase
{
    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal abstract IEnumerable<Parameters> Parameter { get; }

    /// <summary>
    ///  Recursively get all the columns associated with the logic.
    /// </summary>
    internal abstract IEnumerable<IColumnValue> Column { get; }

    /// <summary>
    /// Produces the SQL represented by this class.
    /// </summary>
    /// <returns>Returns a SQL string represented by this class.</returns>
    public string ToSql()
    {
        //get an IEnumerable of all the duplicate parameter names
        IEnumerable<ParameterTag> duplicates = Parameter
            .Select(parameter => parameter.Name)
            .GroupBy(name => name)
            .Where(nameGroup => nameGroup.Count() > 1)
            .Select(nameGroup => nameGroup.Key);

        return ToSql(string.Empty, duplicates);
    }
    
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
    internal abstract string ToSql(string prefix, IEnumerable<ParameterTag> duplicates);

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
    public Dictionary<ParameterTag,object> GetParameters()
    {
        //get an IEnumerable of all the duplicate parameter names
        IEnumerable<ParameterTag> duplicates = Parameter
            .Select(parameter => parameter.Name)
            .GroupBy(parameter => parameter)
            .Where(nameGroup => nameGroup.Count() > 1)
            .Select(nameGroup => nameGroup.Key);

        return GetParameters(string.Empty, duplicates)
               .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    /// <summary>
    /// This method recursively adds prefixes to the parameter names to ensure unique parameter names
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
    internal abstract IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates);
}
