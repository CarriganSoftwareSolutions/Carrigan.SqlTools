using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
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
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joints"/> or contains <c>null</c> join entries.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when duplicate <see cref="ParameterTag"/> keys are encountered while aggregating parameters.
    /// </exception>
    internal Dictionary<ParameterTag, object> Parameters
    {
        get
        {
            if (ValidatedJoints.Count() == 1)
                return [.. ValidatedJoints.SelectMany((join, i) => join.GetParameters($"JoinParameter"))];
            else
                return [.. ValidatedJoints.SelectMany((join, i) => join.GetParameters($"Joins{i}Parameter"))];
            }
        }

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
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joints"/> or contains <c>null</c> join entries.
    /// </exception>
    internal IEnumerable<TableTag> TableTags =>
        ValidatedJoints.Select(static join => join.TableTag).Append(TableTag);

    /// <summary>
    /// Generates the complete SQL fragment for all <c>JOIN</c> clauses
    /// represented by this <see cref="JoinsBase"/> instance.
    /// </summary>
    /// <returns>
    /// A concatenated SQL string containing all <c>JOIN</c> clauses in sequence.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joints"/> or contains <c>null</c> join entries.
    /// </exception>
    /// <remarks>
    /// Any exception thrown by an individual join while rendering SQL will be propagated to the caller.
    /// </remarks>
    internal string ToSql()
    {
        if(ValidatedJoints.Count() == 1)
            return string.Join(" ", ValidatedJoints.Select(join => join.ToSql("Join")));
        else
            return string.Join(" ", ValidatedJoints.Select((join, i) => join.ToSql($"Joins{i}")));

    }

    internal IEnumerable<SqlFragment> ToSqlFragments()
    {
        if (ValidatedJoints.Count() == 1)
            return ValidatedJoints.SelectMany(join => join.ToSqlFragments("Join"));
        else
            return ValidatedJoints.SelectMany((join, i) => join.ToSqlFragments($"Joins{i}"));
    }

    /// <summary>
    /// Determines whether this <see cref="JoinsBase"/> instance contains any join definitions.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if no joins are defined; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joints"/> or contains <c>null</c> join entries.
    /// </exception>
    internal virtual bool IsEmpty() =>
        ValidatedJoints.None();

    private IEnumerable<JoinBase> ValidatedJoints
    {
        get
        {
            IEnumerable<JoinBase>? joints = Joints ?? throw new InvalidOperationException($"{GetType().Name}.{nameof(Joints)} returned null.");
            JoinBase[] materialized = joints as JoinBase[] ?? [.. joints];

            if (materialized.Any(static j => j is null))
                throw new InvalidOperationException($"{GetType().Name}.{nameof(Joints)} contains null join entries.");

            return materialized;
        }
    }
}
