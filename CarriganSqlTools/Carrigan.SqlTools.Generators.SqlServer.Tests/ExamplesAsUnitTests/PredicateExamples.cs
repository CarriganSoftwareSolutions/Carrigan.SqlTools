using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;


namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class PredicateExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();


    [Fact]
    public void PredicateAnd()
    {
        ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
        ColumnValue<Customer> equalEmail = new(nameof(Customer.Email), "Hank@example.com");
        ColumnValue<Customer> equalPhone = new(nameof(Customer.Phone), "+1(555)555-5555");
        And and = new(equalName, equalEmail, equalPhone);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = and
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (([Customer].[Name] = @Name_1) AND ([Customer].[Email] = @Email_2) AND ([Customer].[Phone] = @Phone_3))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_3", "+1(555)555-5555");
    }

    [Fact]
    public void PredicateSingleAnd()
    {
        ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
        And and = new(equalName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = and
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateColumn()
    {
        Parameter parameterName = new("Hank", "Name");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = equalName
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateColumnValues()
    {
        ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = columnValue
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateColumnEqualsColumn()
    {
        //Note: ColumnEqualsColumn<leftT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> columnValue = new(nameof(Customer.Id), nameof(Order.CustomerId));
        LeftJoin<Order> join = new(columnValue);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void PredicateContains()
    {
        Parameter parameterEmail = new("@example.", "Email");
        Column<Customer> columnEmail = new(nameof(Customer.Email));
        Contains<Customer> predicate = new(columnEmail, parameterEmail);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE CONTAINS([Customer].[Email], @Email_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "@example.");
    }

    [Fact]
    public void PredicateEqual()
    {
        Parameter parameterName = new("Hank", "Name");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = equalName
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }


    [Fact]
    public void PredicateExists()
    {
        Predicates orderTotalGreaterThan = new GreaterThan
        (
            new Column<Order>(nameof(Order.Total)),
            new Parameter(100.00m, "Total")
        );
        Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, orderTotalGreaterThan, null, null, null);
        Exists exists = new(subQuery);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = exists
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 100.00m);
    }

    [Fact]
    public void PredicateGreaterThan()
    {
        Parameter parameterTotal = new(1776.00m, "Total");
        Column<Order> columnTotal = new(nameof(Order.Total));
        GreaterThan predicate = new(columnTotal, parameterTotal);
        SelectBuilder<Order> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 1776.00m);
    }

    [Fact]
    public void PredicateGreaterThanEquals()
    {
        Parameter parameterTotal = new(1776.00m, "Total");
        Column<Order> columnTotal = new(nameof(Order.Total));
        GreaterThanEqual predicate = new(columnTotal, parameterTotal);
        SelectBuilder<Order> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] >= @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        //Assert.Single(query.Parameters);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 1776.00m);
    }

    [Fact]
    public void PredicateIsNotNull()
    {
        Column<Customer> columnName = new(nameof(Customer.Name));
        IsNotNull notNull = new(columnName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = notNull
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NOT NULL)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void PredicateIsNull()
    {
        Column<Customer> columnName = new(nameof(Customer.Name));
        IsNull isNull = new(columnName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = isNull
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NULL)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void PredicateLessThan()
    {
        Parameter parameterTotal = new(1776.00m, "Total");
        Column<Order> columnTotal = new(nameof(Order.Total));
        LessThan predicate = new(columnTotal, parameterTotal);
        SelectBuilder<Order> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] < @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 1776.00m);
    }

    [Fact]
    public void PredicateLessThanEquals()
    {
        Parameter parameterTotal = new(1776.00m, "Total");
        Column<Order> columnTotal = new(nameof(Order.Total));
        LessThanEqual predicate = new(columnTotal, parameterTotal);
        SelectBuilder<Order> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] <= @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 1776.00m);
    }

    [Fact]
    public void PredicateLike()
    {
        Parameter parameterEmail = new("%@example.com", "Email");
        Column<Customer> columnEmail = new(nameof(Customer.Email));
        Like predicate = new(columnEmail, parameterEmail);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Email] LIKE @Email_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "%@example.com");
    }

    [Fact]
    public void PredicateNot()
    {
        Parameter parameterName = new("Hank", "Name");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equal = new(columnName, parameterName);
        Not not = new(equal);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = not
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (NOT ([Customer].[Name] = @Name_1))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateNotEqual()
    {
        Parameter parameterName = new("Hank", "Name");
        Column<Customer> columnName = new(nameof(Customer.Name));
        NotEqual predicate = new(columnName, parameterName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = predicate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] <> @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateNotExists()
    {
        Predicates orderTotalGreaterThan = new GreaterThan
        (
            new Column<Order>(nameof(Order.Total)),
            new Parameter(100.00m, "Total")
        );
        Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, orderTotalGreaterThan, null, null, null);
        NotExists notExists = new(subQuery);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = notExists
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (NOT EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 100.00m);
    }

    [Fact]
    public void PredicateOr()
    {
        ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
        ColumnValue<Customer> equalEmail = new(nameof(Customer.Email), "Hank@example.com");
        ColumnValue<Customer> equalPhone = new(nameof(Customer.Phone), "+1(555)555-5555");
        Or or = new(equalName, equalEmail, equalPhone);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = or
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (([Customer].[Name] = @Name_1) OR ([Customer].[Email] = @Email_2) OR ([Customer].[Phone] = @Phone_3))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_3", "+1(555)555-5555");
    }

    [Fact]
    public void PredicateSingleOr()
    {
        //This method of having an add with only one predicate is supported for uses cases when you might only have one predicate, or might have more.
        //This way you don't HAVE to test for an the only one edge case. Instead OR intelligently ignores itself.

        ColumnValue<Customer> equalName = new(nameof(Customer.Name), "Hank");
        Or or = new(equalName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = or
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateParameter()
    {
        Parameter parameterName = new("Hank", "Name");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = equalName
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void PredicateParameterGeneric()
    {
        Parameter<Customer> parameterName = new(nameof(Customer.Name), "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = equalName
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }
}