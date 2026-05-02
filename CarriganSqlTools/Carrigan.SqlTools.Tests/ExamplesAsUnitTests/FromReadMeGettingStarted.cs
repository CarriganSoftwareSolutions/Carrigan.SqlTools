using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using System.Text;

//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class FromReadMeGettingStarted
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

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

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] = @Id_1)", query.QueryText);
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

        Assert.Equal("INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) VALUES (@Id_1, @Name_2, @Email_3, @Phone_4);", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_1").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_2").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_3").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_4").Single().Value);
    }

    [Fact]
    public void InsertWithAutoId()
    {
        Customer entity = new()
        {
            Name = "Hank",
            Email = "Hank@example.com",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.InsertAutoId(entity);

        string expectedQueryText = ModifyInsertQueryWithReturn("INSERT INTO [Customer] ([Name], [Email], [Phone])", "VALUES (@Name_1, @Email_2, @Phone_3);", "INT");

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_1").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_3").Single().Value);
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

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name_1, [Email] = @Email_2, [Phone] = @Phone_3 WHERE [Id] = @Id_4;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_4").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_1").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_3").Single().Value);
    }

    [Fact]
    public void UpdateByIdSelectColumns()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnCollection<Customer> columns = new(nameof(Customer.Email));
        Customer entity = new() { Id = 42, Name = "Hank", Email = "Hank@example.gov" };
        SqlQuery query = customerGenerator.UpdateById(entity, columns);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email_1 WHERE [Id] = @Id_2;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_2").Single().Value);
        Assert.Equal("Hank@example.gov", (string)query.Parameters.Where(param => param.Key == "@Email_1").Single().Value);
    }

    [Fact]
    public void Delete()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.Delete(entity);

        Assert.Equal("DELETE FROM [Customer] WHERE [Id] = @Id_1;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_1").Single().Value);
    }

    [Fact]
    public void DeleteById()
    {
        Customer[] entities = [new() { Id = 1 }, new() { Id = 2 }];
        SqlQuery query = customerGenerator.DeleteById(entities);

        Assert.Equal("DELETE FROM [Customer] WHERE (([Customer].[Id] = @Id_1) OR ([Customer].[Id] = @Id_2))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(1, (int)query.Parameters.Where(param => param.Key == "@Id_1").Single().Value);
        Assert.Equal(2, (int)query.Parameters.Where(param => param.Key == "@Id_2").Single().Value);
    }
}