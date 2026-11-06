using Carrigan.Core.Exceptions;
using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;


namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents one or more SQL <c>JOIN</c> operations applied to the table modeled by
/// </summary>
/// <remarks>
/// Join definitions are validated in the order provided. Each join’s predicate(s) may only reference:
/// <list type="bullet">
/// <item><description>Tables already introduced earlier in the join sequence, and/or</description></item>
/// <item><description>The table being introduced by the current join.</description></item>
/// </list>
/// If a join references a table that has not yet been introduced, an <see cref="InvalidTableException"/> is thrown.
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join1 = new(predicate);
/// 
/// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
/// 
/// Joins joins = new(join1, join2);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = joins
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
/// INNER JOIN "PaymentMethod"
/// ON ("Order"."PaymentMethodId" = "PaymentMethod"."Id")
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// INNER JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// INNER JOIN [PaymentMethod]
/// ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
/// ]]></code>
/// </example>
/// <example>
/// <code language="csharp"><![CDATA[
/// SelectTags selectTags =
///     SelectTagGenerator.Get<Customer>("Id", "CustomerId")
///         .Concat<Customer>(["Name", "Email", "Phone"])
///         .Append<Order>("Id", "OrderId")
///         .Concat<Order>(["OrderDate", "Total"])
///         .Append<PaymentMethod>("Id", "PaymentMethodId")
///         .Append<PaymentMethod>("ZipCode");
/// 
/// ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join1 = new(customerIdEquals);
/// 
/// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
/// 
/// Joins joins = new(join1, join2);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = selectTags,
///     Joins = joins
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT 
///     "Customer"."Id" AS "CustomerId",
///     "Customer"."Name", 
///     "Customer"."Email",
///     "Customer"."Phone",
///     "Order"."Id" AS "OrderId", 
///     "Order"."OrderDate",
///     "Order"."Total", 
///     "PaymentMethod"."Id" AS "PaymentMethodId",
///     "PaymentMethod"."ZipCode" 
/// FROM "Customer" 
/// INNER JOIN "Order"
///    ON ("Customer"."Id" = "Order"."CustomerId") 
/// INNER JOIN "PaymentMethod" 
///    ON ("Order"."PaymentMethodId" = "PaymentMethod"."Id")
/// 
/// --SqlServer
/// SELECT
///     [Customer].[Id] AS [CustomerId], 
///     [Customer].[Name], 
///     [Customer].[Email], 
///     [Customer].[Phone], 
///     [Order].[Id] AS [OrderId], 
///     [Order].[OrderDate], 
///     [Order].[Total], 
///     [PaymentMethod].[Id] AS [PaymentMethodId], 
///     [PaymentMethod].[ZipCode] 
/// FROM [Customer]
/// INNER JOIN [Order]
///    ON ([Customer].[Id] = [Order].[CustomerId]) 
/// INNER JOIN [PaymentMethod] 
///    ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
/// ]]></code>
/// </example>

public class Joins : ISqlFragment, IEmpty
{
    /// <summary>
    /// The collection of join operations represented by this instance.
    /// </summary>
    protected IEnumerable<Join> Joints { get; set; }

    public static implicit operator Joins(Join join) => new(join);

    #region constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Joins"/> class from one or more join definitions.
    /// </summary>
    /// <param name="joins">The join operations in the order they should appear in SQL.</param>
    /// 
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="joins"/> is <c>null</c> or contains a <c>null</c> join entry.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when a join’s predicate references a table that has not been introduced earlier in the join sequence.
    /// </exception>
    public Joins(params IEnumerable<Join> joins)
    {
        ArgumentNullException.ThrowIfNull(joins);

        Joints = joins.Materialize(Core.Enums.NullOptionsEnum.ArgumentException);
    }
    #endregion



    public IEnumerable<Join> Validate(TableTag joinOntoRoot)
    {

        List<Join> joints = [];
        HashSet<TableTag> invalids = [];
        HashSet<TableTag> validTables = [joinOntoRoot];

        // Validate each join to ensure it is joined in the proper order for column participation.
        foreach (Join join in Joints)
        {
            validTables.Add(join.JoinAs);

            // Ensure each table involved in the join comes from either an earlier table or the table being joined.
            TableTag[] newInvalids =
            [
                .. join.JoinsOn.Where(tableExpression => validTables.DoesNotContain(tableExpression))
            ];

            if (newInvalids.Length == 0)
            {
                joints.Add(join);
            }
            else
            {
                invalids.UnionWith(newInvalids);
            }
        }

        if (invalids.IsNotNullOrEmpty())
        {
            throw new InvalidTableException([.. invalids]);
        }
        return joints;
    }

    #region static Creators

