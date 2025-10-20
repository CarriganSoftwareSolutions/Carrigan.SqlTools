namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s logical <c>XOR</c> operator for combining two predicate expressions
/// in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
public class Xor : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Xor"/> class,
    /// representing SQL’s logical <c>XOR</c> (exclusive OR) operator.
    /// </summary>
    /// <param name="left">The left-hand predicate operand.</param>
    /// <param name="right">The right-hand predicate operand.</param>
    public Xor(Predicates left, Predicates right) : base(left, right, "^")
    {
    }
}