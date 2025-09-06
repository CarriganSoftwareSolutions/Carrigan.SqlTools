using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

public class Xor : ComparisonOperators
{
    public Xor(PredicatesBase left, PredicatesBase right) : base(left, right, "^")
    {
    }
}