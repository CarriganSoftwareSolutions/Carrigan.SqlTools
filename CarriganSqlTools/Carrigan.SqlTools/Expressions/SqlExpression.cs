using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.Tags;
using System.Numerics;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a node in a SQL expression tree that can be rendered for a specific dialect.
/// </summary>
public abstract class SqlExpression : IEquatable<SqlExpression>, IEqualityOperators<SqlExpression, SqlExpression, bool>
{
    /// <summary>
    /// Represents the direct children of the current expression.
    /// </summary>
    internal IEnumerable<SqlExpression> ChildNodes { get; }

    /// <summary>
    /// Base constructor for all expression classes.
    /// </summary>
    /// <param name="childExpressions">Represents all child nodes for a given expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected SqlExpression(IEnumerable<SqlExpression> childExpressions)
    {
        ArgumentNullException.ThrowIfNull(childExpressions, nameof(childExpressions));

        ChildNodes = childExpressions.Materialize(NullOptionsEnum.Exception);
    }

    /// <summary>
    /// Gets all parameter expressions reachable below the current expression node.
    /// </summary>
    // TODO: still need?
    internal IEnumerable<Parameter> DescendantParameters =>
        DescendantNodes.OfType<Parameter>();

    /// <summary>
    /// Retrieves all descendant expressions regardless of type.
    /// </summary>
    internal IEnumerable<SqlExpression> DescendantNodes =>
        GetAllDescendantExpressions(ChildNodes);

    /// <summary>
    /// Retrieves all descendants of type <see cref="IColumnBase"/>.
    /// </summary>
    // TODO: Still need?
    internal IEnumerable<IColumnBase> DescendantColumns =>
        DescendantNodes.OfType<IColumnBase>();

    /// <summary>
    /// Gets the table tags represented by leaf expressions directly attached to this expression.
    /// </summary>
    public virtual IEnumerable<TableTag> LeafTables => [];

    /// <summary>
    /// Gets all table tags represented by leaf expressions anywhere underneath this expression.
    /// </summary>
    public IEnumerable<TableTag> DescendantLeafTables =>
        LeafTables
            .Concat(DescendantNodes.SelectMany(static expression => expression.LeafTables))
            .Distinct();

    /// <summary>
    /// Indicates whether this expression is valid in an aggregate SELECT list for the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <returns><c>false</c> unless an expression type overrides this method.</returns>
    public virtual bool IsAggregate() =>
        false;

    /// <summary>
    /// Aggregate functions are valid aggregate SELECT expressions.
    /// </summary>
    /// <param name="groupBys">The optional <c>GROUP BY</c> clause.</param>
    /// <returns>Always <c>true</c>.</returns>
    [Obsolete("Use the overload with no parameters instead.")]
    public bool IsAggregate(GroupBysBase? groupBys) =>
        IsAggregate();

    /// <summary>
    /// Indicates whether this expression tree contains any aggregate expressions.
    /// </summary>
    /// <returns><c>true</c> if the expression tree contains any aggregate expressions; otherwise, <c>false</c>.</returns>
    public bool ContainsAggregate() =>
        IsAggregate() || ChildNodes.Any(ContainsAggregate);

    /// <summary>
    /// Indicates whether the specified expression tree contains any aggregate expressions. 
    /// </summary>
    /// <param name="expression">
    /// The expression tree to check for aggregate expressions.
    /// </param>
    /// <returns><c>true</c> if the expression tree contains any aggregate expressions; otherwise, <c>false</c>.</returns>
    public static bool ContainsAggregate(SqlExpression expression) =>
        expression.ContainsAggregate();

    /// <summary>
    /// Gets the equality contract shared by expressions that represent the same semantic SQL construct.
    /// </summary>
    /// <remarks>
    /// Most expression types use their runtime type. Expression families that have aliases or multiple strongly typed
    /// representations override this value so equivalent SQL constructs can compare structurally.
    /// </remarks>
    protected virtual object EqualityContract =>
        this is IColumnExpressionIdentity ? typeof(IColumnExpressionIdentity) :
        this is IParameter ? typeof(IParameter) :
        GetType();

    /// <summary>
    /// Compares the state specific to this expression after the equality contract has been matched.
    /// </summary>
    protected virtual bool EqualsCore(SqlExpression other)
    {
        if (this is IColumnExpressionIdentity leftColumn && other is IColumnExpressionIdentity rightColumn)
            return leftColumn.EqualityColumnTag.Equals(rightColumn.EqualityColumnTag);

        if (this is IParameter leftParameter && other is IParameter rightParameter)
            return leftParameter.Name.Equals(rightParameter.Name);

        return ChildNodes.SequenceEqual(other.ChildNodes);
    }

    /// <summary>
    /// Adds the state used by <see cref="EqualsCore(SqlExpression)"/> to the supplied hash code.
    /// </summary>
    protected virtual void AddToHashCode(ref HashCode hashCode)
    {
        if (this is IColumnExpressionIdentity column)
        {
            hashCode.Add(column.EqualityColumnTag);
            return;
        }

        if (this is IParameter parameter)
        {
            hashCode.Add(parameter.Name);
            return;
        }

        foreach (SqlExpression childNode in ChildNodes)
            hashCode.Add(childNode);
    }

    /// <summary>
    /// Determines whether this expression is structurally equivalent to another SQL expression.
    /// </summary>
    public bool Equals(SqlExpression? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null || EqualityContract.Equals(other.EqualityContract) == false)
            return false;

        return EqualsCore(other);
    }

    /// <summary>
    /// Determines whether the specified object is a structurally equivalent SQL expression.
    /// </summary>
    public override bool Equals(object? obj) =>
        Equals(obj as SqlExpression);

    /// <summary>
    /// Returns a hash code based on the expression's semantic equality contract and structural state.
    /// </summary>
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(EqualityContract);
        AddToHashCode(ref hashCode);
        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Determines whether two SQL expressions are structurally equivalent.
    /// </summary>
    public static bool operator ==(SqlExpression? left, SqlExpression? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two SQL expressions are structurally different.
    /// </summary>
    public static bool operator !=(SqlExpression? left, SqlExpression? right) =>
        (left == right) == false;

    /// <summary>
    /// Returns a dialect-neutral SQL representation of this expression by rendering its SQL fragments through the neutral diagnostic dialect.
    /// </summary>
    public override string ToString() =>
        ToSqlFragments(NeutralDialect.Instance).ToSql(NeutralDialect.Instance);

    /// <summary>
    /// Generates the SQL fragments for this expression tree using the supplied SQL dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render dialect-dependent fragments.</param>
    /// <returns>The SQL fragments represented by this expression tree.</returns>
    public abstract IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect);

    /// <summary>
    /// Gets the SQL parameters contained by this expression tree.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the fragments.</param>
    /// <returns>The SQL fragment parameters required to render this expression tree.</returns>
    internal IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        ToSqlFragments(dialect).SelectMany(sqlFragment => sqlFragment.GetSqlFragmentParameters(dialect));


    /// <summary>
    /// Recursively enumerates every child expression below the supplied expression collection.
    /// </summary>
    /// <param name="expressions">The expression collection whose descendants should be enumerated.</param>
    /// <returns>All descendant expression nodes in depth-first order.</returns>
    private static IEnumerable<SqlExpression> GetAllDescendantExpressions(IEnumerable<SqlExpression> expressions)
    {
        foreach (SqlExpression expression in expressions)
        {
            yield return expression;

            foreach (SqlExpression childExpression in GetAllDescendantExpressions(expression.ChildNodes))
                yield return childExpression;
        }
    }
}
