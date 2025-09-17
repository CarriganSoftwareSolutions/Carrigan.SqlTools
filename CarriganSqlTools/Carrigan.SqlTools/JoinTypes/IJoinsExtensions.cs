using Carrigan.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Just some extensions methods for IJoins.
/// </summary>
public static class IJoinsExtensions
{
    /// <summary>
    /// Is null or empty?
    /// </summary>
    /// <param name="joins"></param>
    /// <returns>True if null or empty, else false.</returns>
    public static bool IsNullOrEmpty(this IJoins? joins) =>
        joins?.Joints?.None() ?? true;
    /// <summary>
    /// Is not null and is not empty?
    /// </summary>
    /// <param name="joins"></param>
    /// <returns>False if null or empty, else true.</returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this IJoins? joins) =>
        joins.IsNullOrEmpty() == false;
}
