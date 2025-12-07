using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Sets;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.Sets;

/// <summary>
/// Determines whether the specified <see cref="ColumnCollection{T}"/> instance is
/// <c>null</c> or contains no column definitions.
/// </summary>
/// <typeparam name="T">The entity or data model type used in the <see cref="ColumnCollection{T}"/> instance.</typeparam>
/// <param name="columnCollection">The <see cref="ColumnCollection{T}"/> instance to evaluate.</param>
/// <returns>
/// <c>true</c> if <paramref name="columnCollection"/> is <c>null</c> or contains no columns;
/// otherwise, <c>false</c>.
/// </returns>
internal static class ColumnCollectionExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="ColumnCollection{T}"/> instance is
    /// <c>null</c> or contains no column definitions.
    /// </summary>
    /// <typeparam name="T">The entity or data model type used in the <see cref="ColumnCollection{T}"/> instance.</typeparam>
    /// <param name="columnCollection">The <see cref="ColumnCollection{T}"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="columnCollection"/> is <c>null</c> or contains no columns;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNullOrEmpty<T>(this ColumnCollection<T>? columnCollection) =>
        columnCollection?.ColumnInfo.IsNullOrEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="ColumnCollection{T}"/> instance is
    /// not <c>null</c> and contains at least one column definition.
    /// </summary>
    /// <typeparam name="T">The entity or data model type used in the <see cref="ColumnCollection{T}"/> instance.</typeparam>
    /// <param name="columnCollection">The <see cref="ColumnCollection{T}"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="columnCollection"/> is not <c>null</c> and contains at least one column;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this ColumnCollection<T>? columnCollection) =>
        columnCollection.IsNullOrEmpty() == false;
}
