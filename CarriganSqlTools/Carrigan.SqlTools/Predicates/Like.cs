namespace Carrigan.SqlTools.Predicates;

public class Like : ComparisonOperators
{
    public Like(PredicatesBase left, PredicatesBase right) : base (left, right, "LIKE")
    {
    }
}
