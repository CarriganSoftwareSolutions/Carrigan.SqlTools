
namespace SqlTools.Predicates;

public class LessThanEquals : ComparisonOperators
{
    public LessThanEquals(PredicatesBase left, PredicatesBase right) : base (left, right, "<=")
    {
    }
}
