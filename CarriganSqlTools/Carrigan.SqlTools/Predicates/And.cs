namespace Carrigan.SqlTools.Predicates;

public class And : LogicalOperators
{
    public And(params IEnumerable<PredicatesBase> predicates) : base("AND", predicates)
    {
    }
}
