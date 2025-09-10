using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class is essentially an alias for Column = Value
/// The intent is to reduce the amount of code needed to perform a routine task.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// ByColumnValues&lt;Customer&gt; coumnValue = new(nameof(Customer.Name), "Hank");
/// SqlQuery query = customerGenerator.Select(null, coumnValue, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class ByColumnValues<T> : PredicatesBase
{
    protected PredicatesBase value;

    private void Initi(Dictionary<string, object> compositeColumnValues)
    {
        IEnumerable<string> invalidColumns;
        if (compositeColumnValues.Keys.None())
            throw new ArgumentException("Must have at least one column name / value pair", nameof(compositeColumnValues));
        else if (compositeColumnValues.Keys.Count < 0)
        {
            throw new ArgumentException("That's quite the nasty feat you just pulled off. How did you get a collection with a negative count? I am impressed.", nameof(compositeColumnValues));
        }

        invalidColumns = compositeColumnValues.Keys.Where(key => SqlToolsReflectorCache<T>.ColumnNames.DoesNotContain(key));
        if(invalidColumns.Any())
            throw SqlIdentifierException.FromInvalidColumnNames<T>(invalidColumns);

        value = new And
        (
            compositeColumnValues.Keys.Select
            (
                key =>
                new Equal
                (
                    new Columns<T>(key),
                    new Parameters(key, compositeColumnValues[key])
                )
            )
        );

    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// A public constructor for a class that represents Column = Value in SQL
    /// </summary>
    /// <param name="column">Column</param>
    /// <param name="value">Value</param>
    public ByColumnValues(string column, object value)
    {
        Initi([.. new[] { new KeyValuePair<string, object>(column, value) }]);
    }

    /// <summary>
    /// A public constructor. Takes the key value pairs and creates an object that represents
    /// an SQL query with a column name for each key and a value for the corresponding value 
    /// and the SQL is equivalent to an AND
    /// Example: Col1 = 1 AND Col2 = 2 AND Col3 = 3
    /// </summary>
    /// <param name="compositeColumnValues">An enumeration of key value pairs.</param>
    public ByColumnValues(params IEnumerable<KeyValuePair<string, object>> compositeColumnValues)
    {
        Initi([.. compositeColumnValues]);
    }


    /// <summary>
    /// A public constructor. Takes the Dictionary and creates an object that represents
    /// an SQL query with a column name for each key and a value for the corresponding value 
    /// and the SQL is equivalent to an AND
    /// Example: Col1 = 1 AND Col2 = 2 AND Col3 = 3
    /// </summary>
    /// <param name="compositeColumnValues">a dictionary </param>
    public ByColumnValues(Dictionary<string, object> compositeColumnValues)
    {
        Initi(compositeColumnValues);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// This scenario forms a predicate where a single column is selected according to multiple values.
    /// Example: Id = 1 OR Id = 2 Or Id = 3
    /// </summary>
    /// <param name="column">The column name to select by.</param>
    /// <param name="values">Multiple values to select.</param>
    /// <returns></returns>
    public static PredicatesBase ByMultipleValues(string column, params IEnumerable<object> values)
    {
        if (values.Any() == false)
            throw new ArgumentException("Must have at least one column name / value pair", nameof(column));

        if (SqlToolsReflectorCache<T>.ColumnNames.DoesNotContain(column))
            throw SqlIdentifierException.FromInvalidColumnNames<T>(column);

        return new Or
        (
            values.Select
            (
                value =>
                new Equal
                (
                    new Columns<T>(column),
                    new Parameters(column, value)
                )
            )
        );
    }

    /// <summary>
    /// Leaf node in recursive logic to get all the parameters associated with the logic.
    /// Since this class doesn't have parameters, just return an empty.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter => 
        value.Parameter;

    /// <summary>
    /// Leaf node in recursive logic to get all the Columns associated with the logic.
    /// Since this there will be only this Column, return it as an enumerable.
    /// </summary>
    internal override IEnumerable<IColumnValue> Column => 
        value.Column;

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
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        value.GetParameters(prefix, duplicates);
}
