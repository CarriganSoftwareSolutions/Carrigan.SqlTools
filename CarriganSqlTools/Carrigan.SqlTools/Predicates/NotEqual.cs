namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class that represents SQL's Not Equal, <>, comparison operator.
/// </summary>
public class NotEqual : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Not Equal, <>, comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    public NotEqual(PredicatesBase left, PredicatesBase right) : base (left, right, "<>")
    {
    }
}
