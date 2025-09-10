namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Greater Than, >, comparison operator.
/// </summary>
public class GreaterThan : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Greater Than, >, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public GreaterThan(PredicatesBase left, PredicatesBase right) : base (left, right, ">")
    {
    }
}
