using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Provides extension methods for <see cref="GroupBysBase"/> instances.
/// </summary>
internal static class GroupByExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="GroupBysBase"/> instance is
    /// <c>null</c> or empty.
    /// </summary>
    /// <param name="value">The <see cref="GroupBysBase"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is <c>null</c> or contains no items;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNullOrEmpty(this GroupBysBase? value) =>
        value?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="GroupBysBase"/> instance is
    /// not <c>null</c> and contains at least one item.
    /// </summary>
    /// <param name="value">The <see cref="GroupBysBase"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is not <c>null</c> and contains at least one item;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNotNullOrEmpty([NotNullWhen(true)]this GroupBysBase? value) =>
        value.IsNullOrEmpty() ==  false;
}
