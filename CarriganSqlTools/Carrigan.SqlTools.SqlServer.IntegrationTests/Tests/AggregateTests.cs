using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;
using System.Reflection.Emit;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public class AggregateTests : IClassFixture<AggregateFixture>
{
    private readonly AggregateFixture _fixture;

    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();

    public AggregateTests(AggregateFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task AggregateTest()
    {
        await _fixture.ResetAsync();

        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Gender)),
            SelectTagGenerator.Get<Book>(nameof(Book.Title)),

            //TODO: SelectTags for Aggregate expressions will throw an exception if no alias is provided, this needs bullet proofing.

            new SelectTag(new Avg(new Cast(new Column<Customer>(nameof(Customer.Age)), SqlServerTypesProvider.AsDecimal())), "Average"),

            new SelectTag(new Max(new Column<Customer>(nameof(Customer.Age))), "Max"),

            new SelectTag(new Min(new Column<Customer>(nameof(Customer.Age))), "Min"),

            new SelectTag(new Count(new Column<Address>(nameof(Address.State))), "Count"),

            new SelectTag(new Sum(new Column<Address>(nameof(Address.StreetNumber))), "Sum")
        );

        Joins<Customer> joins = new
        (
            new InnerJoin<Order>
            (
                new ColumnEqualsColumn<Customer, Order>
                (
                    nameof(Customer.Id),
                    nameof(Order.CustomerId)
                )
            ),

            new InnerJoin<OrderedItem>
            (
                new ColumnEqualsColumn<Order, OrderedItem>
                (
                    nameof(Order.Id),
                    nameof(OrderedItem.OrderId)
                )
            ),

            new InnerJoin<Book>
            (
                new ColumnEqualsColumn<Book, OrderedItem>
                (
                    nameof(Book.Id),
                    nameof(OrderedItem.BookId)
                )
            ),

            new InnerJoin<Address>
            (
                new ColumnEqualsColumn<Order, Address>
                (
                    nameof(Order.AddressId),
                    nameof(Address.Id)
                )
            )
        );

        GroupBys groupBys = new
        (
            new GroupBy<Customer>(nameof(Customer.Gender)),
            new GroupBy<Book>(nameof(Book.Title))
        );

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects,
            Joins = joins,
            Where = new IsNotNull(
                new Column<Book>(nameof(Book.Price))),
            GroupBys = groupBys
        };

        SqlQuery query = CustomerSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<AggregateResult> allAggregateData = await CommandsAsync.ExecuteReaderAsync<AggregateResult>(query, null, connection);

        CustomerBookAggregateDataSet.Validate(allAggregateData);
    }
}
