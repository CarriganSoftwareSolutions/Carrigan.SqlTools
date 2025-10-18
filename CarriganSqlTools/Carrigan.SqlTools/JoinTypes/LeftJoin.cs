using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;


//TODO: proof read Documentation
/// <summary>
/// Represents an SQL <c>LFT JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model for the table being left joined on.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// JoinsBase join = Joins<Customer>.LeftJoin<Order>(predicate);
/// 
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// LEFT JOIN [Order] 
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class LeftJoin<rightT> : JoinBase
{
    private readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeftJoin{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>LEFT JOIN</c>.
    /// </param>

    public LeftJoin(Predicates predicate) : base(predicate) => 
        _sql = $"LEFT JOIN {TableTag} ON {predicate.ToSql()}";

    /// <summary>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that contains
    /// a newly created <see cref="LeftJoin{rightT}"/> object.
    /// </summary>
    /// <typeparam name="leftT">this is the class representing the table being joined onto.</typeparam>
    /// <param name="predicates">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>LEFT JOIN</c>.
    /// </param>
    /// <returns>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that contains
    /// a newly created <see cref="LeftJoin{rightT}"/> object.
    /// </returns>
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new (new LeftJoin<rightT>(predicate)) ;

    /// <summary>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that 
    /// contains the current <see cref="LeftJoin{rightT}"/> object.
    /// </summary>
    /// <typeparam name="leftT">this is the class representing the table being joined onto.</typeparam>
    /// <returns>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that 
    /// contains the current <see cref="LeftJoin{rightT}"/> object.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() =>
        new (this);

    /// <summary>
    /// An enumeration of all Table Tags involved in the joins predicates.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// Converts the current <see cref="InnerJoin{rightT}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>A SQL string representing the <c>LEFT JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
