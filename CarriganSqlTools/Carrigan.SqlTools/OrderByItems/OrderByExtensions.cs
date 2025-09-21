using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Provides extension methods for <see cref="IOrderByClause"/> instances.
/// </summary>
public static class OrderByExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="IOrderByClause"/> instance is
    /// <c>null</c> or empty.
    /// </summary>
    /// <param name="value">The <see cref="IOrderByClause"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is <c>null</c> or contains no items;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty(this IOrderByClause? value) =>
        value?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="IOrderByClause"/> instance is
    /// not <c>null</c> and contains at least one item.
    /// </summary>
    /// <param name="value">The <see cref="IOrderByClause"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is not <c>null</c> and contains at least one item;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)]this IOrderByClause? value) =>
        value.IsNullOrEmpty() ==  false;
}
