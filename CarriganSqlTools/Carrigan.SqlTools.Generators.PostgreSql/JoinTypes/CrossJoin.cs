using Carrigan.Core.Exceptions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
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
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// CrossJoin<Order> join = new(predicate);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer" 
/// CROSS JOIN "Order" 
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// CROSS JOIN [Order]
/// ]]></code>
/// </example>
public class CrossJoin<rightT> : CrossJoin where rightT : class
{
    #region public constructors using reflection
    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    public CrossJoin() : this((TableAlias?)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="joinAsTableAlias">
    /// An optional <see cref="TableAlias"/> to use for the right-hand table in the join. If not provided, the no table alias will be used.
    /// </param>
    [ExternalOnly]
    public CrossJoin(string joinAsTableAlias) : this(TableAlias.New(joinAsTableAlias))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="joinAsTableAlias">
    /// An optional <see cref="TableAlias"/> to use for the right-hand table in the join. If not provided, the no table alias will be used.
    /// </param>
    public CrossJoin(TableAlias? joinAsTableAlias)
        : base(SqlToolsReflectorCache<rightT>.Table, SelectTagGenerator.GetAll<rightT>(), joinAsTableAlias)
    {
    }
    #endregion

    public static Joins Joins() =>
        new(new CrossJoin<rightT>((TableAlias?)null));

    public static Joins Joins(string joinAsTableAlias) =>
        new(new CrossJoin<rightT>(TableAlias.New(joinAsTableAlias)));

    public static Joins Joins(TableAlias? joinAsTableAlias) =>
        new(new CrossJoin<rightT>(joinAsTableAlias));
}
