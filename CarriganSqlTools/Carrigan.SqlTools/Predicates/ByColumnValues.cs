using Carrigan.Core.Extensions;
using Carrigan.SqlTools;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

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
            throw new ArgumentException("Thats quite the nasty feat you just pulled off. How did you get a collection with a negative count? I am impressed.", nameof(compositeColumnValues));
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
    public ByColumnValues(string column, object value)
    {
        Initi([.. new[] { new KeyValuePair<string, object>(column, value) }]);
    }
    public ByColumnValues(params IEnumerable<KeyValuePair<string, object>> compositeColumnValues)
    {
        Initi([.. compositeColumnValues]);
    }
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

    internal override IEnumerable<Parameters> Parameter => 
        value.Parameter;

    internal override IEnumerable<IColumnValue> Column => 
        value.Column;

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        value.ToSql(prefix, duplicates);

    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        value.GetParameters(prefix, duplicates);
}
