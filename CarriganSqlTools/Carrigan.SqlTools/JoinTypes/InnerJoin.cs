using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

//TODO: proof read Documentation
/// <summary>
/// Represents an SQL <c>INNER JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model for the table being inner joined on.
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
    /// The condition that defines the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.
    /// </param>
    public InnerJoin(Predicates predicates) : base(predicates) => 
        _sql = $"INNER JOIN {TableTag} ON {predicates.ToSql()}";

    /// <summary>
    /// Creates and returns an new <see cref="Joins{leftT}"/>  object that contains
    /// a newly created <see cref="InnerJoin{rightT}"/> object.
    /// </summary>
    /// <typeparam name="leftT">this is the class representing the table being joined onto.</typeparam>
    /// <param name="predicates">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.
    /// </param>
    /// <returns>
    /// Creates and returns an new <see cref="JoinTypes.Joins{leftT}"/>  object that contains
    /// a newly created <see cref="InnerJoin{rightT}"/> object.
    /// </returns>
    public static Joins<leftT> Joins<leftT>(Predicates predicates) =>
        new(new InnerJoin<rightT>(predicates));

    /// <summary>
    /// Creates and returns an new <see cref="Joins{leftT}"/>  object that 
    /// contains the current <see cref="InnerJoin{rightT}"/> object.
    /// </summary>
    /// <typeparam name="leftT">this is the class representing the table being joined onto.</typeparam>
    /// <returns>
    /// Creates and returns an new <see cref="Joins{leftT}"/>  object that 
    /// contains the current <see cref="InnerJoin{rightT}"/> object.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    /// <summary>
    /// An enumeration of all Table Tags involved in the joins predicates.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// Converts the current <see cref="InnerJoin{rightT}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>A SQL string representing the <c>INNER JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
