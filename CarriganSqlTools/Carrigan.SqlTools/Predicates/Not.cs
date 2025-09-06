
namespace SqlTools.Predicates;

public class Not : PredicatesBase
{
    PredicatesBase _someValue;
    public Not(PredicatesBase someValue)
    {
        _someValue = someValue;
    }

    internal override IEnumerable<Parameters> Parameter =>
       _someValue.Parameter;

    internal override IEnumerable<IColumnValue> Column =>
       _someValue.Column;

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        $"(NOT {_someValue.ToSql(prefix, duplicates)})";
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _someValue.GetParameters(prefix, duplicates);
}