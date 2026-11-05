using Carrigan.Core.Exceptions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents a general SQL <c>JOIN</c> operation.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// Join<Order> join = new(predicate);
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
/// --PostgreSQL
/// SELECT "Customer".* 
/// FROM "Customer" 
/// JOIN "Order" 
/// ON ("Customer"."Id" = "Order"."CustomerId")
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class Join : ITableExpression
{
    #region properties

    /// <summary>
    /// Gets an enumeration of all <see cref="Tags.SelectTags"/> objects that represent the  joined table.
    /// </summary>
    public SelectTags SelectTags { get; }

    /// <summary>
    /// Optional readonly subquery for the right-hand side.
    /// </summary>
    /// <remarks>Null when no subquery is provided. Assigned at construction and immutable
    /// thereafter.</remarks>
    private readonly Subquery? Subquery;

    /// <summary>
    /// Gets the <see cref="TableTagBase"/> associated with the right-hand (joined) table
    /// </summary>
    public TableTagBase JoinAs =>
        (TableTagBase?) TableAliasTag ?? TableTag ?? throw new InvalidOperationException("This join lacks a table reference.");

    /// <summary>
    /// Gets the <see cref="TableTagBase"/> associated with the right-hand (joined) table
    /// in the SQL <c>JOIN</c> clause.
    /// </summary>
    public TableAliasTag? TableAliasTag { get; }

    /// <summary>
    /// Optional readonly table tag for the right-hand side of the join operation.
    /// </summary>
    public readonly TableTag? TableTag;

    /// <summary>
    /// Optional readonly join type fragment for the join operation.
    /// </summary>
    private readonly JoinFragment JoinType;

    /// <summary>
    /// Gets the predicate used to relate the joined table to the previous table source.
    /// </summary>
    private readonly Predicates Predicates;


    /// <summary>
    /// Gets an enumeration of all <see cref="TableTag"/> objects involved in the
    /// current join's predicate logic.
    /// </summary>
    /// <remarks>
    /// Each <see cref="TableTag"/> represents a table referenced within the join's <c>ON</c> clause.
    /// When this join type does not use predicates, this returns an empty sequence.
    /// </remarks>
    internal IEnumerable<TableTagBase> JoinsOn =>
        Predicates
            .AllParticipatingTables
            .Distinct();

    /// <summary>Wraps this join in a non-generic join sequence.</summary>
    public Joins AsJoins() => new(this);
    #endregion

    #region constructors from subquery
    [ExternalOnly]
    public Join(Predicates predicates, Subquery subQuery, string joinAsTableAlias)
        : this(predicates, subQuery, new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinAsTableAlias)))
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
    /// <param name="joinAsTableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause. 
    /// This is required when joining on a subquery to provide a reference name for the derived table.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> or <paramref name="joinAsTableAlias"/> is <c>null</c>.
    /// </exception>
    public Join(Predicates predicates, Subquery subQuery, TableAlias joinAsTableAlias)
        : this(JoinFragment.Join, predicates, subQuery, new TableAliasTag (joinAsTableAlias ?? throw new ArgumentNullException(nameof(joinAsTableAlias))))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified join type, predicates, subquery, and table alias.
    /// Used to define joins with specific Join types Ex Left Join, Right Join, etc.
    /// </summary>
    /// <param name="joinType">
    /// The type of join operation to perform (e.g., INNER JOIN, LEFT JOIN, etc.).
    /// </param>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a direct table reference.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="joinType"/>, <paramref name="predicates"/>, or <paramref name="joinAsTableAlias"/> is <c>null</c>.
    /// </exception>
    internal Join(JoinFragment joinType, Predicates predicates, Subquery subQuery, TableAlias joinAsTableAlias)
        : this(joinType, predicates, subQuery, new TableAliasTag(joinAsTableAlias ?? throw new ArgumentNullException(nameof(joinAsTableAlias))))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified join type, predicates, subquery, and table alias.
    /// Used to define joins with specific Join types Ex Left Join, Right Join, etc.
    /// </summary>
    /// <param name="joinType">
    /// The type of join operation to perform (e.g., INNER JOIN, LEFT JOIN, etc.).
    /// </param>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a direct table reference.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause.
    /// </param>
    internal Join(JoinFragment joinType, Predicates predicates, Subquery subQuery, TableAliasTag? joinAsTableAlias)
    {
        ArgumentNullException.ThrowIfNull(joinType, nameof(joinType));
        if (joinType.HasPredicates)
            ArgumentNullException.ThrowIfNull(predicates, nameof(predicates));
        Subquery = subQuery;
        JoinType = joinType;
        Predicates = predicates;
        TableAliasTag = joinAsTableAlias;
        SelectTags = Subquery.SelectTags.FromTableAlias(joinAsTableAlias?.TableAlias);
    }

    #endregion

    #region internal table tag / select tag constructors using Join Fragments for Join Type
    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified join type, select tags, predicates, table tag, and optional table alias.
    /// </summary>
    /// <param name="joinType">
    /// The type of join operation to perform (e.g., INNER JOIN, LEFT JOIN, etc.).
    /// </param>
    /// <param name="tableTag">
    /// The tag associated with the table being joined.
    /// </param>
    /// <param name="selectTags">
    /// The select tags for the joined table or subquery.
    /// </param>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <param name="joinASTableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause.
    /// </param>
    internal Join(JoinFragment joinType, TableTag tableTag, SelectTags selectTags, Predicates predicates, TableAlias? joinASTableAlias)
        : this(joinType, tableTag, selectTags, predicates, TableAliasTag.New(joinASTableAlias))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Join"/> class with the specified join type, select tags, predicates, table tag, and optional table alias.
    /// </summary>
    /// <param name="joinType">
    /// The type of join operation to perform (e.g., INNER JOIN, LEFT JOIN, etc.).
    /// </param>
    /// <param name="tableTag">
    /// The tag associated with the table being joined.
    /// </param>
    /// <param name="selectTags">
    /// The select tags for the joined table or subquery.
    /// </param>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <param name="joinAsTableAlias">
    /// The alias to use for the joined table or subquery in the SQL <c>JOIN</c> clause.
    /// </param>
    internal Join(JoinFragment joinType, TableTag tableTag, SelectTags selectTags, Predicates predicates, TableAliasTag? joinAsTableAlias)
    {
        ArgumentNullException.ThrowIfNull(joinType, nameof(joinType));
        ArgumentNullException.ThrowIfNull(selectTags, nameof(selectTags));
        if (joinType.HasPredicates)
            ArgumentNullException.ThrowIfNull(predicates, nameof(predicates));
        ArgumentNullException.ThrowIfNull(tableTag, nameof(tableTag));
        TableTag = tableTag;
        JoinType = joinType;
        Predicates = predicates;
        TableAliasTag = joinAsTableAlias;
        SelectTags = selectTags;
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
    /// <param name="joinAsTableAlias">
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
    public static Joins Joins(Predicates predicates, Subquery subQuery, string joinAsTableAlias) =>
        new(new Join(predicates, subQuery, new TableAlias(joinAsTableAlias)));

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
    /// <param name="joinAsTableAlias">
    /// The table alias representing the right table being joined onto.
    /// </param>
    /// 
    /// <returns>
    /// A new <see cref="JoinTypes.Joins"/> object containing a single <see cref="Join"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    public static Joins Joins(Predicates predicates, Subquery subQuery, TableAlias joinAsTableAlias) =>
        new(new Join(predicates, subQuery, joinAsTableAlias));
    #endregion

    #region internal static creation methods
    /// <summary>
    /// Creates and returns a new <see cref="JoinTypes.Joins"/> object that contains
    /// a newly created <see cref="Join"/> operation.
    /// </summary>
    /// <param name="joinType"></param>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <param name="subQuery">
    /// An optional <see cref="Subquery"/> to use as the right-hand side of the join instead of a
    /// direct table reference. This allows for joining against complex subqueries while maintaining type
    /// safety and intellisense support for the right-hand side model.
    /// </param>
    /// <param name="joinAsTableAliasTag">
    /// The table alias representing the right table being joined onto.
    /// </param>
    /// 
    /// <returns>
    /// A new <see cref="JoinTypes.Joins"/> object containing a single <see cref="Join"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    internal static Joins Joins(JoinFragment joinType, Predicates predicates, Subquery subQuery, TableAliasTag joinAsTableAliasTag) =>
        new(new Join(joinType, predicates, subQuery, joinAsTableAliasTag));
    #endregion

    #region ISqlFragment interface methods
    /// <summary>
    /// Converts the current <see cref="Join"/> instance to its <see cref="ISqlFragment"/> representation.
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns>
    /// A <see cref="ISqlFragment"/> string representing the <c>JOIN</c> clause.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the join predicates render to an empty SQL expression, because <c>JOIN</c> requires an <c>ON</c> clause.
    /// </exception>
    /// <remarks>
    /// Any exception thrown while rendering the predicate tree or while resolving <see cref="TableTag"/> will be propagated to the caller.
    /// </remarks>
    public virtual IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    { 
        if (JoinType.HasPredicates && (Predicates == null || Predicates is EmptyPredicate))
            throw new InvalidOperationException("JOIN requires at least one predicate for the ON clause.");

        yield return ISqlFragment.Space;
        yield return JoinType;
        yield return ISqlFragment.Space;
        if (Subquery is not null)
        {
            yield return Subquery;
            yield return new SqlFragmentText(" AS ");
            yield return TableAliasTag ?? throw new NullReferenceException("Joining on a SubQuery requires a Table Alias");
        }
        else if (TableTag is not null)
        {
            yield return TableTag;
            if(TableAliasTag is not null)
            {
                yield return new SqlFragmentText(" AS ");
                yield return TableAliasTag;
            }
        }
        else
            throw new NullReferenceException();
        if (JoinType.HasPredicates)
        {
            yield return new SqlFragmentText(" ON ");
            foreach (ISqlFragment fragment in Predicates.ToSqlFragments(dialect))
                yield return fragment;
        }
    }

    /// <summary>
    /// Converts the current <see cref="Join"/> instance to its <see cref="ISqlFragment"/> representation.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use when rendering the SQL fragments.
    /// </param>
    /// <returns>
    /// A collection of <see cref="SqlFragmentParameter"/> objects representing the parameters used in the SQL fragments.
    /// </returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        ToSqlFragments(dialect).SelectMany(sqlFragment => sqlFragment.GetSqlFragmentParameters(dialect));

    /// <summary>
    /// Flattens the current <see cref="Join"/> instance into a single SQL string representation.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use when rendering the SQL fragments.
    /// </param>
    /// <returns>
    /// A string representing the flattened SQL for the current <see cref="Join"/> instance.
    /// </returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        ToSqlFragments(dialect);

    /// <summary>
    /// Flattens the current <see cref="Join"/> instance into a single SQL string representation.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use when rendering the SQL fragments.
    /// </param>
    /// <returns>
    /// A string representing the flattened SQL for the current <see cref="Join"/> instance.
    /// </returns>
    public string ToSql(ISqlDialects dialect) =>
        Flatten(dialect).ToSql(dialect);
    #endregion
}
