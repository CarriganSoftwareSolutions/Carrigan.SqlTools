using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the base class that represents a SQL column in the predicate logic.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameters parameterName = new("Name", "Hank");
/// Columns&lt;Customer@gt; columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Columns  <T> : PredicatesBase, IColumnValue
{
    /// <summary>
    /// The name of the property representing the column
    /// </summary>
    public string PropertyName { get; }
    /// <summary>
    /// The Tag for the Column
    /// </summary>
    public ColumnTag ColumnTag { get; }
    /// <summary>
    /// The Tag for the Table
    /// </summary>
    public TableTag TableTag { get; }

    internal static ArgumentException NoSuchProperty(string properyName) =>
        new ($"{properyName} is not the valid name of a property in the class, {SqlToolsReflectorCache<T>.Type.Name}, representing: {SqlToolsReflectorCache<T>.Table}.", nameof(properyName));

    /// <summary>
    /// A constructor for a column in the predicate logic.
    /// </summary>
    /// <param name="propertyName">The name of property representing the column.</param>
    /// <exception cref="ArgumentException">Gets thrown if the propertyName is not is not a valid property for the class <see cref="T"/> representing the table.</exception>
    public Columns(string propertyName)
    {
        SqlToolsReflectorCache<T>.ValidateEntityPropertyNames(propertyName);

        TableTag = SqlToolsReflectorCache<T>.Table;
        ColumnTag = SqlToolsReflectorCache<T>.GetColumnTagByProperty(propertyName) ?? throw NoSuchProperty(propertyName);
        PropertyName = propertyName;
    }

    /// <summary>
    /// Leaf node in recursive logic to get all the parameters associated with the logic.
    /// Since this class doesn't have parameters, just return an empty.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter =>
        [];

    /// <summary>
    /// Leaf node in recursive logic to get all the Columns associated with the logic.
    /// Since this there will be only this Column, return it as an enumerable.
    /// </summary>
    internal override IEnumerable<IColumnValue> Column =>
        [this];

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
        ColumnTag;

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
        [];

    /// <summary>
    /// used for unit testing only
    /// </summary>
    internal static IEnumerable<Columns<T>> Get(params IEnumerable<string> propertyNames)
    {
        IEnumerable<string> invalid = propertyNames.Where(propertyName => (SqlToolsReflectorCache<T>.ContainsProperty(propertyName) is false));
        if (invalid.Any())
            throw SqlIdentifierException.FromInvalidColumnNames<T>(invalid);
        return propertyNames.Select(propertyName => new Columns<T>(propertyName));
    }
    /// <summary>
    /// used for unit testing only
    /// </summary>
    public static IEnumerable<Columns<T>> Get() =>
        SqlToolsReflectorCache<T>.Columns.Select(column => new Columns<T>(column._columnName));
}
