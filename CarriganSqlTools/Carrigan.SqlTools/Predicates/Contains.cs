
namespace SqlTools.Predicates;

public class Contains<T> : PredicatesBase
{
    private Columns<T> _column;
    private Parameters _parameter;
    public Contains(Columns<T> column, Parameters parameter)
    {
        _column = column;
        _parameter = parameter;
    }

    internal override IEnumerable<Parameters> Parameter =>
       [_parameter];

    internal override IEnumerable<IColumnValue> Column =>
       [_column];

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        $"CONTAINS({_column.ToSql()}, {_parameter.ToSql()})";
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _parameter.GetParameters(prefix, duplicates);
}
