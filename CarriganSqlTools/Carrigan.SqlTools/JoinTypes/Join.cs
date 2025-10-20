using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents a general SQL <c>JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn&lt;Customer, Order&gt; predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// JoinsBase join = Joins<Customer>.Join<Order>(predicate);
/// 
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// JOIN [Order] 
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class Join<rightT> : JoinBase
{
    /// <summary>
    /// The SQL representation of the <c>JOIN</c> clause.
    /// </summary>
    protected readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicate">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    public Join(Predicates predicate) : base(predicate) =>
        _sql = $"JOIN {TableTag} ON {predicate.ToSql()}";

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object containing
    /// a newly created <see cref="Join{rightT}"/> instance.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <param name="predicate">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> instance containing a <see cref="Join{rightT}"/>.
    /// </returns>
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new(new Join<rightT>(predicate));

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object containing
    /// a newly created <see cref="Join{rightT}"/> instance.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <param name="predicate">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> instance containing a <see cref="Join{rightT}"/>.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    /// <summary>
    /// Gets the <see cref="Tags.TableTag"/> representing the right-side table
    /// associated with <typeparamref name="rightT"/>.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Converts the current <see cref="Join{rightT}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>
    /// A SQL string representing the <c>JOIN</c> clause and its corresponding <c>ON</c> predicate.
    /// </returns>
    internal override string ToSql() =>
        _sql;
}
