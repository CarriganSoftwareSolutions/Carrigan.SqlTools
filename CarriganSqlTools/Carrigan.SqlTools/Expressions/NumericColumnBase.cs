using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a reflected model property that maps to a numeric SQL column and can be used directly as a predicate.
/// </summary>
/// <typeparam name="T">The entity or data model type that defines the table containing the referenced numeric column.</typeparam>
/// <remarks>
/// This predicate is intended for SQL dialects that allow a numeric-valued column expression in predicate contexts.
/// The referenced C# property must be declared as a numeric type or nullable a numeric type.
/// Note: Note numeric type is being used in a broader sense (i.e. short, int, long, float, double, decimal)
/// as opposed to a literal programming language type (ex: numeric is a type postgre sql)
/// </remarks>
public abstract class NumericColumnBase<T> : NumericExpression where T : class
{
    /// <summary>
    /// The validated column expression represented by this predicate.
    /// </summary>
    private readonly ColumnBase<T> _column;

    /// <summary>
    /// Gets the <see cref="ColumnInfo"/> of the reflected model property represented by this predicate.
    /// </summary>
    internal ColumnInfo ColumnInfo =>
        _column.ColumnInfo;

    /// <summary>
    /// Gets the <see cref="PropertyName"/> of the reflected model property represented by this predicate.
    /// </summary>
    //TODO: Unit test
    internal PropertyName PropertyName => _column.PropertyName;

    /// <summary>
    /// Initializes a new <see cref="NumericColumnBase{T}"/> instance from a reflected column expression.
    /// </summary>
    /// <param name="column">The column expression whose data model property must be of a numeric type or nullable numeric type.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="column"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="column"/> does not represent a numeric type or nullable numeric type property.</exception>
    protected NumericColumnBase(ColumnBase<T> column) : base([ValidateColumn(column)], column) =>
        _column = column;

    /// <summary>
    /// Validates that the supplied column represents a numeric type or nullable numeric type property in the data model.
    /// </summary>
    /// <param name="column">The column expression to validate.</param>
    /// <returns>The validated column expression.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="column"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the reflected property type is not numeric type or nullable numeric type.</exception>
    private static ColumnBase<T> ValidateColumn(ColumnBase<T> column)
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));

        Type columnType = column.ColumnInfo.Type;
        if (columnType.IsNumericType() is  false)
            throw new NonNumericValueException($"{column.ColumnInfo.PropertyName} must represent a numeric property on {typeof(T).Name} to be used as a numeric predicate.");

        return column;
    }

    /// <summary>
    /// Produces the SQL fragment represented by the underlying numeric column.
    /// </summary>
    /// <param name="dialect">The SQL dialect for which to generate the fragment.</param>
    /// <returns>The SQL fragment represented by the underlying numeric column.</returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        foreach (ISqlFragment fragment in _column.ToSqlFragments(dialect))
            yield return fragment;
    }
}
