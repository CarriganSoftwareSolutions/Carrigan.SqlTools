using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Provides extension methods for working with <see cref="Joins"/> instances.
/// </summary>
/// <remarks>
/// These helpers provide consistent null and emptiness checks for join collections represented by
/// <see cref="Joins"/> and its derived types, such as <see cref="Joins{T}"/>.
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
    internal static bool IsNullOrEmpty<T>([NotNullWhen(false)] this Joins<T>? relation) =>
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
    internal static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this Joins<T>? relation) =>
        relation.IsNullOrEmpty() == false;
}
