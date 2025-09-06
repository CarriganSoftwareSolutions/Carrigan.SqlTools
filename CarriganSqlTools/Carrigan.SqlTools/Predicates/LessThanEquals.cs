using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

public class LessThanEquals : ComparisonOperators
{
    public LessThanEquals(PredicatesBase left, PredicatesBase right) : base (left, right, "<=")
    {
    }
}
