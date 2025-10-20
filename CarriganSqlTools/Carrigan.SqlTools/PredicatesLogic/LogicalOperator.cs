using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL logical operators (e.g., <c>AND</c>, <c>OR</c>) used to combine
/// one or more predicate expressions in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
public abstract class LogicalOperator : Predicates
{
    private readonly string _operator;
    private readonly IEnumerable<Predicates> _predicates;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogicalOperator"/> class for the specified
    /// SQL logical operator (e.g., <c>AND</c>, <c>OR</c>) and predicates.
    /// </summary>
    /// <remarks>
    /// Behavior:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     If no predicates are provided, an <see cref="ArgumentNullException"/> is thrown.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     If exactly one predicate is provided, that predicate is emitted directly without
    ///     adding the operator.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     If two or more predicates are provided, they are combined with the specified operator.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="op">The SQL operator token to use (e.g., <c>"AND"</c>, <c>"OR"</c>).</param>
    /// <param name="predicates">One or more boolean predicate expressions to combine.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c> or contains no elements.
    /// </exception>
    public LogicalOperator(string op, params IEnumerable<Predicates> predicates)
    {
        if (predicates.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(predicates), $"{nameof(predicates)} must contain at least one value.");
        _operator = op;
        _predicates = predicates;
    }

    /// <summary>
    /// Recursively retrieves all parameters used by the combined predicates.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
        _predicates.SelectMany(predicate => predicate.Parameters);

    /// <summary>
    /// Recursively retrieves all columns referenced by the combined predicates.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
        _predicates.SelectMany(predicate => predicate.Columns);

    /// <summary>
    /// Generates the SQL fragment represented by this logical operator.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This is appended to parameter
    /// names to keep them unique when duplicates occur.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-specified parameter tags that are duplicates, used to decide when a prefix
    /// must be applied.
    /// </param>
    /// <returns>
    /// A SQL string for the combined predicates. If a single predicate was provided, returns that
    /// predicate’s SQL without adding the operator; otherwise returns the predicates joined by the
    /// operator and wrapped in parentheses.
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates)
    {
        if (_predicates.Count() == 1)
            return _predicates.Single().ToSql(prefix, duplicates);
        else
            return $"({string.Join($" {_operator} ", _predicates.Select((predicate,index) => predicate.ToSql($"{prefix}_{index}", duplicates)))})";
    }

    /// <summary>
    /// Recursively retrieves all parameters used by the combined predicates as key–value pairs.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This is appended to parameter
    /// names to keep them unique when duplicates occur.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-specified parameter tags that are duplicates, used to decide when a prefix
    /// must be applied.
    /// </param>
    /// <returns>
    /// A sequence of parameter tag / value pairs for all combined predicates.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates)
    {
        if (_predicates.Count() == 1)
            return _predicates.Single().GetParameters(prefix, duplicates);
        else
            return _predicates.SelectMany((predicate, index) => predicate.GetParameters($"{prefix}_{index}", duplicates));
    }
}