    public static Joins CrossJoin(Subquery subQuery, string joinTableAlias) =>
        JoinTypes.CrossJoin.Joins
        (
            subQuery,
            new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinTableAlias))
        );

    public static Joins CrossJoin(Subquery subQuery, TableAlias joinTableAlias) =>
        JoinTypes.CrossJoin.Joins(subQuery, joinTableAlias);

    public static Joins FullJoin(Predicates predicates, Subquery subQuery, string joinTableAlias) =>
        JoinTypes.FullJoin.Joins
        (
            predicates,
            subQuery,
            new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinTableAlias))
        );

    public static Joins FullJoin(Predicates predicates, Subquery subQuery, TableAlias joinTableAlias) =>
        JoinTypes.FullJoin.Joins(predicates, subQuery, joinTableAlias);

    public static Joins InnerJoin(Predicates predicates, Subquery subQuery, string joinTableAlias) =>
        JoinTypes.InnerJoin.Joins
        (
            predicates,
            subQuery,
            new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinTableAlias))
        );

    public static Joins InnerJoin(Predicates predicates, Subquery subQuery, TableAlias joinTableAlias) =>
        JoinTypes.InnerJoin.Joins(predicates, subQuery, joinTableAlias);

    public static Joins Join(Predicates predicates, Subquery subQuery, string joinTableAlias) =>
        JoinTypes.Join.Joins
        (
            predicates,
            subQuery,
            new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinTableAlias))
        );

    public static Joins Join(Predicates predicates, Subquery subQuery, TableAlias joinTableAlias) =>
        JoinTypes.Join.Joins(predicates, subQuery, joinTableAlias);

    public static Joins LeftJoin(Predicates predicates, Subquery subQuery, string joinTableAlias) =>
        JoinTypes.LeftJoin.Joins
        (
            predicates,
            subQuery,
            new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinTableAlias))
        );

    public static Joins LeftJoin(Predicates predicates, Subquery subQuery, TableAlias joinTableAlias) =>
        JoinTypes.LeftJoin.Joins(predicates, subQuery, joinTableAlias);

    public static Joins RightJoin(Predicates predicates, Subquery subQuery, string joinTableAlias) =>
        JoinTypes.RightJoin.Joins
        (
            predicates,
            subQuery,
            new TableAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(joinTableAlias))
        );

    public static Joins RightJoin(Predicates predicates, Subquery subQuery, TableAlias joinTableAlias) =>
        JoinTypes.RightJoin.Joins(predicates, subQuery, joinTableAlias);

    #endregion


    #region IEnumerable Style methods
    /// <summary>
    /// Gets the first join in the current join sequence.
    /// </summary>
    /// <returns>The first join in the sequence.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="Joints"/> is <see langword="null"/> or contains a <see langword="null"/> entry.
    /// </exception>
    internal Join First() =>
        Joints.First();

    /// <summary>
    /// Determines whether this <see cref="Joins"/> instance contains any join definitions.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if no joins are defined; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joints"/> or contains <c>null</c> join entries.
    /// </exception>
    public bool IsEmpty() =>
        Joints.None();

    /// <summary>
    /// Determines whether this <see cref="Joins"/> instance contains any join definitions.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if one or more joins are defined; otherwise, <see langword="false"/>.           
    /// </returns>
    public bool IsNotEmpty() =>
        IsEmpty() is false;

    /// <summary>
    /// Creates and returns a new <see cref="Joins"/> instance that contains all joins from the current instance
    /// plus the additional join provided.
    /// </summary>
    /// <param name="newJoin">The join operation to append.</param>
    /// <returns>A new <see cref="Joins"/> instance containing the additional join.</returns>
    public Joins Append(Join newJoin) =>
        new(Joints.Append(newJoin));

    /// <summary>
    /// Creates and returns a new <see cref="Joins"/> instance that contains all joins from the current instance
    /// plus the additional joins provided.
    /// </summary>
    /// <param name="newJoins">The additional join operations to include.</param>
    /// <returns>A new <see cref="Joins"/> instance containing the additional joins.</returns>
    public Joins Concat(IEnumerable<Join> newJoins) =>
        new(Joints.Concat(newJoins));

    /// <summary>
    /// Creates and returns a new <see cref="Joins"/> instance that contains all joins from the current instance
    /// plus the additional joins provided.
    /// </summary>
    /// <param name="newJoins">The additional join operations to include.</param>
    /// <returns>A new <see cref="Joins"/> instance containing the additional joins.</returns>
    public Joins Concat(Joins newJoins) =>
        new(Joints.Concat(newJoins.Joints));

    #endregion

    #region ISqlFragment Implementation
    /// <summary>
    /// Generates the SQL fragments for all <c>JOIN</c> clauses
    /// represented by this <see cref="Joins"/> instance.
    /// </summary>
    /// <returns>
    /// The SQL fragments that render each <c>JOIN</c> clause in sequence.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived type returns <c>null</c> for <see cref="Joints"/> or contains <c>null</c> join entries.
    /// </exception>
    /// <remarks>
    /// Any exception thrown by an individual join while rendering SQL will be propagated to the caller.
    /// </remarks>
    public IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
        Joints.SelectMany(join => join.ToSqlFragments(dialect));

    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        ToSqlFragments(dialect).SelectMany(sqlFragment => sqlFragment.GetSqlFragmentParameters(dialect));

    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        ToSqlFragments(dialect);

    public string ToSql(ISqlDialects dialect) =>
        Flatten(dialect).ToSql(dialect);
    #endregion
}