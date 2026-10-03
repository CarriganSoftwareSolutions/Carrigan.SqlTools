using Carrigan.Core.Attributes;
using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PredicatesLogic;
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
    internal readonly IEnumerable<SqlExpression> ChildNodes;

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

        ChildNodes = childExpressions.Materialize(NullOptionsEnum.ArgumentNullException);
    }

    /// <summary>
    /// Retrieves all descendant expressions regardless of type.
    /// </summary>
    internal IEnumerable<SqlExpression> DescendantNodes =>
        GetAllDescendantExpressions(ChildNodes);

    #region parameters
    /// <summary>
    /// Gets every parameter expression participating in this expression tree, including the current node and all descendants.
    /// </summary>
    /// <remarks>
    /// Root + Descendants 
    /// Preserves expression-tree traversal order.
    /// </remarks>
    public IEnumerable<IParameter> AllParticipatingParameters =>
        EnumerateSelfAndDescendants().OfType<IParameter>();

    /// <summary>
    /// Gets all parameter expressions below the current node.
    /// </summary>
    [Obsolete("This is likely not doing what we need it to do anymore.")]
    internal IEnumerable<Parameter> DescendantParameters =>
        DescendantNodes.OfType<Parameter>();
    /// <summary>
    /// Indicates whether this expression tree contains any parameter expressions.
    /// </summary>
    /// <returns>
    /// <c>true</c> when this expression or any child expression represents a parameter; otherwise, <c>false</c>.
    /// </returns>
    internal bool HasParameters() =>
        AllParticipatingParameters.Any();
    #endregion

    #region columns
    /// <summary>
    /// Gets every column-shaped SQL expression participating in this expression tree, including the current node and all descendants.
    /// </summary>
    /// <remarks>
    /// Root + Descendants
    /// The returned expressions are the actual participating <see cref="Column"/> nodes. Enumeration is deferred
    /// and preserves expression-tree traversal order.
    /// </remarks>
    public IEnumerable<SqlExpression> AllParticipatingColumns =>
        EnumerateSelfAndDescendants()
            .Where(static expression => expression is Column);

    /// <summary>
    /// Gets every column-shaped SQL expression participating in this expression tree, excluding any aggregate expressions.
    /// </summary>
    internal IEnumerable<SqlExpression> AllNonAggregateColumns =>
        EnumerateAllNonAggregates()
            .Where(static expression => expression is Column);

    /// <summary>
    /// Gets all reflected column expressions below the current node.
    /// </summary>
    [Obsolete("This is likely not doing what we need it to do anymore.")]
    internal IEnumerable<Column> DescendantColumns =>
        DescendantNodes.OfType<Column>();

    /// <summary>
    /// Indicates whether this expression tree contains any column expressions.
    /// </summary>
    /// <returns>
    /// <c>true</c> when this expression or any child expression represents a column; otherwise, <c>false</c>.
    /// </returns>
    public bool HasColumns() =>
        AllParticipatingColumns.Any();
    #endregion

    #region tables

    /// <summary>
    /// Gets the table tags represented by leaf expressions directly attached to this expression.
    /// </summary>
    /// <remarks>
    /// Tables are not expressions, and are not actually in the tree.
    /// This is why they are treated as Leafs
    /// </remarks>
    public virtual IEnumerable<TableTag> LeafTables => [];

    /// <summary>
    /// Gets every table participating in this expression tree, including tables represented by the current node and all descendants.
    /// </summary>
    /// <remarks>
    /// Root + Descendants
    /// Duplicate table tags are removed while preserving the order in which the tables are first encountered.
    /// </remarks>
    public IEnumerable<TableTag> AllParticipatingTables =>
        EnumerateSelfAndDescendants()
            .SelectMany(static expression => expression.LeafTables)
            .Distinct();

    /// <summary>
    /// Gets all table tags represented by this expression and its descendants.
    /// </summary>
    [Obsolete("This is likely not doing what we need it to do anymore.")]
    public IEnumerable<TableTag> DescendantLeafTables =>
        AllParticipatingTables;
    #endregion

    #region aggregates
    /// <summary>
    /// Indicates whether this expression is an aggregate expression.
    /// </summary>
    /// <returns>
    /// <c>true</c> when this expression is an aggregate expression; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAggregate() =>
        this is Aggregates;

    /// <summary>
    /// Indicates whether this expression is not an aggregate expression.
    /// </summary>
    /// <returns>
    /// <c>true</c> when this expression is not an aggregate expression; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNotAggregate() =>
        IsAggregate() is false;

    /// <summary>
    /// Aggregate functions are valid aggregate SELECT expressions.
    /// </summary>
    /// <param name="groupBys">The optional <c>GROUP BY</c> clause.</param>
    /// <returns>Always <c>true</c>.</returns>
    [Obsolete("Use IsAggregate() instead.")]
    public bool IsAggregate(GroupBys? groupBys) =>
        IsAggregate();

    /// <summary>
    /// Indicates whether this expression tree contains any aggregate expressions.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the expression tree contains any aggregate expressions; otherwise, <c>false</c>.
    /// </returns>
    public bool HasAggregates() =>
        IsAggregate() || DescendantNodes.Any(child => child.IsAggregate());

    /// <summary>
    /// Indicates whether this expression tree contains any aggregate expressions.
    /// </summary>
    /// <returns><c>true</c> if the expression tree contains any aggregate expressions; otherwise, <c>false</c>.</returns>
    [Obsolete("Use HasAggregates() instead.")]
    public bool ContainsAggregate() =>
        HasAggregates();

    /// <summary>
    /// Indicates whether the specified expression tree contains any aggregate expressions.
    /// </summary>
    /// <param name="expression">
    /// The expression tree to check for aggregate expressions.
    /// </param>
    /// <returns><c>true</c> if the expression tree contains any aggregate expressions; otherwise, <c>false</c>.</returns>
    [Obsolete("Use the instance method HasAggregates() instead.")]
    public static bool ContainsAggregate(SqlExpression expression) =>
        expression.HasAggregates();
    #endregion

    /// <summary>
    /// Gets the canonical expression used for equality and hashing.
    /// </summary>
    /// <remarks>
    /// Transparent adapter expressions override this member to return the expression they wrap so the adapter does not create a new semantic identity.
    /// </remarks>
    protected virtual SqlExpression EqualityExpression =>
        this;

    /// <summary>
    /// Gets the equality contract shared by expressions that represent the same semantic SQL construct.
    /// </summary>
    /// <remarks>
    /// Most expression types use their runtime type. Expression families that have aliases or multiple strongly typed
    /// representations override this value so equivalent SQL constructs can compare structurally.
    /// </remarks>
    protected virtual object EqualityContract =>
        this is Column ? typeof(Column) :
        this is IParameter ? typeof(IParameter) :
        GetType();

    /// <summary>
    /// Compares the state specific to this expression after the equality contract has been matched.
    /// </summary>
    protected virtual bool EqualsCore(SqlExpression other)
    {
        if (this is Column leftColumn && other is Column rightColumn)
            return leftColumn.ColumnInfo.ColumnTag.Equals(rightColumn.ColumnInfo.ColumnTag);

        if (this is IParameter leftParameter && other is IParameter rightParameter)
            return leftParameter.Name.Equals(rightParameter.Name);

        return ChildNodes.SequenceEqual(other.ChildNodes);
    }

    /// <summary>
    /// Adds the state used by <see cref="EqualsCore(SqlExpression)"/> to the supplied hash code.
    /// </summary>
    protected virtual void AddToHashCode(ref HashCode hashCode)
    {
        if (this is Column column)
        {
            hashCode.Add(column.ColumnInfo.ColumnTag);
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

        if (other is null)
            return false;

        SqlExpression leftExpression = GetEqualityExpression();
        SqlExpression rightExpression = other.GetEqualityExpression();

        if (ReferenceEquals(leftExpression, rightExpression))
            return true;

        if (leftExpression.EqualityContract.Equals(rightExpression.EqualityContract) == false)
            return false;

        return leftExpression.EqualsCore(rightExpression);
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
        SqlExpression equalityExpression = GetEqualityExpression();

        if (ReferenceEquals(this, equalityExpression) == false)
            return equalityExpression.GetHashCode();

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
    /// Resolves transparent equality adapters to the underlying expression that defines their semantic identity.
    /// </summary>
    private SqlExpression GetEqualityExpression()
    {
        SqlExpression expression = this;
        SqlExpression equalityExpression = expression.EqualityExpression;

        while (ReferenceEquals(expression, equalityExpression) == false)
        {
            expression = equalityExpression;
            equalityExpression = expression.EqualityExpression;
        }

        return expression;
    }


    /// <summary>
    /// Enumerates this expression followed by every descendant expression in depth-first order.
    /// </summary>
    private IEnumerable<SqlExpression> EnumerateSelfAndDescendants()
    {
        yield return this;

        foreach (SqlExpression descendant in DescendantNodes)
            yield return descendant;
    }
    /// <summary>
    /// Enumerates this expression followed by every descendant expression in depth-first order.
    /// </summary>
    private IEnumerable<SqlExpression> EnumerateAllNonAggregates()
    {
        if (IsNotAggregate())
        {
            yield return this;

            foreach (SqlExpression descendant in GetAllDescendantNonAggregateExpressions(this.ChildNodes))
                yield return descendant;
        }
    }

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

    /// <summary>
    /// Recursively enumerates every child expression below the supplied expression collection, excluding any aggregate expressions.
    /// </summary>
    /// <param name="expressions">
    /// The expression collection whose non-aggregate descendants should be enumerated.
    /// </param>
    /// <returns>
    /// All descendant non-aggregate expression nodes in depth-first order.
    /// </returns>
    private static IEnumerable<SqlExpression> GetAllDescendantNonAggregateExpressions(IEnumerable<SqlExpression> expressions)
    {
        foreach (SqlExpression expression in expressions)
        {
            if (expression.IsNotAggregate())
            {
                yield return expression;

                foreach (SqlExpression childExpression in GetAllDescendantNonAggregateExpressions(expression.ChildNodes))
                    yield return childExpression;
            }
        }
    }

    /// <summary>
    /// Wraps this expression in a <see cref="Predicates"/> adapter without validating that the expression represents a predicate.
    /// </summary>
    /// <returns>A transparent <see cref="Predicates"/> adapter over this expression.</returns>
    [TypeSafetyLoss]
    public Predicates AsPredicate() =>
        new PredicateWrapper(this);

    #region child validators
    /// <summary>
    /// Validates the provided values for the specified function, ensuring that the number of arguments meets the minimum requirement.
    /// </summary>
    /// <param name="minArguments">The expressions to validate.</param>
    /// <param name="sqlExpressions">The expressions to validate.</param>
    /// <returns>A materialized sequence containing the validated expressions.</returns>
    protected static IEnumerable<SqlExpression> ValidateValues(int minArguments, IEnumerable<SqlExpression> sqlExpressions)
    {
        ArgumentNullException.ThrowIfNull(sqlExpressions, nameof(sqlExpressions));

        if (sqlExpressions.Count() < minArguments)
            throw new ArgumentException($"Scalar function requires {minArguments} or more expressions.", nameof(sqlExpressions));

        return sqlExpressions;
    }

    /// <summary>
    /// Validates that the provided SQL expression is not null.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SQL expression to validate.
    /// </param>
    /// <returns>
    /// The validated SQL expression.
    /// </returns>
    protected static SqlExpression ValidateValue(SqlExpression sqlExpression)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));
        return sqlExpression;
    }

    /// <summary>
    /// Validates that the provided SQL expressions are not null and not empty.
    /// </summary>
    /// <param name="sqlExpressions">
    /// The SQL expressions to validate.
    /// </param>
    /// <returns>
    /// A materialized sequence containing the validated expressions.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpressions"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="sqlExpressions"/> is empty.
    /// </exception>
    protected static IEnumerable<SqlExpression> ValidateValues(params IEnumerable<SqlExpression> sqlExpressions) => 
        ValidateValues(1, sqlExpressions);

    /// <summary>
    /// Validates that the provided value is not null and wraps it in a <see cref="Parameter"/> expression.
    /// </summary>
    /// <param name="value">
    /// The value to validate and wrap in a <see cref="Parameter"/> expression.
    /// </param>
    /// <returns>
    /// A <see cref="Parameter"/> expression containing the validated value.
    /// </returns>
    protected static Parameter ValidateParameterValue(object value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return new Parameter(value);
    }
    #endregion
}
