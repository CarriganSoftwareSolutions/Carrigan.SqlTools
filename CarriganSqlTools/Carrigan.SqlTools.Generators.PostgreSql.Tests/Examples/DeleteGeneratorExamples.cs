using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class DeleteGeneratorExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void Delete()
    {
        DeleteBuilder<Customer> deleteBuilder = new()
        {
        };

        SqlQuery query = customerGenerator.Delete(deleteBuilder);

        string expectedQueryText =
            """
            DELETE FROM "Customer";
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithWhere()
    {
        ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
        DeleteBuilder<Customer> deleteBuilder = new()
        {
            Where = columnValue
        };

        SqlQuery query = customerGenerator.Delete(deleteBuilder);

        string expectedQueryText =
            """
            DELETE FROM "Customer" WHERE ("Customer"."Name" = $1)
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
    }

    [Fact]
    public void DeleteWithUsingWhere()
    { 
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Usings = [TableTag.Get<Customer>()],
            Where = predicate
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        string expectedQueryText =
            """
            DELETE FROM "Order" USING "Customer" WHERE ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithUsingWhereParameter()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Usings = [TableTag.Get<Customer>()],
            Where = new And(predicate, customerEmail)
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        string expectedQueryText =
            """
            DELETE FROM "Order" USING "Customer" WHERE (("Customer"."Id" = "Order"."CustomerId") AND ("Customer"."Email" = $1))
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "spam@example.com");
    }


    [Fact]
    public void DeleteWithUsingJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnEqualsColumn<Order, PaymentMethod> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));

        ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
        InnerJoin<PaymentMethod> paymentMethodJoin = new(paymentMethodIdEquals);
        Joins<Order> joins = new(paymentMethodJoin);

        ColumnValue<PaymentMethod> paymentMethodZipCode = new(nameof(PaymentMethod.ZipCode), "37040");

        DeleteBuilder<Customer, Order> deleteBuilder = new()
        {
            Usings = [TableTag.Get<Order>()],
            Joins = joins,
            Where = new And(customerIdEquals, paymentMethodZipCode)
        };

        SqlQuery query = customerGenerator.Delete(deleteBuilder);

        string expectedQueryText =
            """
            DELETE FROM "Customer" USING "Order" INNER JOIN "PaymentMethod" ON ("Order"."PaymentMethodId" = "PaymentMethod"."Id") WHERE (("Customer"."Id" = "Order"."CustomerId") AND ("PaymentMethod"."ZipCode" = $1))
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "37040");
    }
}
