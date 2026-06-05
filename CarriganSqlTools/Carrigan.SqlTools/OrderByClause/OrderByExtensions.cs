using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.OrderByClause;

/// <summary>
/// Provides extension methods for <see cref="OrderBysBase"/> instances.
/// </summary>
internal static class OrderByExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="OrderBysBase"/> instance is
    /// <c>null</c> or empty.
    /// </summary>
    /// <param name="value">The <see cref="OrderBysBase"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is <c>null</c> or contains no items;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNullOrEmpty(this OrderBysBase? value) =>
        value?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="OrderBysBase"/> instance is
    /// not <c>null</c> and contains at least one item.
    /// </summary>
    /// <param name="value">The <see cref="OrderBysBase"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is not <c>null</c> and contains at least one item;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNotNullOrEmpty([NotNullWhen(true)]this OrderBysBase? value) =>
        value.IsNullOrEmpty() ==  false;
}
