using Carrigan.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Provides extension methods for the <see cref="IJoins"/> interface.
/// </summary>
internal static class IJoinsExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="IJoins"/> instance is <c>null</c>
    /// or contains no join elements.
    /// </summary>
    /// <param name="joins">The <see cref="IJoins"/> instance to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="joins"/> is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    internal static bool IsNullOrEmpty(this IJoins? joins) =>
        joins?.Joints?.None() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="IJoins"/> instance is not null
    /// and contains at least one join element.
    /// </summary>
    /// <param name="joins">The <see cref="IJoins"/> instance to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="joins"/> is not <c>null</c> and contains at least one element; otherwise, <c>false</c>.</returns>

    internal static bool IsNotNullOrEmpty([NotNullWhen(true)] this IJoins? joins) =>
        joins.IsNullOrEmpty() == false;
}
