using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

//TODO: proof read Documentation
/// <summary>
/// Represents an SQL <c>JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model for the table being joined on.
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
    protected readonly string _sql;


    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>

    public Join(Predicates predicate) : base(predicate) =>
        _sql = $"JOIN {TableTag} ON {predicate.ToSql()}";

    /// <summary>
    /// Creates and returns an new <see cref="Joins{leftT}"/>  object that contains
    /// a newly created <see cref="Join{rightT}"/> object.
    /// </summary>
    /// <typeparam name="leftT">this is the class representing the table being joined onto.</typeparam>
    /// <param name="predicates">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <returns>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that contains
    /// a newly created <see cref="Join{rightT}"/> object.
    /// </returns>
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new(new Join<rightT>(predicate));

    /// <summary>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that 
    /// contains the current <see cref="Join{rightT}"/> object.
    /// </summary>
    /// <typeparam name="leftT">this is the class representing the table being joined onto.</typeparam>
    /// <returns>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that 
    /// contains the current <see cref="Join{rightT}"/> object.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);


    /// <summary>
    /// An enumeration of all Table Tags involved in the joins predicates.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// Converts the current <see cref="Join{rightT}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>A SQL string representing the <c>JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
