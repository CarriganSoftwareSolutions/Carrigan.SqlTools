namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's LIKE comparison operator.
/// </summary>
public class Like : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's LIKE, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public Like(PredicatesBase left, PredicatesBase right) : base (left, right, "LIKE")
    {
    }
}
