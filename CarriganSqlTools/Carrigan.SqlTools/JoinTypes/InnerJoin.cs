using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;


/// <summary>
/// Represents an SQL <c>INNER JOIN</c> operation against the table modeled by
/// <typeparamref name="rightT"/>.
/// </summary>
/// <typeparam name="rightT">
/// The data model (right side of the join) for the table being joined.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// JoinsBase join = Joins<Customer>.InnerJoin<Order>(predicate);
/// 
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// INNER JOIN [Order] 
/// ON ([Customer].[Id] = [Order].[CustomerId]
/// ]]></code>
/// </example>
public class InnerJoin<rightT> : JoinBase
{
    private readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the <see cref="InnerJoin{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the <c>INNER JOIN</c>.
    /// </param>
    public InnerJoin(Predicates predicates) : base(predicates) => 
        _sql = $"INNER JOIN {TableTag} ON {predicates.ToSql()}";

    /// <summary>
    /// Creates a new <see cref="Joins{leftT}"/> that contains a newly created
    /// <see cref="InnerJoin{rightT}"/> using the supplied <paramref name="predicates"/>.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the <c>INNER JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="InnerJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> Joins<leftT>(Predicates predicates) =>
        new(new InnerJoin<rightT>(predicates));

    /// <summary>
    /// Wraps the current <see cref="InnerJoin{rightT}"/> in a new <see cref="Joins{leftT}"/>.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing this <see cref="InnerJoin{rightT}"/>.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with <typeparamref name="rightT"/>,
    /// used as the right side of the <c>INNER JOIN</c>.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Converts this <see cref="InnerJoin{rightT}"/> to its SQL representation.
    /// </summary>
    /// <returns>
    /// A SQL string representing the <c>INNER JOIN</c> clause, including the <c>ON</c> predicate(s).
    /// </returns>
    internal override string ToSql() =>
        _sql;
}
