using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Serves as the abstract base class for collections of SQL <c>JOIN</c> operations.
/// </summary>
/// <remarks>
/// This class represents a sequence of one or more <see cref="JoinBase"/> instances that collectively
/// define the set of <c>JOIN</c> clauses used in a query.  
/// Derived types, such as <see cref="Joins{T}"/>, manage construction and validation of these joins
/// to ensure referential correctness and proper join ordering.
/// </remarks>
public abstract class JoinsBase
{
    /// <summary>
    /// Represents a collection of objects where each instance defines a single SQL <c>JOIN</c> operation.
    /// </summary>
    /// <remarks>
    /// This property is intentionally named <c>Joints</c> to avoid a naming conflict with the class name
    /// <see cref="Joins{T}"/>. Each element in this collection corresponds to one join clause,
    /// such as an <c>INNER JOIN</c> or <c>LEFT JOIN</c>.
    /// </remarks>
    protected abstract IEnumerable<JoinBase> Joints { get; set; }

    /// <summary>
    /// Gets a dictionary containing all parameters associated with the join logic.
    /// </summary>
    /// <remarks>
    /// Each parameter represents a value used in a predicate within one or more <c>JOIN</c> conditions.
    /// </remarks>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> mapping <see cref="ParameterTag"/> instances
    /// to their corresponding parameter values.
    /// </returns>
    internal Dictionary<ParameterTag, object> Parameters =>
        [.. Joints.SelectMany(join => join.Parameters)];

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the base (left-most) table in the join sequence.
    /// </summary>
    internal abstract TableTag TableTag { get; }

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects included in the current join sequence,
    /// including the base (left-most) table.
    /// </summary>
    /// <remarks>
    /// This provides a quick way to determine whether a given table participates in
    /// any join operation within this <see cref="JoinsBase"/> instance.
    /// </remarks>
    /// <returns>
    /// An enumeration of all <see cref="TableTag"/> instances involved in the join sequence.
    /// </returns>
    internal IEnumerable<TableTag> TableTags =>
        Joints.Select(join => join.TableTag).Append(TableTag);

    /// <summary>
    /// Generates the complete SQL fragment for all <c>JOIN</c> clauses
    /// represented by the current <see cref="Joints"/> collection.
    /// </summary>
    /// <returns>
    /// A concatenated SQL string containing all <c>JOIN</c> clauses in sequence.
    /// </returns>
    internal string ToSql() =>
        string.Join(" ", Joints.Select(join => join.ToSql()));

    /// <summary>
    /// Determines whether this <see cref="JoinsBase"/> instance contains any join definitions.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if no joins are defined; otherwise, <see langword="false"/>.
    /// </returns>
    internal bool IsEmpty() =>
        Joints.None();
}