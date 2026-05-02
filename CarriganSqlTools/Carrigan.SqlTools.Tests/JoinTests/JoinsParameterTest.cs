using Carrigan.SqlTools.Dialects;
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

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Total_1) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @ZipCode_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Total_1"));
        Assert.Equal("37067", query.GetParameterValue<string>("@ZipCode_2"));
    }

    [Fact]
    public void SelectCountJoinParameterTest()
    {
        ColumnValue<Order> orderCondition = new(nameof(Order.Total), 1000m);
        InnerJoin<Order> join1 = new(orderCondition);

        ColumnValue<PaymentMethod> paymentCondition = new(nameof(PaymentMethod.ZipCode), "37067");
        InnerJoin<PaymentMethod> join2 = new(paymentCondition);

        SqlQuery query = customerGenerator.SelectCount(null, new Joins<JoinLeftTable>(join1, join2), null);

        Assert.Equal("SELECT COUNT([Customer].*) FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Total_1) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @ZipCode_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Total_1"));
        Assert.Equal("37067", query.GetParameterValue<string>("@ZipCode_2"));
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

        Assert.Equal("UPDATE [Customer] SET [Customer].[Name] = @Name_1, [Customer].[Email] = @Email_2, [Customer].[Phone] = @Phone_3 FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Total_4) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @ZipCode_5)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(5, query.GetParameterCount());
        Assert.Equal("Jenny Tester", query.GetParameterValue<string>("@Name_1"));
        Assert.Equal("Jenny@wall.com", query.GetParameterValue<string>("@Email_2"));
        Assert.Equal("867-5309", query.GetParameterValue<string>("@Phone_3"));
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Total_4"));
        Assert.Equal("37067", query.GetParameterValue<string>("@ZipCode_5"));
    }

    [Fact]
    public void DeleteJoinParameterTest()
    {
        ColumnValue<Order> orderCondition = new(nameof(Order.Total), 1000m);
        InnerJoin<Order> join1 = new(orderCondition);

        ColumnValue<PaymentMethod> paymentCondition = new(nameof(PaymentMethod.ZipCode), "37067");
        InnerJoin<PaymentMethod> join2 = new(paymentCondition);

        SqlQuery query = customerGenerator.Delete(new Joins<JoinLeftTable>(join1, join2), null);

        Assert.Equal("DELETE FROM [Customer] INNER JOIN [Order] ON ([Order].[Total] = @Total_1) INNER JOIN [PaymentMethod] ON ([PaymentMethod].[ZipCode] = @ZipCode_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1000m, query.GetParameterValue<decimal>("@Total_1"));
        Assert.Equal("37067", query.GetParameterValue<string>("@ZipCode_2"));
    }
}