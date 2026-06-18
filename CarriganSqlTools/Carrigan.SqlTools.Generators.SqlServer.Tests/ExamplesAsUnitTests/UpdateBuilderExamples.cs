using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class UpdateBuilderExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void UpdateWithSetColumnJoinsAndWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Column<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new()
        {
            Id = 10,
            Total = 123.45m
        };

        ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

        Column<Customer> customerId = new(nameof(Customer.Id));
        Column<Order> orderCustomerId = new(nameof(Order.CustomerId));
        Equal customerIdsEquals = new(orderCustomerId, customerId);
        InnerJoin<Customer> joinOnCustomerId = new(customerIdsEquals);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        UpdateBuilder<Order> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            Joins = joinOnCustomerId,
            Where = customerEmailEquals
        };

        SqlQuery query = orderGenerator.Update(updateBuilder);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @Total_1 FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 123.45m);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "spam@example.com");
    }

    [Fact]
    public void UpdateWithSetColumnWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid

        Customer entity = new()
        {
            Email = "spam@example.com"
        };
        ColumnCollection<Customer> columnCollection = new(nameof(Customer.Email));
        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");

        UpdateBuilder<Customer> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            Where = customerEmailEquals
        };

        SqlQuery query = customerGenerator.Update(updateBuilder);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email_1 WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "spam@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@example.com");
    }
}
