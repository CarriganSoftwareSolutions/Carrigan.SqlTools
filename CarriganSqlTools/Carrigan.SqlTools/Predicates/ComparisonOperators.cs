namespace Carrigan.SqlTools.Predicates;

public abstract class ComparisonOperators : PredicatesBase
{
    private PredicatesBase _left;
    private PredicatesBase _right;
    private string _operator;
    public ComparisonOperators(PredicatesBase left, PredicatesBase right, string op)
    {
        _operator = op;
        _left = left;
        _right = right;
    }

    internal override IEnumerable<Parameters> Parameter =>
        _left.Parameter.Concat(_right.Parameter);

    internal override IEnumerable<IColumnValue> Column =>
        _left.Column.Concat(_right.Column);

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        $"({_left.ToSql($"{prefix}_L", duplicates)} {_operator} {_right.ToSql($"{prefix}_R", duplicates)})";

    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _left.GetParameters($"{prefix}_L", duplicates).Concat(_right.GetParameters($"{prefix}_R", duplicates));
}