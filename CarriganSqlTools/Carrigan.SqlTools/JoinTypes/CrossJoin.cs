using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>CROSS JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// JoinsBase join = Joins<Customer>.CrossJoin<Order>();
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// CROSS JOIN [Order]
/// ]]></code>
/// </example>
public class CrossJoin<rightT> : JoinBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CrossJoin{rightT}"/> class.
    /// </summary>
    public CrossJoin() : base(new EmptyPredicate())
    { }

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// a newly created <see cref="CrossJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing a single <see cref="CrossJoin{rightT}"/> instance.
    /// </returns>
    public static Joins<leftT> Joins<leftT>() =>
        new(new CrossJoin<rightT>());

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
    public Joins<leftT> AsJoins<leftT>() =>
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
    /// Converts the current <see cref="CrossJoin{rightT}"/> instance to its SQL representation.
    /// </summary>
    /// <param name="branchPrefix">
    /// The branch prefix used to distinguish parameters in join predicates from the main where clause.
    /// This value is ignored for <c>CROSS JOIN</c> because no <c>ON</c> clause is emitted.
    /// </param>
    /// <returns>
    /// A SQL string representing the <c>CROSS JOIN</c> clause.
    /// </returns>
    internal override string ToSql(string branchPrefix) =>
        $"CROSS JOIN {TableTag}";
}
