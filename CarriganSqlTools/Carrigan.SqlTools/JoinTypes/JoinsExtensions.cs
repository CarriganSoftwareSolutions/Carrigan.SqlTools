using Carrigan.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;
//TODO: proof read documentation
/// <summary>
/// Provides extension methods for the <see cref="Joins{leftT}"/> class.
/// </summary>
internal static class JoinsExtensions
{

    /// <summary>
    /// Determines whether the specified <see cref="Joins{leftT}"/> instance is <c>null</c>
    /// or contains no join elements.
    /// </summary>
    /// <typeparam name="leftT"></typeparam>
    /// <param name="joins">The <see cref="Joins{leftT}"/> instance to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="joins"/> is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    internal static bool IsNullOrEmpty<leftT>(this Joins<leftT>? joins) =>
        joins?.Joints?.None() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="Joins{leftT}"/> instance is not null
    /// and contains at least one join element.
    /// </summary>
    /// <typeparam name="leftT"></typeparam>
    /// <param name="joins">The <see cref="Joins{leftT}"/> instance to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="joins"/> is not <c>null</c> and contains at least one element; otherwise, <c>false</c>.</returns>

    internal static bool IsNotNullOrEmpty<leftT>([NotNullWhen(true)] this Joins<leftT>? joins) =>
        joins.IsNullOrEmpty() == false;
}
