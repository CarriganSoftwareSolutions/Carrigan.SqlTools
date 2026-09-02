using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a reflected model property that maps to a boolean SQL column and can be used directly as a predicate.
/// </summary>
/// <typeparam name="T">The entity or data model type that defines the table containing the referenced boolean column.</typeparam>
/// <remarks>
/// This predicate is intended for SQL dialects that allow a boolean-valued column expression in predicate contexts.
/// The referenced C# property must be declared as <see cref="bool"/> or nullable <see cref="bool"/>.
/// </remarks>
public abstract class BooleanColumnBase<T> : Predicates, IColumnBase  where T : class
{
    /// <summary>
    /// The validated column expression represented by this predicate.
    /// </summary>
    private readonly ColumnBase<T> _column;

    public ColumnInfo ColumnInfo =>
        _column.ColumnInfo;

    /// <summary>
    /// Gets the C# property name that represents the boolean SQL column.
    /// </summary>
    //TODO: Unit test.
    internal PropertyName PropertyName => _column.PropertyName;

    /// <summary>
    /// Initializes a new <see cref="BooleanColumnBase{T}"/> instance from a reflected column expression.
    /// </summary>
    /// <param name="column">The column expression whose data model property must be <see cref="bool"/> or nullable <see cref="bool"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="column"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="column"/> does not represent a <see cref="bool"/> or nullable <see cref="bool"/> property.</exception>
    protected BooleanColumnBase(ColumnBase<T> column) : base([], column)
    {
        //IMPORTANT NOTE: Do not pass column to the base as a child node. That is wrong, and will cause duplicate Columns in the ancestry tree.
        //Ask me how I know, rhetorically speaking, don't actually ask.
        ArgumentNullException.ThrowIfNull(column, nameof(column));

        Type columnType = column.ColumnInfo.Type;
        if (columnType != typeof(bool) && columnType != typeof(bool?))
            throw new NonBooleanValueException($"{column.ColumnInfo.PropertyName} must represent a bool or bool? property on {typeof(T).Name} to be used as a boolean predicate.");

        _column = column;
    }

    /// <summary>
    /// Produces the SQL fragment represented by the underlying boolean column.
    /// </summary>
    /// <param name="dialect">The SQL dialect for which to generate the fragment.</param>
    /// <returns>The SQL fragment represented by the underlying boolean column.</returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        foreach (ISqlFragment fragment in _column.ToSqlFragments(dialect))
            yield return fragment;
    }
}
