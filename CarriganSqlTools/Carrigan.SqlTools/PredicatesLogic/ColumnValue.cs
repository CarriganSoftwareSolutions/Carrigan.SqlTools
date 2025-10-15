using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Data.Common;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class is essentially an alias for Column = Value
/// The intent is to reduce the amount of code needed to perform a routine task.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnValues&lt;Customer&gt; coumnValue = new(nameof(Customer.Name), "Hank");
/// SqlQuery query = customerGenerator.Select(null, coumnValue, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class ColumnValue<T> : Predicates
{
    protected readonly Predicates value;

    /// <summary>
    /// A public constructor. 
    /// </summary>
    /// <param name="propertyName">Column</param>
    /// <param name="parameterValue">Value</param>
    public ColumnValue(PropertyName propertyName, object parameterValue)
    {
        _ = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName); //called for validation.
        Column<T> left = new (propertyName);
        Parameter right = new(left.ColumnInfo.ParameterTag, parameterValue);
        value = new Equal
        (
           left,
           right
        );
    }

    /// <summary>
    /// A public constructor. 
    /// </summary>
    /// <param name="propertyName">Column</param>
    /// <param name="parameterValue">Value</param>
    [ExternalOnly]
    public ColumnValue(string propertyName, object parameterValue) 
        : this(new PropertyName(propertyName), parameterValue) { }

    /// <summary>
    /// Leaf node in recursive logic to get all the parameters associated with the logic.
    /// Since this class doesn't have parameters, just return an empty.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters => 
        value.Parameters;

    /// <summary>
    /// Leaf node in recursive logic to get all the Columns associated with the logic.
    /// Since this there will be only this Column, return it as an enumerable.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns => 
        value.Columns;

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
        value.ToSql(prefix, duplicates);

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
        value.GetParameters(prefix, duplicates);
}
