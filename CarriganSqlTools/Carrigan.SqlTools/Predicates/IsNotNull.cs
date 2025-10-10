using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical IS NOT NULL operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// IsNotNull notNull = new(columnName);
/// SqlQuery query = customerGenerator.Select(null, notNull, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NOT NULL)
/// ]]></code>
/// </example>
public class IsNotNull : Predicates
{
    private readonly Predicates _someValue;

    /// <summary>
    /// This is the constructor for the classes that represents SQL's IS NOT NULL operator
    /// </summary>
    /// <param name="someValue">should represent something that may or may not be a null value in SQL</param>
    public IsNotNull(Predicates someValue) => 
        _someValue = someValue;

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
       _someValue.Parameters;

    /// <summary>
    ///  Recursively get all the columns associated with the logic.
    /// </summary>
    internal override IEnumerable<IColumn> Columns =>
       _someValue.Columns;

    //TODO: Proof read documentation
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
        $"({_someValue.ToSql(prefix, duplicates)} IS NOT NULL)";

    //TODO: Proof read documentation
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
        _someValue.GetParameters(prefix, duplicates);
}
