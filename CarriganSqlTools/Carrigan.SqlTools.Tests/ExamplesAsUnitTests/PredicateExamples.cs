
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class PredicateExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();


    [Fact]
    public void PredicateAnd()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);

        Parameter parameterEmail = new ("Email", "Hank@example.com");
        Column<Customer> columnEmail = new(nameof(Customer.Email));
        Equal equalEmail = new(columnEmail, parameterEmail);


        Parameter parameterPhone = new("Phone", "+1(555)555-5555");
        Column<Customer> columnPhone = new(nameof(Customer.Phone));
        Equal equalPhone = new(columnPhone, parameterPhone);

        And and = new (equalName, equalEmail, equalPhone);

        SqlQuery query = customerGenerator.Select(null, null, and, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (([Customer].[Name] = @Parameter_Name) AND ([Customer].[Email] = @Parameter_Email) AND ([Customer].[Phone] = @Parameter_Phone))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Single(param => param.Key == "@Parameter_Phone").Value);
    }

    [Fact]
    public void PredicateSingleAnd()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        And and = new(equalName);
        SqlQuery query = customerGenerator.Select(null, null, and, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
        /// </summary>
    }

    [Fact]
    public void PredicateColumnValues()
    {
        ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
        SqlQuery query = customerGenerator.Select(null, null, columnValue, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateColumnEqualsColumn()
    {
        //Note: ColumnEqualsColumn<leftT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> columnValue = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Joins<Customer> joins = LeftJoin<Order>.Joins<Customer>(columnValue);
        SqlQuery query = customerGenerator.Select(null, joins, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void PredicateColumn()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, null, equalName, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateContains()
    {
        Parameter parameterEmail = new("Email", "@example.");
        Column<Customer> columnEmail = new(nameof(Customer.Email));
        Contains<Customer> predicate = new(columnEmail, parameterEmail);
        SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE CONTAINS([Customer].[Email], @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("@example.", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
    }

    [Fact]
    public void PredicateEqual()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, null, equalName, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateGreaterThan()
    {
        Parameter parameterTotal = new("Total", 1776.00m);
        Column<Order> columnTotal = new(nameof(Order.Total));
        GreaterThan predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateGreaterThanEquals()
    {
        Parameter parameterTotal = new("Total", 1776.00m);
        Column<Order> columnTotal = new(nameof(Order.Total));
        GreaterThanEqual predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] >= @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateIsNotNull()
    {
        Column<Customer> columnName = new(nameof(Customer.Name));
        IsNotNull notNull = new(columnName);
        SqlQuery query = customerGenerator.Select(null, null, notNull, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NOT NULL)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void PredicateIsNull()
    {
        Column<Customer> columnName = new(nameof(Customer.Name));
        IsNull isNull = new(columnName);
        SqlQuery query = customerGenerator.Select(null, null, isNull, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NULL)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void PredicateLessThan()
    {
        Parameter parameterTotal = new("Total", 1776.00m);
        Column<Order> columnTotal = new(nameof(Order.Total));
        LessThan predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] < @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateLike()
    {
        Parameter parameterEmail = new("Email", "%@example.com");
        Column<Customer> columnEmail = new(nameof(Customer.Email));
        Like predicate = new(columnEmail, parameterEmail);
        SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Email] LIKE @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("%@example.com", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
    }

    [Fact]
    public void PredicateNot()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equal = new(columnName, parameterName);
        Not not = new(equal);
        SqlQuery query = customerGenerator.Select(null, null, not, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (NOT ([Customer].[Name] = @Parameter_Name))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateNotEqual()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        NotEqual predicate = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] <> @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateLessThanEquals()
    {
        Parameter parameterTotal = new("Total", 1776.00m);
        Column<Order> columnTotal = new(nameof(Order.Total));
        LessThanEqual predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] <= @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateOr()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);

        Parameter parameterEmail = new("Email", "Hank@example.com");
        Column<Customer> columnEmail = new(nameof(Customer.Email));
        Equal equalEmail = new(columnEmail, parameterEmail);


        Parameter parameterPhone = new("Phone", "+1(555)555-5555");
        Column<Customer> columnPhone = new(nameof(Customer.Phone));
        Equal equalPhone = new(columnPhone, parameterPhone);

        Or or = new(equalName, equalEmail, equalPhone);

        SqlQuery query = customerGenerator.Select(null, null, or, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (([Customer].[Name] = @Parameter_Name) OR ([Customer].[Email] = @Parameter_Email) OR ([Customer].[Phone] = @Parameter_Phone))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Single(param => param.Key == "@Parameter_Phone").Value);
    }

    [Fact]
    public void PredicateSingleOr()
    {
        //This method of having an add with only one predicate is supported for uses cases when you might only have one predicate, or might have more.
        //This way you don't HAVE to test for an the only one edge case. Instead OR intelligently ignores itself.
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        Or or = new(equalName);
        SqlQuery query = customerGenerator.Select(null, null, or, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateParameter()
    {
        Parameter parameterName = new("Name", "Hank");
        Column<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, null, equalName, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

}
