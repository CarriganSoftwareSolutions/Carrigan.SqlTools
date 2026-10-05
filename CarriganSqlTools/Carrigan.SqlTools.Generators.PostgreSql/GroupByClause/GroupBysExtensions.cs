using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Provides strongly typed convenience methods for appending model properties to a <see cref="GroupBys"/> collection.
/// </summary>
public static class GroupBysExtensions
{

    /// <summary>
    /// Appends a group-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The table model whose C# properties represent SQL columns.</typeparam>
    /// <param name="groupBys">The existing grouping collection.</param>
    /// <param name="propertyName">The C# property name representing the SQL column.</param>
    /// <returns>A new collection containing the additional group-by item.</returns>
    public static GroupBys Append<T>(this GroupBys groupBys, PropertyName propertyName) where T : class =>
        new (groupBys.GroupByItems.Append(new GroupBy<T>(propertyName)));

    /// <summary>
    /// Appends a group-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The table model whose C# properties represent SQL columns.</typeparam>
    /// <param name="groupBys">The existing grouping collection.</param>
    /// <param name="propertyName">The C# property name representing the SQL column.</param>
    /// <returns>A new collection containing the additional group-by item.</returns>
    public static GroupBys Append<T>(this GroupBys groupBys, string propertyName) where T : class =>
        groupBys.Append<T>(new PropertyName(propertyName));
}
