using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

//TODO: REDO Documentation
namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>INNER JOIN</c> operation.
/// </summary>
/// <typeparam name="T">
/// The data model for the primary (left or base) table from which records are selected,
/// updated, or deleted.
/// </typeparam>
/// <typeparam name="J">
/// The data model for the secondary (right or joined) table that is joined to the primary table.
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
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown when a <see cref="ColumnTag"/>  referenced in a <c>JOIN</c> clause belongs to a table
    /// that is not included in the <c>JOIN</c>.
    /// </exception>
    public InnerJoin(Predicates predicates) : base(predicates) => 
        _sql = $"INNER JOIN {TableTag} ON {predicates.ToSql()}";

    //TODO: Documentation
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new(new InnerJoin<rightT>(predicate));

    //TODO: Documentation
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// Converts the current <see cref="InnerJoin{T,J}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>A SQL string representing the <c>INNER JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
