using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;

//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class FromReadMeExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();
    private static readonly SqlGenerator<PhoneModel> phoneGenerator = new();
    private static readonly SqlGenerator<EmailModel> emailGenerator = new();
    private static readonly SqlGenerator<ProcedureExec> procedureExlGenerator = new();

    private static string ModifyInsertQueryToReturnScalar(string queryText)
    {
        // This method mirrors the logic used to transform an insert query to return the id.
        // Build the final query using a temporary table to store the GUID
        StringBuilder sqlQuery = new();
        sqlQuery.AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);");
        sqlQuery.AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"));
        sqlQuery.AppendLine("SELECT InsertedId FROM @OutputTable;");
        return sqlQuery.ToString();
    }

    [Fact]
    public void SelectAllRows()
    {
        SqlQuery query = customerGenerator.SelectAll();

        Assert.Equal("SELECT [Customer].* FROM [Customer]", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectById()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.SelectById(entity);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] = @Parameter_Id)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(42, (int)query.Parameters.Single().Value);
    }

    [Fact]
    public void Insert()
    {
        Customer entity = new() 
        { 
            Id = 42, 
            Name = "Hank", 
            Email = "Hank@example.com", 
            Phone = "+1(555)555-5555" 
        };
        SqlQuery query = customerGenerator.Insert(entity);

        Assert.Equal("INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) VALUES (@Id, @Name, @Email, @Phone);", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
    }

    [Fact]
    public void InsertWithAutoId()
    {
        Customer entity = new() 
        { 
            Name = "Hank", 
            Email = "Hank@example.com",
            Phone= "+1(555)555-5555" 
        };
        SqlQuery query = customerGenerator.InsertAutoId(entity);

        string expectedQueryText = ModifyInsertQueryToReturnScalar("INSERT INTO [Customer] ([Name], [Email], [Phone]) VALUES (@Name, @Email, @Phone);");

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
    }

    [Fact]
    public void UpdateById()
    {
        Customer entity = new() 
        { 
            Id = 42, 
            Name = "Hank Hill", 
            Email = "Hank.Hill@example.com",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.UpdateById(entity);

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank Hill", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank.Hill@example.com", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
    }

    [Fact]
    public void UpdateByIdSelectColumns()
    {
        //Note: SetColumns<T> validates the names of the properties, and throws an error if the property isn't valid
        SetColumns<Customer> columns = new(nameof(Customer.Email));
        Customer entity = new() { Id = 42, Name = "Hank", Email = "Hank@example.gov" };
        SqlQuery query = customerGenerator.UpdateById(entity, columns);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank@example.gov", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
    }

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
    public void DeleteById()
    {
        Customer[] entities = [new() { Id = 1 }, new() { Id = 2 }];
        SqlQuery query = customerGenerator.DeleteById(entities);

        Assert.Equal("DELETE FROM [Customer] WHERE (([Customer].[Id] = @Parameter_0_R_Id) OR ([Customer].[Id] = @Parameter_1_R_Id))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(1, (int)query.Parameters.Where(param => param.Key == "@Parameter_0_R_Id").Single().Value);
        Assert.Equal(2, (int)query.Parameters.Where(param => param.Key == "@Parameter_1_R_Id").Single().Value);
    }

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Customer, Order> join = new(columnEqualsColumn);

        OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SqlQuery query = customerGenerator.Select(join, null, orderByOrderDate, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithTwoPartOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Customer, Order> join = new(columnEqualsColumn);

        OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));
        OrderByItem<Customer> orderByCustomerId = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBy orderBy = new(orderByCustomerId, orderByOrderDate);

        SqlQuery query = customerGenerator.Select(join, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Customer].[Id] DESC, [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Order, Customer> join = new(columnEqualsColumn);

        ColumnValues<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Delete(join, customerEmail);

        Assert.Equal("DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@Parameter_Email").Single().Value);
    }

    [Fact] 
    public void SelectCountWithWhere()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Columns<Order> totalCol = new (nameof(Order.Total));
        Parameters minTotal = new ("Total", 500m);
        GreaterThan greaterThan = new (totalCol, minTotal);

        SqlQuery query = orderGenerator.SelectCount(null, greaterThan);

        Assert.Equal("SELECT COUNT(*) FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }

    [Fact]
    public void UpdateWithJoinsAndWhere()
    {
        //Note: SetColumns<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new () { Id = 10, Total = 123.45m };

        SetColumns<Order> setColumns = new(nameof(Order.Total));

        ColumnEqualsColumn<Order, Customer> columnEqualsColumn = new(nameof(Order.CustomerId), nameof(Customer.Id));

        InnerJoin<Order, Customer> joinOnCustomerId = new(columnEqualsColumn);

        ColumnValues<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Update(entity, setColumns, joinOnCustomerId, customerEmailEquals);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @ParameterSet_Total FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(123.45m, (decimal)query.Parameters.Where(param => param.Key == "@ParameterSet_Total").Single().Value);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@Parameter_Email").Single().Value);
    }

    [Fact]
    public void TableColumnKey()
    {
        PhoneModel phone = new()
        {
            Id = 2718,
            CustomerId = 3141,
            PhoneNumber = "07700 900461"
        };
        SqlQuery query = phoneGenerator.UpdateById(phone);

        string expectedSql = "UPDATE [schema].[Phone] SET [CustomerId] = @CustomerId, [Phone] = @Phone WHERE [Id] = @Id;";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(3, query.GetParameterCount());
        Assert.Equal(2718, query.GetParameterValue<int>("Id"));
        Assert.Equal(3141, query.GetParameterValue<int>("CustomerId"));
        Assert.Equal("07700 900461", query.GetParameterValue<string>("Phone"));
    }

    [Fact]
    public void IdentifierPrimaryKey()
    {
        EmailModel email = new()
        {
            Id = 10,
            CustomerId = 313,
            EmailAddress = "Exterminate@Skaro.gov"
        };
        SqlQuery query = emailGenerator.UpdateById(email);

        string expectedSql = "UPDATE [schema].[Email] SET [CustomerId] = @CustomerId, [Email] = @Email WHERE [Id] = @Id;";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(3, query.GetParameterCount());
        Assert.Equal(10, query.GetParameterValue<int>("Id"));
        Assert.Equal(313, query.GetParameterValue<int>("CustomerId"));
        Assert.Equal("Exterminate@Skaro.gov", query.GetParameterValue<string>("Email"));
    }

    [Fact]
    public void Procedure()
    {
        ProcedureExec procedureExec = new()
        {
            ValueColumn = "DangItBobby"
        };
        SqlQuery query = procedureExlGenerator.Procedure(procedureExec);

        string expectedSql = "[schema].[UpdateThing]";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal("DangItBobby", query.GetParameterValue<string>("SomeValue"));
    }
}
