using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

public class GreaterThanEquals : ComparisonOperators
{
    public GreaterThanEquals(PredicatesBase left, PredicatesBase right) : base (left, right, ">=")
    {
    }
}
