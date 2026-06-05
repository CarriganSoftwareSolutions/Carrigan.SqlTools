using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: subquery, subqueries, intellisense

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>INNER JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumnBase{leftT, rightT}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new (predicate);
///
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// INNER JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class InnerJoin<rightT> : JoinBase where rightT : class
{
    /// <summary>
    /// Optional readonly subquery for the right-hand side.
    /// </summary>
    /// <remarks>Null when no subquery is provided. Assigned at construction and immutable
    /// thereafter.</remarks>
    private readonly Subquery<rightT>? Subquery;

    /// <summary>
    /// Initializes a new instance of the <see cref="InnerJoin{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery{rightT}"/> to use as the right-hand side of the join instead of a
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public InnerJoin(Predicates predicates, Subquery<rightT>? subQuery = null) : base(predicates) =>
        Subquery = subQuery;

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// a newly created <see cref="InnerJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery{rightT}"/> to use as the right-hand side of the join instead of a
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing a single <see cref="InnerJoin{rightT}"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public static Joins<leftT> Joins<leftT>(Predicates predicates, Subquery<rightT>? subQuery = null) where leftT : class =>
        new(new InnerJoin<rightT>(predicates, subQuery));

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// the current <see cref="InnerJoin{rightT}"/> instance.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing the current <see cref="InnerJoin{rightT}"/> instance.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() where leftT : class =>
        new(this);

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the right-side table in the <c>INNER JOIN</c> operation.
    /// </summary>
    /// <exception cref="TypeInitializationException">
    /// Thrown when the SQL reflection cache for <typeparamref name="rightT"/> fails to initialize.
    /// </exception>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Converts the current <see cref="InnerJoin{rightT}"/> instance to its <see cref="ISqlFragment"/> representation.
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns>
    /// A <see cref="ISqlFragment"/> representing the <c>INNER JOIN</c> clause.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the join predicates render to an empty SQL expression, because <c>INNER JOIN</c> requires an <c>ON</c> clause.
    /// </exception>
    /// <remarks>
    /// Any exception thrown while rendering the predicate tree or while resolving <see cref="TableTag"/> will be propagated to the caller.
    /// </remarks>
    ///
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        if (_predicates is null || _predicates is EmptyPredicate)
            throw new InvalidOperationException("INNER JOIN requires at least one predicate for the ON clause.");

        yield return new SqlFragmentText(" INNER JOIN ");
        if (Subquery is not null)
        {
            yield return Subquery;
            yield return new SqlFragmentText(" AS ");
        }
        yield return TableTag;
        yield return new SqlFragmentText(" ON ");
        foreach (ISqlFragment fragment in _predicates.ToSqlFragments(dialect))
            yield return fragment;
    }
}
