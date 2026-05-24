using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: subquery, subqueries, intellisense

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>FULL JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates property names and throws an exception
/// if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// FullJoin<Order> join = new (predicate);
///
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// FULL JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class FullJoin<rightT> : JoinBase where rightT : class
{
    /// <summary>
    /// Optional readonly subquery for the right-hand side.
    /// </summary>
    /// <remarks>Null when no subquery is provided. Assigned at construction and immutable
    /// thereafter.</remarks>
    private readonly SubQuery<rightT>? SubQuery;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullJoin{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>FULL JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="SubQuery{rightT}"/> to use as the right-hand side of the join instead of a 
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public FullJoin(Predicates predicates, SubQuery<rightT>? subQuery = null) : base(predicates) =>
        SubQuery = subQuery;

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// a newly created <see cref="FullJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>FULL JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="SubQuery{rightT}"/> to use as the right-hand side of the join instead of a 
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing a single <see cref="FullJoin{rightT}"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public static Joins<leftT> Joins<leftT>(Predicates predicates, SubQuery<rightT>? subQuery = null) where leftT : class =>
        new(new FullJoin<rightT>(predicates, subQuery));

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// the current <see cref="FullJoin{rightT}"/> instance.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing the current <see cref="FullJoin{rightT}"/> instance.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() where leftT : class =>
        new(this);

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the right-side table in the <c>FULL JOIN</c> operation.
    /// </summary>
    /// <exception cref="TypeInitializationException">
    /// Thrown when the SQL reflection cache for <typeparamref name="rightT"/> fails to initialize.
    /// </exception>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Converts the current <see cref="FullJoin{rightT}"/> instance to its <see cref="ISqlFragment"/> representation.
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns>
    /// A <see cref="ISqlFragment"/> representing the <c>FULL JOIN</c> clause.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the join predicates render to an empty SQL expression, because <c>FULL JOIN</c> requires an <c>ON</c> clause.
    /// </exception>
    /// <remarks>
    /// Any exception thrown while rendering the predicate tree will be propagated to the caller.
    /// </remarks>
    /// <param name="branchPrefix">
    /// The branch prefix used to distinguish parameters in join predicates from the main where clause.
    /// </param>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect, string branchPrefix)
    {
        if (_predicates is null || _predicates is EmptyPredicate)
            throw new InvalidOperationException("FULL JOIN requires at least one predicate for the ON clause.");

        yield return new SqlFragmentText(" FULL JOIN ");
        if (SubQuery is not null)
        {
            yield return SubQuery;
            yield return new SqlFragmentText(" AS ");
        }
        yield return TableTag;
        yield return new SqlFragmentText(" ON ");
        foreach (ISqlFragment fragment in _predicates.ToSqlFragments(dialect))
            yield return fragment;
    }
}
