using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: joins

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Defines a class that represents one or more SQL <c>JOIN</c> operations.
/// </summary>
/// <typeparam name="leftT">
/// The data model representing the left (base) table onto which joins are applied.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join1 = new(predicate);
/// 
/// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
/// 
/// Joins<Customer> joins = new(join1, join2);
/// 
/// SqlQuery query = customerGenerator.Select(null, joins, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// INNER JOIN [Order] 
/// ON ([Customer].[Id] = [Order].[CustomerId]) 
/// INNER JOIN [PaymentMethod] 
/// ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Note: <c><ColumnEqualsColumn<Customer, Order>/c> validates the names of the properties, and throws an error if the property isn't valid
/// </para>
/// /// <code language="csharp"><![CDATA[
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
/// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
/// 
/// Joins<Customer> joins = new(join1, join2);
/// 
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
    /// A collection of <see cref="JoinBase"/> instances, where each instance represents
    /// a single SQL <c>JOIN</c> operation.
    /// </summary>
    /// <remarks>
    /// This property is named <c>Joints</c> to avoid a naming conflict with the enclosing class name.
    /// </remarks>
    protected override IEnumerable<JoinBase> Joints { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Joins{leftT}"/> class.
    /// </summary>
    /// <param name="joins">
    /// One or more <see cref="JoinBase"/> instances, where each instance defines a single SQL <c>JOIN</c> operation.
    /// </param>
    /// <exception cref="InvalidTableException">
    /// Thrown if any join references a table that is not part of the valid join sequence.
    /// </exception>
    public Joins(params IEnumerable<JoinBase> joins)
    {
        Joints = [];
        IEnumerable<TableTag> invalids; ;

        //validate each join to ensure it is joined in the proper order for column participation.
        foreach (JoinBase join in joins)
        {
            IEnumerable<TableTag> valid = TableTags.Append(join.TableTag);
            //ensure each column involved in the join comes from either an earlier table or the table being joined on 
            invalids = join.JoinsOn.Where(table => valid.DoesNotContain(table));
            if (invalids.IsNullOrEmpty())
                Joints = Joints.Append(join);
            else
                throw new InvalidTableException(invalids);
        }
    }

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="LeftJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">
    /// The data model representing the right-side table being joined.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>LEFT JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="LeftJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> LeftJoin<rightT>(Predicates predicates) =>
        JoinTypes.LeftJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="RightJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">
    /// The data model representing the right-side table being joined.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>RIGHT JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="RightJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> RightJoin<rightT>(Predicates predicates) =>
        JoinTypes.RightJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created generic <see cref="Join{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">
    /// The data model representing the right-side table being joined.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="Join{rightT}"/>.
    /// </returns><see cref="Join{rightT}"/> object.
    /// </returns>
    public static Joins<leftT> Join<rightT>(Predicates predicates) =>
        JoinTypes.Join<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="InnerJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">
    /// The data model representing the right-side table being joined.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="InnerJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> InnerJoin<rightT>(Predicates predicates) =>
        JoinTypes.InnerJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="FullJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">
    /// The data model representing the right-side table being joined.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>Full JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="FullJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> FullJoin<rightT>(Predicates predicates) =>
        JoinTypes.FullJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> instance that contains
    /// a newly created <see cref="CrossJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="rightT">
    /// The data model representing the right-side table being joined.
    /// </typeparam>
    /// <param name="predicates">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>Cross JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> containing a single <see cref="CrossJoin{rightT}"/>.
    /// </returns>
    public static Joins<leftT> CrossJoin<rightT>(Predicates predicates) =>
        JoinTypes.CrossJoin<rightT>.Joins<leftT>(predicates);

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the left (base) table in the join sequence.
    /// </summary>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<leftT>.Table;
}
