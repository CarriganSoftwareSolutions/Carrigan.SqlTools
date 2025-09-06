
namespace SqlTools.Predicates;

public class GreaterThanEquals : ComparisonOperators
{
    public GreaterThanEquals(PredicatesBase left, PredicatesBase right) : base (left, right, ">=")
    {
    }
}
