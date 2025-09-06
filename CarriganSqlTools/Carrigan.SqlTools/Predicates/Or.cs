

namespace SqlTools.Predicates;

public class Or : LogicalOperators
{
    public Or(params IEnumerable<PredicatesBase> predicates) : base ("OR", predicates)
    {
    }
}
