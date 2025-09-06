namespace Carrigan.SqlTools.Predicates;

public class Equal : ComparisonOperators
{
    public Equal(PredicatesBase left, PredicatesBase right) : base (left, right, "=")
    {
    }
}
