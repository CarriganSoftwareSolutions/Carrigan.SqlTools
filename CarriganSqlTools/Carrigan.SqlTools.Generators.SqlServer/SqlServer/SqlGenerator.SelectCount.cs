using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlServer;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// Builds an <see cref="SqlQuery"/> containing a parameterized
    /// <c>SELECT COUNT(...)</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>SELECT</c> projection, <c>JOIN</c>, and <c>WHERE</c> clauses.
    /// </summary>
    /// <param name="distinct"></param>
    /// <param name="select">
    /// Optional result projection to be counted. If omitted or empty, the generator
    /// counts <c>{Table}.*</c>.
    /// </param>
    /// <param name="joins">
    /// Optional joins to include in the count query. Omit to count only rows from the base table.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated count SQL and whose
    /// <c>Parameters</c> are derived from <paramref name="predicates"/> and any joins.
    /// </returns>
    /// <remarks>
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown when <paramref name="select"/> defines duplicate or ambiguous result column names.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when any table referenced by <paramref name="select"/> or <paramref name="predicates"/> (or by their columns)
    /// is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = orderGenerator.SelectCount(null, null, null, null);
    /// ]]></code>
    /// <para><see cref="Column{T}"/> validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code><![CDATA[
    /// SELECT COUNT([Order].[Id]) FROM [Order]
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>
    /// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Column<Order> totalCol = new(nameof(Order.Total));
    /// Parameter minTotal = new("Total", 500m);
    /// GreaterThan greaterThan = new(totalCol, minTotal);
    /// 
    /// SqlQuery query = orderGenerator.SelectCount(null, null, null, greaterThan);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT([Order].[Id]) 
    /// FROM [Order] 
    /// WHERE ([Order].[Total] > @Parameter_Total)
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>
    /// <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if a property isn't valid
    /// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Column<Order> totalCol = new(nameof(Order.Total));
    /// Parameter minTotal = new("Total", 500m);
    /// GreaterThan greaterThan = new(totalCol, minTotal);
    /// 
    /// ColumnEqualsColumn<Order, Customer> columnCompare = new(nameof(Order.CustomerId), nameof(Customer.Id));
    /// Join<Customer> join = new(columnCompare);
    /// 
    /// SqlQuery query = orderGenerator.SelectCount(null, null, join, greaterThan);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT([Order].[Id]) 
    /// FROM [Order] 
    /// JOIN [Customer] 
    /// ON ([Order].[CustomerId] = [Customer].[Id]) 
    /// WHERE ([Order].[Total] > @Parameter_Total)
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>
    /// <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if a property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// SelectTag selectTag = SelectTag.Get<Customer>("Id", "CustomerId");
    /// 
    /// ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Order> join1 = new(customerIdEquals);
    /// 
    /// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
    /// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
    /// 
    /// Joins<Customer> joins = new(join1, join2);
    /// 
    /// SqlQuery query = customerGenerator.SelectCount(true, selectTag, joins, null); 
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT(DISTINCT Customer].[Id])
    /// FROM [Customer] 
    /// INNER JOIN [Order] 
    /// ON ([Customer].[Id] = [Order].[CustomerId]) 
    /// INNER JOIN [PaymentMethod] 
    /// ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
    /// ]]></code>
    /// </example>
    /// <param name="predicates">
    /// Optional filter predicates to compose the <c>WHERE</c> clause for the count.
    /// </param>
    public SqlQuery SelectCount(bool? distinct, SelectTag? select, Joins<T>? joins, Predicates? predicates) =>
        base.BaseSelectCount(distinct, select, joins, predicates);
}
