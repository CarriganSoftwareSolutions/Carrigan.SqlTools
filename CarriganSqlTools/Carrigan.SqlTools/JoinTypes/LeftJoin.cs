using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

//TODO: REDO Documentation,  Examples
/// <summary>
/// Represents an SQL <c>LEFT JOIN</c>.
/// </summary>
/// <typeparam name="rightT">
/// The data model for the secondary (right or joined) table that is joined to the primary table.
/// </typeparam>
/// <example>
/// <para>
/// Note: <c>ColumnEqualsColumn&lt;lefT, rightT&gt;</c> validates property names and throws an exception if a property name is invalid.
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
    /// <param name="predicate">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>LEFT JOIN</c>.
    /// </param>
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown when a <see cref="ColumnTag"/>  referenced in a <c>JOIN</c> clause belongs to a table
    /// that is not included in the <c>JOIN</c>.
    /// </exception>

    public LeftJoin(Predicates predicate) : base(predicate) => 
        _sql = $"LEFT JOIN {TableTag} ON {predicate.ToSql()}";

    //TODO: Documentation, Examples
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new (new LeftJoin<rightT>(predicate)) ;

    //TODO: Documentation, Examples
    public Joins<leftT> AsJoins<leftT>() =>
        new (this);

    /// <summary>
    /// Enumerates all possible columns included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given column
    /// participates in a table that participates in any join operation.
    /// </summary>
    //public override IEnumerable<ColumnInfo> ColumnInfo =>
    //    SqlToolsReflectorCache<T>.ColumnInfo.Concat(SqlToolsReflectorCache<rightT>.ColumnInfo);

    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Generates the SQL representation of the <c>LEFT JOIN</c> clause.
    /// </summary>
    /// <returns>A SQL string representing the <c>LEFT JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
