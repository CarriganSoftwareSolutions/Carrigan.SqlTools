namespace Carrigan.SqlTools.Predicates;

public class IsNotNull : PredicatesBase
{
    private PredicatesBase _someValue;
    public IsNotNull(PredicatesBase someValue)
    {
        _someValue = someValue;
    }

    internal override IEnumerable<Parameters> Parameter =>
       _someValue.Parameter;

    internal override IEnumerable<IColumnValue> Column =>
       _someValue.Column;

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        $"({_someValue.ToSql(prefix, duplicates)} IS NOT NULL)";
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _someValue.GetParameters(prefix, duplicates);
}
