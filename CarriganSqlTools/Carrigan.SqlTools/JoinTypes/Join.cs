namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>JOIN</c> operation. This class functions as an alias
/// for the <see cref="LeftJoin{T,J}"/> class.
/// </summary>
/// <typeparam name="T">
/// The data model for the primary (left or base) table from which records are
/// selected, updated, or deleted.
/// </typeparam>
/// <typeparam name="J">
/// The data model for the secondary (right or joined) table that is joined to
/// the primary table.
/// </typeparam>
/// <example>
/// <para>
/// Note: <c>ColumnEqualsColumn&lt;lefT, rightT&gt;</c> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn&lt;Customer, Order&gt; predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// Join&lt;Customer, Order&gt; join = new(predicate);
/// Join<Customer, Order> join = new(equals);
///
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// LEFT JOIN [Order]  ON 
/// ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class Join<T, J> : LeftJoin<T, J>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class.
    /// </summary>
    /// <param name="predicate">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    public Join(Predicates.Predicates predicate) : base(predicate)
    { }
}
