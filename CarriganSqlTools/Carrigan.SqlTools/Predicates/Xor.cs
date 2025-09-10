namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical XOR operator for logical operations on two predicate values.
/// </summary>
public class Xor : ComparisonOperators
{
    /// <summary>
    /// Constructor for the logical boolean operator "XOR".
    /// </summary>
    /// <param name="left">Left predicate</param>
    /// <param name="right">Right predicate</param>
    public Xor(PredicatesBase left, PredicatesBase right) : base(left, right, "^")
    {
    }
}