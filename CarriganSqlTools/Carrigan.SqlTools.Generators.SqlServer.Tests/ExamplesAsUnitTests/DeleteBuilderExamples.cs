using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class DeleteBuilderExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void DeleteNullNull()
    {
        DeleteBuilder<Customer> deleteBuilder = new()
        {
        };

        SqlQuery query = customerGenerator.Delete(deleteBuilder);

        Assert.Equal("DELETE FROM [Customer];", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithNullJoin()
    {
        ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
        DeleteBuilder<Customer> deleteBuilder = new()
        {
            Where = columnValue
        };

        SqlQuery query = customerGenerator.Delete(deleteBuilder);

        Assert.Equal("DELETE FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void DeleteWithNullPredicate()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Customer> join = new(predicate);

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        Assert.Equal("DELETE [Order] FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Joins = join,
            Where = customerEmail
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        Assert.Equal("DELETE [Order] FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Email_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "spam@example.com");
    }
}
