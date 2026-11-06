using Carrigan.SqlTools.Tags;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Provides extension methods for working with <see cref="Joins"/> instances.
/// </summary>
/// <remarks>
/// These helpers provide consistent null and emptiness checks for join collections represented by
/// <see cref="Joins"/> and its derived types, such as <see cref="Joins"/>.
/// </remarks>
internal static class RelationsExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="Joins"/> instance is <see langword="null"/>
    /// or contains no defined join operations.
    /// </summary>
    /// <param name="relation">The <see cref="Joins"/> instance to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="relation"/> is <see langword="null"/> or has no join elements;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="relation"/> is not <see langword="null"/> and the underlying join collection is invalid
    /// (for example, <see cref="Joins"/> has a null join collection or contains null join entries).
    /// </exception>
    internal static bool IsNullOrEmpty([NotNullWhen(false)] this Joins? relation) =>
        relation?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="Joins"/> instance is not <see langword="null"/>
    /// and contains at least one join operation.
    /// </summary>
    /// <param name="relation">The <see cref="Joins"/> instance to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="relation"/> is not <see langword="null"/>
    /// and contains one or more join elements; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="relation"/> is not <see langword="null"/> and the underlying join collection is invalid
    /// (for example, <see cref="Joins"/> has a null join collection or contains null join entries).
    /// </exception>
    internal static bool IsNotNullOrEmpty([NotNullWhen(true)] this Joins? relation) =>
        relation.IsNullOrEmpty() == false;


    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects included in the current join sequence,
    /// including the base (left-most) table.
    /// </summary>
    /// <remarks>
    /// This provides a quick way to determine whether a given table participates in
    /// any join operation within this <see cref="Joins"/> instance.
    /// </remarks>
    /// <returns>
    /// An enumeration of all <see cref="TableTag"/> instances involved in the join sequence.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joins"/> or contains <c>null</c> join entries.
    /// </exception>
    public static IEnumerable<TableTagBase> TableTags(this IEnumerable<Join> joins) =>
        joins.Select(static join => join.JoinAs);
}
