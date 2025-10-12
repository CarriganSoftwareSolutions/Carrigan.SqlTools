using Carrigan.Core.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.JoinTypes;
//TODO: proof read documentation
/// <summary>
/// Provides extension methods for the <see cref="Relations"/> class.
/// </summary>
internal static class RelationsExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="Relations"/> instance is <c>null</c>
    /// or contains no join elements.
    /// </summary>
    /// <typeparam name="leftT"></typeparam>
    /// <param name="relation">The <see cref="Relations"/> instance to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="relation"/> is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    internal static bool IsNullOrEmpty(this Relations? relation) =>
        relation?.IsEmpty() ?? true;

    /// <summary>
    /// Determines whether the specified <see cref="Relations"/> instance is not null
    /// and contains at least one join element.
    /// </summary>
    /// <typeparam name="leftT"></typeparam>
    /// <param name="relation">The <see cref="Relations"/> instance to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="relation"/> is not <c>null</c> and contains at least one element; otherwise, <c>false</c>.</returns>

    internal static bool IsNotNullOrEmpty([NotNullWhen(true)] this Relations? relation) =>
        relation.IsNullOrEmpty() == false;
}
