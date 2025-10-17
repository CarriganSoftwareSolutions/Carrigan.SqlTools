using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: joins

namespace Carrigan.SqlTools.JoinTypes;

//TODO: REDO Documentation
/// <summary>
/// Defines a class that represent one or more SQL join operations.
/// </summary>
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
    /// Represents a collection of classes where each class defines a single SQL join operation.
    /// The name differs from the preferred “Joins” to avoid a naming conflict (e.g., Joins.Joins),
    /// which would result in a compiler error.
    /// </summary>
    protected override IEnumerable<JoinBase> Joints { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Joins"/> class.
    /// </summary>
    /// <param name="joins">
    /// One or more sequences of <see cref="Joins"/> objects, where each object represents a single SQL join operation.
    /// </param>
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

    public static Joins<leftT> LeftJoin<rightT>(Predicates predicates) =>
        JoinTypes.LeftJoin<rightT>.Joins<leftT>(predicates);

    public static Joins<leftT> Join<rightT>(Predicates predicates) =>
        JoinTypes.Join<rightT>.Joins<leftT>(predicates);

    public static Joins<leftT> InnerJoin<rightT>(Predicates predicates) =>
        JoinTypes.InnerJoin<rightT>.Joins<leftT>(predicates);

    internal override TableTag TableTag =>
        SqlToolsReflectorCache<leftT>.Table;
}
