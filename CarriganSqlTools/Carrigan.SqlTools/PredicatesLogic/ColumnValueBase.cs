
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents the predicate <c>Column = Value</c> for use in SQL <c>WHERE</c> or <c>JOIN</c> conditions.
/// This class is a convenience wrapper that reduces the boilerplate required to compare a column
/// against a constant value using the SQL equality operator (<c>=</c>).
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
/// <remarks>
/// Property name validation is performed during construction. If the provided property name does not
/// map to a valid, eligible property on <typeparamref name="T"/>, an exception will be thrown.
/// </remarks>
public abstract class ColumnValueBase<T> : Predicates where T : class
{
    /// <summary>
    /// The composed predicate (e.g., <c>[T].[Column] = @Parameter_Column</c>) that this class builds.
    /// </summary>
    protected readonly Predicates value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValueBase{T}"/> class,
    /// representing a predicate that compares a column to a constant value using
    /// the SQL equality operator (<c>=</c>).
    /// </summary>
    /// <param name="left">
    /// The left-hand side of the predicate, representing the column to compare. This is used to construct
    /// </param>
    /// <param name="parameterValue">
    /// The constant value to compare against the column in the generated SQL.
    /// </param>

    public ColumnValueBase(ColumnBase<T> left, object? parameterValue) : this(CreateValue(left, parameterValue))
    {
    }

    /// <summary>
    /// Private constructor used to ensure the base <see cref="Predicates"/> type is constructed
    /// with the composed predicate as a child node.
    /// </summary>
    /// <param name="equal">The composed <see cref="Equal"/> predicate.</param>
    private ColumnValueBase(Equal equal) : base([equal]) =>
        value = equal;

    /// <summary>
    /// Factory method that creates the <see cref="Equal"/> predicate comparing the column to the parameter value.
    /// </summary>
    /// <param name="left">
    /// The left-hand side of the predicate, representing the column to compare. This is used to construct
    /// </param>
    /// <param name="parameterValue">
    /// The constant value to compare against the column in the generated SQL. This is used to construct
    /// </param>
    /// <returns></returns>
    protected static Equal CreateValue(ColumnBase<T> left, object? parameterValue)
    {
        Parameter right = new(parameterValue, left.ColumnInfo.ParameterTag);

        return new Equal(left, right);
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>

    /// <returns>
    /// The SQL fragment represented by this predicate, e.g., <c>[T].[Column] = @Parameter_Column</c>.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        foreach (ISqlFragment fragment in value.ToSqlFragments(dialect))
            yield return fragment;
    }
}
