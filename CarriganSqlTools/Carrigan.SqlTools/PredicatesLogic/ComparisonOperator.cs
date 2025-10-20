using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL comparison predicates (e.g., <c>=</c>, <c>&lt;&gt;</c>, <c>&gt;</c>, <c>&lt;</c>, etc.).
/// Combines two child <see cref="Predicates"/> nodes with a SQL comparison operator and
/// participates in recursive SQL/parameter generation.
/// </summary>
public abstract class ComparisonOperator : Predicates
{
    /// <summary>
    /// The left-side predicate of the comparison.
    /// </summary>
    private Predicates _left;
    /// <summary>
    /// The right-side predicate of the comparison.
    /// </summary>
    private Predicates _right;

    /// <summary>
    /// The SQL text for the comparison operator (e.g., <c>=</c>, <c>&lt;&gt;</c>, <c>&gt;</c>, <c>&lt;</c>).
    /// </summary>
    private string _operator;

    /// <summary>
    /// Initializes the comparison with its left/right operands and SQL operator.
    /// Intended for reuse by derived types’ constructors.
    /// </summary>
    /// <param name="left">The left-side predicate.</param>
    /// <param name="right">The right-side predicate.</param>
    /// <param name="op">The SQL representation of the comparison operator.</param>
    protected void Initialize(Predicates left, Predicates right, string op)
    {
        _operator = op;
        _left = left;
        _right = right;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Protected parameter less constructor for use by derived classes that call
    /// <see cref="Initialize(Predicates, Predicates, string)"/> explicitly.
    /// </summary>
    protected ComparisonOperator()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    { }

    /// <summary>
    /// Base constructor for comparison operators.
    /// </summary>
    /// <param name="left">The left-side predicate.</param>
    /// <param name="right">The right-side predicate.</param>
    /// <param name="op">The SQL representation of the comparison o
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ComparisonOperator(Predicates left, Predicates right, string op) => 
        Initialize(left, right, op);

    /// <summary>
    /// Recursively retrieves all <see cref="Parameter"/> instances referenced by this comparison.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
        _left.Parameters.Concat(_right.Parameters);

    /// <summary>
    /// Recursively retrieves all <see cref="ColumnBase"/> instances referenced by this comparison.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
        _left.Columns.Concat(_right.Columns);

    /// <summary>
    /// Produces the SQL fragment represented by this comparison operator and its operands.
    /// </summary>
    /// <param name="prefix">
    /// A disambiguation prefix accumulated during predicate-tree traversal.
    /// Used to ensure parameter names are unique when duplicates exist.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied <see cref="ParameterTag"/> values detected as duplicates.
    /// Leaf nodes use this to decide if the <paramref name="prefix"/> should be applied.
    /// </param>
    /// <returns>
    /// A SQL fragment in the form <c>(&lt;left-sql&gt; OP &lt;right-sql&gt;)</c>, e.g.,
    /// <c>([T].[Col] = @Parameter_Col)</c>.
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        $"({_left.ToSql($"{prefix}_L", duplicates)} {_operator} {_right.ToSql($"{prefix}_R", duplicates)})";

    /// <summary>
    /// Recursively retrieves all parameters associated with this comparison as key/value pairs.
    /// </summary>
    /// <param name="prefix">
    /// A disambiguation prefix accumulated during predicate-tree traversal.
    /// Used to ensure parameter names are unique when duplicates exist.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied <see cref="ParameterTag"/> values detected as duplicates.
    /// Leaf nodes use this to decide if the <paramref name="prefix"/> should be applied.
    /// </param>
    /// <returns>
    /// A sequence of key/value pairs mapping <see cref="ParameterTag"/> to the corresponding value.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        _left.GetParameters($"{prefix}_L", duplicates).Concat(_right.GetParameters($"{prefix}_R", duplicates));
}