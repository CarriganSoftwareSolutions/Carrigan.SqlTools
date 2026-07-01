using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base type for all SQL predicate nodes used to compose <c>WHERE</c> and <c>JOIN</c> conditions.
/// Supports recursive SQL generation and parameter collection (with automatic de-duplication/prefixing).
/// </summary>
public abstract class Predicates : SqlExpression
{

    /// <summary>
    /// Base constructor for all predicate classes.
    /// </summary>
    /// <param name="childExpressions">Represents all child nodes for a given predicate.</param>
    /// <param name="dialectNeutralStringRepresentation">
    /// Represents a dialect-neutral string representation of the expression, used for debugging, logging, and key-value pairs.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected Predicates(IEnumerable<SqlExpression> childExpressions, string dialectNeutralStringRepresentation) 
        : base(childExpressions, dialectNeutralStringRepresentation)
    {
    }
}
