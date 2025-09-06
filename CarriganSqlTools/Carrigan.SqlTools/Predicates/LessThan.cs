namespace SqlTools.Predicates;

public class LessThan : ComparisonOperators
{
    public LessThan(PredicatesBase left, PredicatesBase right) : base (left, right, "<")
    {
    }
}
