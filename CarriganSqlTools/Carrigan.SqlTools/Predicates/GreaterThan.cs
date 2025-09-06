using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

public class GreaterThan : ComparisonOperators
{
    public GreaterThan(PredicatesBase left, PredicatesBase right) : base (left, right, ">")
    {
    }
}
