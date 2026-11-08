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
/// Represents an SQL <c>CROSS JOIN</c> operation.
/// </summary>
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
public class CrossJoin : Join
{

    #region constructors from SubQuery
    [ExternalOnly]
    public CrossJoin(Subquery subQuery, string tableAlias)
        : this(subQuery, new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(tableAlias)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified subquery and table alias.
    /// Represents a Join with a default join type of Join
    /// </summary>
    /// <param name="subQuery">
    ///  An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a direct table reference. 
    ///  This allows for joining against complex subqueries while maintaining type safety and intellisense support for the right-hand side model.
    /// </param>
    /// <param name="tableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause. 
    /// This is required when joining on a subquery to provide a reference name for the derived table.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="tableAlias"/> is <c>null</c>.
    /// </exception>
    public CrossJoin(Subquery subQuery, TableAlias tableAlias)
        : base(JoinFragment.CrossJoin, new EmptyPredicate(), subQuery, tableAlias ?? throw new ArgumentNullException(nameof(tableAlias)))
    {
    }
    #endregion

    #region constructors from TableTag, SelectTag
    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified join type, select tags, table tag, and optional table alias.
    /// </summary>
    /// <param name="tableTag">
    /// The tag associated with the table being joined.
    /// </param>
    /// <param name="selectTags">
    /// The select tags for the joined table or subquery.
    /// </param>
    /// <param name="tableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause.
    /// </param>
    internal CrossJoin(TableTag tableTag, SelectTags selectTags, TableAlias? tableAlias)
        : base(JoinFragment.CrossJoin, tableTag, selectTags, new EmptyPredicate(), tableAlias)
    {

    }
    #endregion


    #region public static creation methods
    /// <summary>
    /// Creates and returns a new <see cref="JoinTypes.Joins"/> object that contains
    /// a newly created <see cref="Join"/> operation.
    /// </summary>
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
    /// Thrown when an alias argument is invalid.
    /// </exception>
    [ExternalOnly]
    public static Joins Joins(Subquery subQuery, string joinTableAlias) =>
        new(new CrossJoin(subQuery, new TableAlias(joinTableAlias)));

    /// <summary>
    /// Creates and returns a new <see cref="JoinTypes.Joins"/> object that contains
    /// a newly created <see cref="Join"/> operation.
    /// </summary>
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
    /// Thrown when an alias argument is invalid.
    /// </exception>
    public static Joins Joins(Subquery subQuery, TableAlias rightTableAlias) =>
        new(new CrossJoin(subQuery, rightTableAlias));
    #endregion
}
