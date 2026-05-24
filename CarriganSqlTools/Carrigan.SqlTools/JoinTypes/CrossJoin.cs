using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: subquery, subqueries, intellisense

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>CROSS JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// CrossJoin<Order> join = new();
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// CROSS JOIN [Order]
/// ]]></code>
/// </example>
public class CrossJoin<rightT> : JoinBase where rightT : class
{
    /// <summary>
    /// Optional readonly subquery for the right-hand side.
    /// </summary>
    /// <remarks>Null when no subquery is provided. Assigned at construction and immutable
    /// thereafter.</remarks>
    private readonly SubQuery<rightT>? SubQuery;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossJoin{rightT}"/> class.
    /// </summary>
    /// <param name="subQuery">
    /// An optional <see cref="SubQuery{rightT}"/> to use as the right-hand side of the join instead of a 
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    public CrossJoin(SubQuery<rightT>? subQuery = null) : base(new EmptyPredicate()) =>
        SubQuery = subQuery;

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// a newly created <see cref="CrossJoin{rightT}"/> operation.
    /// </summary>
    /// <param name="subQuery">
    /// An optional <see cref="SubQuery{rightT}"/> to use as the right-hand side of the join instead of a 
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing a single <see cref="CrossJoin{rightT}"/> instance.
    /// </returns>
    public static Joins<leftT> Joins<leftT>(SubQuery<rightT>? subQuery = null) where leftT : class =>
        new(new CrossJoin<rightT>(subQuery));

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// the current <see cref="CrossJoin{rightT}"/> instance.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing the current <see cref="CrossJoin{rightT}"/> instance.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() where leftT : class =>
        new(this);

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the right-side table in the <c>CROSS JOIN</c> operation.
    /// </summary>
    /// <exception cref="TypeInitializationException">
    /// Thrown when the SQL reflection cache for <typeparamref name="rightT"/> fails to initialize.
    /// </exception>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Converts the current <see cref="CrossJoin{rightT}"/> instance to its <see cref="ISqlFragment"/> representation.
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns>
    /// A <see cref="ISqlFragment"/> representing the <c>CROSS JOIN</c> clause.
    /// </returns><see cref="ISqlFragment"/>
    /// <summary>
    /// Generates the SQL fragments representing the <c>CROSS JOIN</c> operation.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="ISqlFragment"/> objects that compose the SQL representation of this
    /// instance.</returns>
    /// <param name="branchPrefix">
    /// The branch prefix used to distinguish parameters in join predicates from the main where clause.
    /// This value is ignored for <c>CROSS JOIN</c> because no <c>ON</c> clause is emitted.
    /// </param>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect, string branchPrefix)
    {
        yield return new SqlFragmentText(" CROSS JOIN ");
        if (SubQuery is not null)
        {
            yield return SubQuery;
            yield return new SqlFragmentText(" AS ");
        }
        yield return TableTag;
    }
}
