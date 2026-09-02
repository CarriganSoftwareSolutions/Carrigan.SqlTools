using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;


/// <summary>
/// Defines the contract for a column expression in SQL, providing access to the associated property name and methods to
/// convert the column expression to SQL fragments.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that defines the table containing the referenced column.
/// </typeparam>
public interface IColumnBase<T> : IColumnBase
{
    /// <summary>
    /// Gets the <see cref="PropertyName"/> that represents the C# property corresponding to the SQL column.
    /// </summary>
    public PropertyName PropertyName { get; }

    /// <summary>
    /// Converts the column expression to a collection of SQL fragments based on the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the SQL fragments.
    /// </param>
    /// <returns>
    /// A collection of <see cref="ISqlFragment"/> instances representing the SQL fragments for this column expression.
    /// </returns>
    public IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect);
}

/// <summary>
/// Defines the contract for a column expression in SQL, providing access to the associated <see cref="ColumnInfo"/>
/// and a string representation of the column.
/// </summary>
public interface IColumnBase
{
    /// <summary>
    /// Gets the resolved column metadata (name, tags, etc.) associated with this column expression.
    /// </summary>
    ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Returns the unquoted column tag representation as a string.
    /// </summary>
    /// <returns>
    /// A string representing the unquoted column tag of this column expression.
    /// </returns>
    string ToString();
}

