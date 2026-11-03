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
/// Represents an SQL <c>FULL JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// FullJoin<Order> join = new(predicate);
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
/// FULL JOIN "Order" 
/// ON ("Customer"."Id" = "Order"."CustomerId")
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// FULL JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class FullJoin<rightT> : FullJoin where rightT : class
{
    #region public constructors using reflection
    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// An optional <see cref="TableAliasTag"/> to use for the right-hand table in the join. If not provided, the no table alias will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public FullJoin(Predicates predicates) : this(predicates, (TableAliasTag?)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// An optional <see cref="TableAlias"/> to use for the right-hand table in the join. If not provided, the no table alias will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    [ExternalOnly]
    public FullJoin(Predicates predicates, string joinAsTableAlias) : this(predicates, TableAlias.New(joinAsTableAlias))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// An optional <see cref="TableAlias"/> to use for the right-hand table in the join. If not provided, the no table alias will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public FullJoin(Predicates predicates, TableAlias? joinAsTableAlias) : this(predicates, TableAliasTag.New(joinAsTableAlias))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// An optional <see cref="TableAliasTag"/> to use for the right-hand table in the join. If not provided, the no table alias will be used.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    internal FullJoin(Predicates predicates, TableAliasTag? joinAsTableAlias)
        : base(SqlToolsReflectorCache<rightT>.Table, SelectTagGenerator.GetAll<rightT>(), predicates, joinAsTableAlias)
    {
    }
    #endregion

    public static Joins Joins(Predicates predicates) =>
        new(new FullJoin<rightT>(predicates, (TableAliasTag?)null));

    public static Joins Joins(Predicates predicates, string joinAsTableAlias) =>
        new(new FullJoin<rightT>(predicates, TableAlias.New(joinAsTableAlias)));

    public static Joins Joins(Predicates predicates, TableAlias? joinAsTableAlias) =>
        new(new FullJoin<rightT>(predicates, joinAsTableAlias));
}
