using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Provides extension methods for working with instances of the <see cref="JoinsBase"/> class.
/// </summary>
/// <remarks>
/// These helper methods simplify common null and emptiness checks for join collections
/// represented by <see cref="JoinsBase"/> and its derived types, such as <see cref="Joins{T}"/>.
/// </remarks>
internal static class RelationsExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="JoinsBase"/> instance is <see langword="null"/>
    /// or contains no defined join operations.
    /// </summary>
    /// <param name="relation">The <see cref="JoinsBase"/> instance to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="relation"/> is <see langword="null"/> or has no join elements;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool IsNullOrEmpty(this JoinsBase? relation) =>
        relation?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="JoinsBase"/> instance is not <see langword="null"/>
    /// and contains at least one join operation.
    /// </summary>
    /// <param name="relation">The <see cref="JoinsBase"/> instance to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="relation"/> is not <see langword="null"/> 
    /// and contains one or more join elements; otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool IsNotNullOrEmpty([NotNullWhen(true)] this JoinsBase? relation) =>
        relation.IsNullOrEmpty() == false;
}
