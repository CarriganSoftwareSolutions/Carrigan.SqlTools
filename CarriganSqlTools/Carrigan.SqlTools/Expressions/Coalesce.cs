using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL COALESCE expression that returns the first non-null value from a list of expressions.
/// </summary>
public class Coalesce : SqlExpression
{
    /// <summary>
    /// Gets the list of values to evaluate in the COALESCE expression.
    /// </summary>
    private IEnumerable<SqlExpression> Values { get; init; }

    /// <summary>
    /// Gets the leaf tables involved in the COALESCE expression.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coalesce"/> class with the specified values.
    /// </summary>
    /// <param name="values">
    /// An array of <see cref="SqlExpression"/> instances representing the values to evaluate in the COALESCE expression. Must contain at least two values.
    /// </param>
    public Coalesce(params IEnumerable<SqlExpression> values) : base([])
    {
        IEnumerable<TableTag> GetLeafTables()
        {
            foreach (SqlExpression sqlExpression in Values)
                foreach (TableTag tableTag in sqlExpression.LeafTables)
                    yield return tableTag;
        }

        Values = ValidateValues(values);
        LeafTables = GetLeafTables();
    }

    /// <summary>
    /// Validates the provided values for the COALESCE expression, ensuring that there are at least two values.
    /// </summary>
    /// <param name="values">
    /// An enumerable of <see cref="SqlExpression"/> instances to validate.
    /// </param>
    /// <returns>
    /// The validated enumerable of <see cref="SqlExpression"/> instances.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the number of values is less than two.
    /// </exception>
    private static IEnumerable<SqlExpression> ValidateValues(IEnumerable<SqlExpression> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Count() < 2)
            throw new ArgumentException("Coalesce requires two or more values.");

        return values;
    }

    /// <summary>
    /// Converts the COALESCE expression into a sequence of SQL fragments based on the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> instances representing the SQL fragments of the COALESCE expression.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("COALESCE(");
        foreach(ISqlFragment sqlFragment in Values.Select(value => (ISqlFragment) value).JoinFragments(ISqlFragment.CommaSpace))
        {
            yield return sqlFragment;
        }
        yield return ISqlFragment.CloseParentheses;
    }

    /// <summary>
    /// Determines whether the COALESCE expression is an aggregate expression based on the aggregate status of its values.
    /// </summary>
    /// <returns>
    /// A boolean indicating whether the COALESCE expression is an aggregate expression. Returns true if all values are aggregate expressions; otherwise, false. 
    /// If the aggregate status of the values is inconsistent, an <see cref="AggregateInconsistencyException"/> is thrown.
    /// </returns>
    /// <exception cref="AggregateInconsistencyException">
    /// Thrown if the aggregate status of the values is inconsistent (i.e., some values are aggregate expressions while others are not).
    /// </exception>
    public override bool IsAggregate()
    {
        if(Values.Select(value => value.IsAggregate()).AllEqual() ?? false)
        {
            return Values.First().IsAggregate();
        }
        else
        {
            throw new AggregateInconsistencyException();
        }
    }
}
