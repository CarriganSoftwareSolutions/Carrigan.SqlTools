using Carrigan.Core.Exceptions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
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
public class InnerJoin : Join
{

    #region constructors from SubQuery
    [ExternalOnly]
    public InnerJoin(Predicates predicates, Subquery subQuery, string tableAlias)
        : this(predicates, subQuery, new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(tableAlias)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified predicates, subquery, and table alias.
    /// Represents a Join with a default join type of Join
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <param name="subQuery">
    ///  An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a direct table reference. 
    ///  This allows for joining against complex subqueries while maintaining type safety and intellisense support for the right-hand side model.
    /// </param>
    /// <param name="tableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause. 
    /// This is required when joining on a subquery to provide a reference name for the derived table.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> or <paramref name="tableAlias"/> is <c>null</c>.
    /// </exception>
    public InnerJoin(Predicates predicates, Subquery subQuery, TableAlias tableAlias)
        : base(JoinFragment.InnerJoin, predicates, subQuery, tableAlias ?? throw new ArgumentNullException(nameof(tableAlias)))
    {
    }
    #endregion

    #region constructors from TableTag, SelectTag
    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified join type, select tags, predicates, table tag, and optional table alias.
    /// </summary>
    /// <param name="tableTag">
    /// The tag associated with the table being joined.
    /// </param>
    /// <param name="selectTags">
    /// The select tags for the joined table or subquery.
    /// </param>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <param name="tableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause.
    /// </param>
    internal InnerJoin(TableTag tableTag, SelectTags selectTags, Predicates predicates, TableAlias? tableAlias)
        : base(JoinFragment.InnerJoin, tableTag, selectTags, predicates, tableAlias)
    {

    }
    #endregion


    #region public static creation methods
    /// <summary>
    /// Creates and returns a new <see cref="JoinTypes.Joins"/> object that contains
    /// a newly created <see cref="Join"/> operation.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <param name="joinTableAlias">
    /// The table alias representing the right table being joined onto.
    /// </param>
    /// 
    /// <returns>
    /// A new <see cref="JoinTypes.Joins"/> object containing a single <see cref="Join"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    [ExternalOnly]
    public static new Joins Joins(Predicates predicates, Subquery subQuery, string joinTableAlias) =>
        new(new InnerJoin(predicates, subQuery, new TableAlias(joinTableAlias)));

    /// <summary>
    /// Creates and returns a new <see cref="JoinTypes.Joins"/> object that contains
    /// a newly created <see cref="Join"/> operation.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <param name="rightTableAlias">
    /// The table alias representing the right table being joined onto.
    /// </param>
    /// 
    /// <returns>
    /// A new <see cref="JoinTypes.Joins"/> object containing a single <see cref="Join"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public static new Joins Joins(Predicates predicates, Subquery subQuery, TableAlias rightTableAlias) =>
        new(new InnerJoin(predicates, subQuery, rightTableAlias));
    #endregion
}
