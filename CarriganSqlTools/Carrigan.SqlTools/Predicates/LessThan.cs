using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

public class LessThan : ComparisonOperators
{
    public LessThan(PredicatesBase left, PredicatesBase right) : base (left, right, "<")
    {
    }
}
