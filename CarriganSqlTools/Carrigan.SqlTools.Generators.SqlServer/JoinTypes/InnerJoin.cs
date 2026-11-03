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
/// Represents an SQL <c>INNER JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
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
/// INNER JOIN "Order" 
/// ON ("Customer"."Id" = "Order"."CustomerId")
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// INNER JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class InnerJoin<rightT> : InnerJoin where rightT : class
{
    #region public constructors using reflection
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
    public InnerJoin(Predicates predicates, string? joinAsTableAlias) : this(predicates, IdentifierTypes.TableAlias.New(joinAsTableAlias))
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
    public InnerJoin(Predicates predicates, TableAlias? joinAsTableAlias = null) : this(predicates, TableAliasTag.New(joinAsTableAlias))
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
    internal InnerJoin(Predicates predicates, TableAliasTag? joinAsTableAlias = null)
        : base(SqlToolsReflectorCache<rightT>.Table, SelectTagGenerator.GetAll<rightT>(), predicates, joinAsTableAlias)
    {
    }
    #endregion

    public static Joins Joins(Predicates predicates, string? joinAsTableAlias) =>
        new (new InnerJoin<rightT>(predicates, TableAlias.New(joinAsTableAlias)));

    public static Joins Joins(Predicates predicates, TableAlias? joinAsTableAlias = null) =>
        new(new InnerJoin<rightT>(predicates, joinAsTableAlias));
}
