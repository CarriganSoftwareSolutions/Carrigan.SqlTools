using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using System.Text;

//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class FromReadMeGettingStarted
{
    private static readonly SqlGenerator<Customer> customerGenerator = new(new SqlServerDialect());

    private static string ModifyInsertQueryWithReturn(string queryPart1, string queryPart2, string type)
    {
        StringBuilder queryBuilder = new();
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id {type});");
        queryBuilder.AppendLine(queryPart1);
        queryBuilder.AppendLine("OUTPUT INSERTED.Id INTO @OutputTable");
        queryBuilder.AppendLine(queryPart2);
        queryBuilder.AppendLine("SELECT Id FROM @OutputTable;");
        return queryBuilder.ToString();
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
        SqlQuery query = customerGenerator.Insert(null, null, entity);

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

        string expectedQueryText = ModifyInsertQueryWithReturn("INSERT INTO [Customer] ([Name], [Email], [Phone])","VALUES (@Name, @Email, @Phone);", "INT");

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
            Name = "Hank", 
            Email = "Hank@example.com",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.UpdateById(entity);

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
    }

    [Fact]
    public void UpdateByIdSelectColumns()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnCollection<Customer> columns = new(nameof(Customer.Email));
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
}
