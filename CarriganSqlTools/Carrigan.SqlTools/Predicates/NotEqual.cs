namespace Carrigan.SqlTools.Predicates;

public class NotEqual : ComparisonOperators
{
    public NotEqual(PredicatesBase left, PredicatesBase right) : base (left, right, "<>")
    {
    }
}
