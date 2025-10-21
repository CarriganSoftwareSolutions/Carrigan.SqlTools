using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Builds an <see cref="SqlQuery"/> containing a parameterized
    /// <c>SELECT COUNT(...)</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>SELECT</c> projection, <c>JOIN</c>, and <c>WHERE</c> clauses.
    /// </summary>
    /// <param name="selects">
    /// Optional result projection to be counted. If omitted or empty, the generator
    /// counts <c>{Table}.*</c>.
    /// </param>
    /// <param name="joins">
    /// Optional joins to include in the count query. Omit to count only rows from the base table.
    /// </param>
    /// <param name="predicates">
    /// Optional filter predicates to compose the <c>WHERE</c> clause for the count.
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
    /// Thrown when <paramref name="selects"/> defines duplicate or ambiguous result column names.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when any table referenced by <paramref name="predicates"/> (or by their columns)
    /// is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    /// <param name="orderBy"></param>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = orderGenerator.SelectCount(null, null, null);
    /// ]]></code>
    /// <para><see cref="Column{T}"/> validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code><![CDATA[
    /// SELECT COUNT([Order].*) FROM [Order]
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
    /// SqlQuery query = orderGenerator.SelectCount(null, null, greaterThan);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT([Order].*) 
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
    /// Joins<Order> joins = Joins<Order>.Join<Customer>(columnCompare);
    /// 
    /// SqlQuery query = orderGenerator.SelectCount(null, joins, greaterThan);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT([Order].*) 
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
    /// SelectTags selectTags =
    ///  SelectTags.Get<Customer>("Id", "CustomerId")
    ///      .Concat<Customer>(["Name", "Email", "Phone"])
    ///      .Append<Order>("Id", "OrderId")
    ///      .Concat<Order>(["OrderDate", "Total"])
    ///      .Append<PaymentMethod>("Id", "PaymentMethodId")
    ///      .Append<PaymentMethod>("ZipCode");
    /// 
    /// ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Order> join1 = new(customerIdEquals);
    /// 
    /// ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
    /// InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);
    /// 
    /// Joins<Customer> joins = new(join1, join2);
    /// 
    /// SqlQuery query = customerGenerator.SelectCount(selectTags, joins, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT
    /// (
    ///     [Customer].[Id] AS CustomerId, 
    ///     [Customer].[Name], 
    ///     [Customer].[Email], 
    ///     [Customer].[Phone], 
    ///     [Order].[Id] AS OrderId, 
    ///     [Order].[OrderDate], 
    ///     [Order].[Total], 
    ///     [PaymentMethod].[Id] AS PaymentMethodId, 
    ///     [PaymentMethod].[ZipCode]
    /// ) 
    /// FROM [Customer] 
    /// INNER JOIN [Order] 
    /// ON ([Customer].[Id] = [Order].[CustomerId]) 
    /// INNER JOIN [PaymentMethod] 
    /// ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
    /// ]]></code>
    /// </example>
    public SqlQuery SelectCount(SelectTagsBase? selects, JoinsBase? joins, Predicates? predicates)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.Columns?.Select(col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectableTableTags);
        StringBuilder queryBuilder;
        AmbiguousResultColumnException? ambiguousResultColumns = AmbiguousResultColumnException.CheckNames(selects);
        if (ambiguousResultColumns is not null)
            throw ambiguousResultColumns;

        if (invalidTags.Any())
        {
            throw new InvalidTableException(invalidTags);
        }

        if (selects is not null && selects.Any())
            queryBuilder = new($"SELECT COUNT({selects.ToSql()}) FROM {Table}");
        else
            queryBuilder = new($"SELECT COUNT({Table}.*) FROM {Table}");

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {joins.ToSql()}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = [.. (joins?.Parameters ?? []).Concat(predicates?.GetParameters() ?? [])],
            CommandType = CommandType.Text
        };
    }
}