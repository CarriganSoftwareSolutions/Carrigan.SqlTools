using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents one or more SQL <c>JOIN</c> operations applied to the table modeled by
/// <typeparamref name="leftT"/>.
/// </summary>
/// <typeparam name="leftT">
/// The data model representing the left (base) table onto which joins are applied.
/// </typeparam>
/// <remarks>
/// Join definitions are validated in the order provided. Each join’s predicate(s) may only reference:
/// <list type="bullet">
/// <item><description>Tables already introduced earlier in the join sequence, and/or</description></item>
/// <item><description>The table being introduced by the current join.</description></item>
/// </list>
/// If a join references a table that has not yet been introduced, an <see cref="InvalidTableException"/> is thrown.
/// </remarks>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates property names and throws an exception
/// if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join1 = new(predicate);
///
/// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// SqlQuery query = customerGenerator.Select(null, joins, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// INNER JOIN [Order] 
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{Customer, Order}"/> validates property names and throws an exception
/// if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// SelectTags selectTags =
///     SelectTags.Get<Customer>("Id", "CustomerId")
///         .Concat<Customer>(["Name", "Email", "Phone"])
///         .Append<Order>("Id", "OrderId")
///         .Concat<Order>(["OrderDate", "Total"])
///         .Append<PaymentMethod>("Id", "PaymentMethodId")
///         .Append<PaymentMethod>("ZipCode");
///
/// ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join1 = new(customerIdEquals);
///
/// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals =
///     new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
///
/// Joins<Customer> joins = new(join1, join2);
/// SqlQuery query = customerGenerator.Select(selectTags, joins, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT 
///     [Customer].[Id] AS CustomerId, 
///     [Customer].[Name], 
///     [Customer].[Email], 
///     [Customer].[Phone], 
///     [Order].[Id] AS OrderId, 
///     [Order].[OrderDate], 
///     [Order].[Total], 
///     [PaymentMethod].[Id] AS PaymentMethodId, 
///     [PaymentMethod].[ZipCode]
/// FROM [Customer]
/// INNER JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// INNER JOIN [PaymentMethod]
/// ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
/// ]]></code>
/// </example>
public class Joins<leftT> : JoinsBase
{
    /// <summary>
    /// The collection of join operations represented by this instance.
    /// </summary>
    protected override IEnumerable<JoinBase> Joints { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Joins{leftT}"/> class from one or more join definitions.
    /// </summary>
    /// <param name="joins">The join operations in the order they should appear in SQL.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="joins"/> is <c>null</c> or contains a <c>null</c> join entry.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when a join’s predicate references a table that has not been introduced earlier in the join sequence.
    /// </exception>
    public Joins(params IEnumerable<JoinBase> joins)
    {
        Joints = [];
        HashSet<TableTag?> invalids = [];
        HashSet<TableTag> validTables = [TableTag];

        //validate each join to ensure it is joined in the proper order for column participation.
        foreach (JoinBase join in joins)
        {
            validTables.Add(join.TableTag);
            //ensure each column involved in the join comes from either an earlier table or the table being joined on 
            IEnumerable<TableTag?>  newInvalids = join.JoinsOn.Where(table => validTables.DoesNotContain(table));
            if (newInvalids.IsNullOrEmpty())
                Joints = Joints.Append(join);
            else
            {
                foreach(TableTag? tableTag in newInvalids)
                {
                    if (invalids.DoesNotContain(tableTag))
                        invalids.Add(tableTag);
                }
            }
        }
        if(invalids.IsNotNullOrEmpty())
            throw new InvalidTableException(invalids);
    }

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="LeftJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">The data model representing the right-side table being joined.</typeparam>
    /// <param name="predicates">The predicate(s) that define the <c>ON</c> clause of the SQL <c>LEFT JOIN</c>.</param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="LeftJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> LeftJoin<rightT>(Predicates predicates) =>
        JoinTypes.LeftJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="RightJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">The data model representing the right-side table being joined.</typeparam>
    /// <param name="predicates">The predicate(s) that define the <c>ON</c> clause of the SQL <c>RIGHT JOIN</c>.</param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="RightJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> RightJoin<rightT>(Predicates predicates) =>
        JoinTypes.RightJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created generic <see cref="Join{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">The data model representing the right-side table being joined.</typeparam>
    /// <param name="predicates">The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.</param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="Join{rightT}"/>.
    /// </returns>
    public static Joins<leftT> Join<rightT>(Predicates predicates) =>
        JoinTypes.Join<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="InnerJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">The data model representing the right-side table being joined.</typeparam>
    /// <param name="predicates">The predicate(s) that define the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.</param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="InnerJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> InnerJoin<rightT>(Predicates predicates) =>
        JoinTypes.InnerJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="FullJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">The data model representing the right-side table being joined.</typeparam>
    /// <param name="predicates">The predicate(s) that define the <c>ON</c> clause of the SQL <c>FULL JOIN</c>.</param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="FullJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> FullJoin<rightT>(Predicates predicates) =>
        JoinTypes.FullJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="CrossJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">The data model representing the right-side table being joined.</typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="CrossJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> CrossJoin<rightT>() =>
        JoinTypes.CrossJoin<rightT>.Joins<leftT>();

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the left (base) table in the join sequence.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<leftT>.Table;
}
