
namespace SqlTools.Predicates;

public class GreaterThan : ComparisonOperators
{
    public GreaterThan(PredicatesBase left, PredicatesBase right) : base (left, right, ">")
    {
    }
}
