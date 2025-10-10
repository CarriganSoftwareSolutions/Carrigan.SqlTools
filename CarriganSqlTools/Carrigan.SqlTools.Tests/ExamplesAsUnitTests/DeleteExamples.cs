using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class DeleteExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();


    [Fact]
    public void Delete()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.Delete(entity);

        Assert.Equal("DELETE FROM [Customer] WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
    }

    [Fact]
    public void DeleteAll()
    {
        SqlQuery query = customerGenerator.DeleteAll();

        Assert.Equal("DELETE FROM [Customer];", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void DeleteById()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.DeleteById(entity);

        Assert.Equal("DELETE FROM [Customer] WHERE ([Customer].[Id] = @Parameter_Id)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Parameter_Id").Single().Value);
    }

    [Fact]
    public void DeleteNullNull()
    {
        SqlQuery query = customerGenerator.Delete(null, null);

        Assert.Equal("DELETE FROM [Customer];", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void DeleteWithNullJoin()
    {
        ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
        SqlQuery query = customerGenerator.Delete(null, columnValue);

        Assert.Equal("DELETE FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Parameter_Name").Single().Value);
    }

    [Fact]
    public void DeleteWithNullPredicate()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order, Customer> join = new(predicate);

        SqlQuery query = orderGenerator.Delete(join, null);

        Assert.Equal("DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order, Customer> join = new(predicate);

        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Delete(join, customerEmail);

        Assert.Equal("DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@Parameter_Email").Single().Value);
    }
}
