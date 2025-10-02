using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Sets;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.Sets;

/// <summary>
/// Determines whether the specified <see cref="SetColumns{T}"/> instance is
/// <c>null</c> or contains no column definitions.
/// </summary>
/// <typeparam name="T">The entity or data model type used in the <see cref="SetColumns{T}"/> instance.</typeparam>
/// <param name="setColumns">The <see cref="SetColumns{T}"/> instance to evaluate.</param>
/// <returns>
/// <c>true</c> if <paramref name="setColumns"/> is <c>null</c> or contains no columns;
/// otherwise, <c>false</c>.
/// </returns>
public static class SetColumnsExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="SetColumns{T}"/> instance is
    /// <c>null</c> or contains no column definitions.
    /// </summary>
    /// <typeparam name="T">The entity or data model type used in the <see cref="SetColumns{T}"/> instance.</typeparam>
    /// <param name="setColumns">The <see cref="SetColumns{T}"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="setColumns"/> is <c>null</c> or contains no columns;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>(this SetColumns<T>? setColumns) =>
        setColumns?.ColumnInfo.IsNullOrEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="SetColumns{T}"/> instance is
    /// not <c>null</c> and contains at least one column definition.
    /// </summary>
    /// <typeparam name="T">The entity or data model type used in the <see cref="SetColumns{T}"/> instance.</typeparam>
    /// <param name="setColumns">The <see cref="SetColumns{T}"/> instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="setColumns"/> is not <c>null</c> and contains at least one column;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this SetColumns<T>? setColumns) =>
        setColumns.IsNullOrEmpty() == false;
}
