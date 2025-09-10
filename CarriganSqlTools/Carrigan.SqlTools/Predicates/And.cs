namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical AND operator for logical operations on one more predicate values.
/// </summary>
public class And : LogicalOperators
{
    /// <summary>
    /// Constructor for the logical boolean operator "AND".
    /// If no predicate values are passed in, then a <see cref="ArgumentNullException"/> is thrown.
    /// If only one predicate value is provided, then this class is deigned to use just that predicate in place of the logical operator.
    /// If two or more are provided then each predicate is chained together with the AND logical operator.
    /// </summary>
    /// <param name="predicates">One or more boolean predicates.</param>
    public And(params IEnumerable<PredicatesBase> predicates) : base("AND", predicates)
    {
    }
}
