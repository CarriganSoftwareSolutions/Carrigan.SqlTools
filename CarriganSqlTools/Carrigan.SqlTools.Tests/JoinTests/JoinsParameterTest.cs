using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;
public class JoinsParameterTest
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectJoinParameterTest()
    {
        ColumnValue<Order> orderCondition = new(nameof(Order.Total), 1000m);
        InnerJoin<Order> join1 = new(orderCondition);

        ColumnValue<PaymentMethod> paymentCondition = new(nameof(PaymentMethod.ZipCode), "37067");
        InnerJoin<PaymentMethod> join2 = new(paymentCondition);
        Joins<Customer> joins = new(join1, join2);

        SqlQuery query = customerGenerator.Select(null, joins, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Parameter_Total) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @Parameter_ZipCode)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Parameter_Total"));
        Assert.Equal("37067", query.GetParameterValue<string>("@Parameter_ZipCode"));
    }

    [Fact]
    public void SelectCountJoinParameterTest()
    {
        ColumnValue<Order> orderCondition = new(nameof(Order.Total), 1000m);
        InnerJoin<Order> join1 = new(orderCondition);

        ColumnValue<PaymentMethod> paymentCondition = new(nameof(PaymentMethod.ZipCode), "37067");
        InnerJoin<PaymentMethod> join2 = new(paymentCondition);

        SqlQuery query = customerGenerator.SelectCount(new Joins<JoinLeftTable>(join1, join2), null);

        Assert.Equal("SELECT COUNT([Customer].*) FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Parameter_Total) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @Parameter_ZipCode)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Parameter_Total"));
        Assert.Equal("37067", query.GetParameterValue<string>("@Parameter_ZipCode"));
    }

    [Fact]
    public void UpdateJoinParameterTest()
    {
        Customer customer = new()
        {
            Id = 8675309,
            Name = "Jenny Tester",
            Email = "Jenny@wall.com",
            Phone = "867-5309"
        };
        ColumnValue<Order> orderCondition = new(nameof(Order.Total), 1000m);
        InnerJoin<Order> join1 = new(orderCondition);

        ColumnValue<PaymentMethod> paymentCondition = new(nameof(PaymentMethod.ZipCode), "37067");
        InnerJoin<PaymentMethod> join2 = new(paymentCondition);

        SqlQuery query = customerGenerator.Update(customer, null, new Joins<JoinLeftTable>(join1, join2), null);

        Assert.Equal("UPDATE [Customer] SET [Customer].[Name] = @ParameterSet_Name, [Customer].[Email] = @ParameterSet_Email, [Customer].[Phone] = @ParameterSet_Phone FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Parameter_Total) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @Parameter_ZipCode)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(5, query.GetParameterCount());
        Assert.Equal("Jenny Tester", query.GetParameterValue<string>("@ParameterSet_Name"));
        Assert.Equal("Jenny@wall.com", query.GetParameterValue<string>("@ParameterSet_Email"));
        Assert.Equal("867-5309", query.GetParameterValue<string>("@ParameterSet_Phone"));
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Parameter_Total"));
        Assert.Equal("37067", query.GetParameterValue<string>("@Parameter_ZipCode"));
    }

    [Fact]
    public void DeleteJoinParameterTest()
    {
        ColumnValue<Order> orderCondition = new(nameof(Order.Total), 1000m);
        InnerJoin<Order> join1 = new(orderCondition);

        ColumnValue<PaymentMethod> paymentCondition = new(nameof(PaymentMethod.ZipCode), "37067");
        InnerJoin<PaymentMethod> join2 = new(paymentCondition);

        SqlQuery query = customerGenerator.Delete(new Joins<JoinLeftTable>(join1, join2), null);
        
        Assert.Equal("DELETE FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Parameter_Total) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @Parameter_ZipCode)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Parameter_Total"));
        Assert.Equal("37067", query.GetParameterValue<string>("@Parameter_ZipCode"));
    }
}
