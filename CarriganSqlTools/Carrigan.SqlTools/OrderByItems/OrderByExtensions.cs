using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Provides extension methods for <see cref="OrderByBase"/> instances.
/// </summary>
internal static class OrderByExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="OrderByBase"/> instance is
    /// <c>null</c> or empty.
    /// </summary>
    /// <param name="value">The <see cref="OrderByBase"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is <c>null</c> or contains no items;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNullOrEmpty(this OrderByBase? value) =>
        value?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="OrderByBase"/> instance is
    /// not <c>null</c> and contains at least one item.
    /// </summary>
    /// <param name="value">The <see cref="OrderByBase"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is not <c>null</c> and contains at least one item;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNotNullOrEmpty([NotNullWhen(true)]this OrderByBase? value) =>
        value.IsNullOrEmpty() ==  false;
}
