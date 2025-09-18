
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
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
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);

        Parameters parameterEmail = new ("Email", "Hank@example.com");
        Columns<Customer> columnEmail = new(nameof(Customer.Email));
        Equal equalEmail = new(columnEmail, parameterEmail);


        Parameters parameterPhone = new("Phone", "+1(555)555-5555");
        Columns<Customer> columnPhone = new(nameof(Customer.Phone));
        Equal equalPhone = new(columnPhone, parameterPhone);

        And and = new (equalName, equalEmail, equalPhone);

        SqlQuery query = customerGenerator.Select(null, and, null, null);

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
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        And and = new(equalName);
        SqlQuery query = customerGenerator.Select(null, and, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
        /// </summary>
    }

    [Fact]
    public void PredicateColumnValues()
    {
        ColumnValues<Customer> columnValue = new(nameof(Customer.Name), "Hank");
        SqlQuery query = customerGenerator.Select(null, columnValue, null, null);

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
        LeftJoin<Customer, Order> join = new(columnValue);
        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void PredicateColumn()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, equalName, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateContains()
    {
        Parameters parameterEmail = new("Email", "@example.");
        Columns<Customer> columnEmail = new(nameof(Customer.Email));
        Contains<Customer> predicate = new(columnEmail, parameterEmail);
        SqlQuery query = customerGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE CONTAINS([Customer].[Email], @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("@example.", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
    }

    [Fact]
    public void PredicateEqual()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, equalName, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateGreaterThan()
    {
        Parameters parameterTotal = new("Total", 1776.00m);
        Columns<Order> columnTotal = new(nameof(Order.Total));
        GreaterThan predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateGreaterThanEquals()
    {
        Parameters parameterTotal = new("Total", 1776.00m);
        Columns<Order> columnTotal = new(nameof(Order.Total));
        GreaterThanEquals predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] >= @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateIsNotNull()
    {
        Columns<Customer> columnName = new(nameof(Customer.Name));
        IsNotNull notNull = new(columnName);
        SqlQuery query = customerGenerator.Select(null, notNull, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NOT NULL)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void PredicateIsNull()
    {
        Columns<Customer> columnName = new(nameof(Customer.Name));
        IsNull isNull = new(columnName);
        SqlQuery query = customerGenerator.Select(null, isNull, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NULL)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void PredicateLessThan()
    {
        Parameters parameterTotal = new("Total", 1776.00m);
        Columns<Order> columnTotal = new(nameof(Order.Total));
        LessThan predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] < @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateLike()
    {
        Parameters parameterEmail = new("Email", "%@example.com");
        Columns<Customer> columnEmail = new(nameof(Customer.Email));
        Like predicate = new(columnEmail, parameterEmail);
        SqlQuery query = customerGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Email] LIKE @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("%@example.com", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
    }

    [Fact]
    public void PredicateNot()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equal = new(columnName, parameterName);
        Not not = new(equal);
        SqlQuery query = customerGenerator.Select(null, not, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (NOT ([Customer].[Name] = @Parameter_Name))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateNotEqual()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        NotEqual predicate = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] <> @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateLessThanEquals()
    {
        Parameters parameterTotal = new("Total", 1776.00m);
        Columns<Order> columnTotal = new(nameof(Order.Total));
        LessThanEquals predicate = new(columnTotal, parameterTotal);
        SqlQuery query = orderGenerator.Select(null, predicate, null, null);

        Assert.Equal("SELECT [Order].* FROM [Order] WHERE ([Order].[Total] <= @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal(1776.00m, (decimal)query.Parameters.Single(param => param.Key == "@Parameter_Total").Value);
    }

    [Fact]
    public void PredicateOr()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);

        Parameters parameterEmail = new("Email", "Hank@example.com");
        Columns<Customer> columnEmail = new(nameof(Customer.Email));
        Equal equalEmail = new(columnEmail, parameterEmail);


        Parameters parameterPhone = new("Phone", "+1(555)555-5555");
        Columns<Customer> columnPhone = new(nameof(Customer.Phone));
        Equal equalPhone = new(columnPhone, parameterPhone);

        Or or = new(equalName, equalEmail, equalPhone);

        SqlQuery query = customerGenerator.Select(null, or, null, null);

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
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        Or or = new(equalName);
        SqlQuery query = customerGenerator.Select(null, or, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

    [Fact]
    public void PredicateParameter()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);
        SqlQuery query = customerGenerator.Select(null, equalName, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
    }

}
