using Carrigan.Core.Extensions;

namespace SqlTools.Predicates;

public abstract class LogicalOperators : PredicatesBase
{
    private string _operator;
    private IEnumerable<PredicatesBase> _predicates;
    public LogicalOperators(string op, params IEnumerable<PredicatesBase> predicates)
    {
        if (predicates.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(predicates), $"{nameof(predicates)} must contain at least one value.");
        _operator = op;
        _predicates = predicates;
    }

    internal override IEnumerable<Parameters> Parameter =>
        _predicates.SelectMany(predicate => predicate.Parameter);

    internal override IEnumerable<IColumnValue> Column =>
        _predicates.SelectMany(predicate => predicate.Column);

    internal override string ToSql(string prefix, IEnumerable<string> duplicates)
    {
        if (_predicates.Count() == 1)
            return _predicates.Single().ToSql(prefix, duplicates);
        else
            return $"({string.Join($" {_operator} ", _predicates.Select((predicate,index) => predicate.ToSql($"{prefix}_{index}", duplicates)))})";
    }
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates)
    {
        if (_predicates.Count() == 1)
            return _predicates.Single().GetParameters(prefix, duplicates);
        else
            return _predicates.SelectMany((predicate, index) => predicate.GetParameters($"{prefix}_{index}", duplicates));
    }
}
