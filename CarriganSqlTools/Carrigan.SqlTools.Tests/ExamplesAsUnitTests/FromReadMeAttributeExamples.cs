using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.


//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class FromReadMeAttributeExamples
{
    private static readonly SqlGenerator<PhoneModel> phoneGenerator = new();
    private static readonly SqlGenerator<EmailModel> emailGenerator = new();
    private static readonly SqlGenerator<ProcedureExec> procedureExecGenerator = new();

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

        string expectedSql = "UPDATE [schema].[Phone] SET [CustomerId] = @CustomerId_1, [Phone] = @Phone_2 WHERE [Id] = @Id_3;";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(3, query.GetParameterCount());
        Assert.Equal(2718, query.GetParameterValue<int>("@Id_3"));
        Assert.Equal(3141, query.GetParameterValue<int>("@CustomerId_1"));
        Assert.Equal("07700 900461", query.GetParameterValue<string>("@Phone_2"));
    }

    [Fact]
    public void IdentifierPrimaryKey()
    {
        EmailModel email = new()
        {
            Id = 10,
            CustomerId = 313,
            EmailAddress = "Exterminate@GenericTinCanLand.gov"
        };
        SqlQuery query = emailGenerator.UpdateById(email);

        string expectedSql = "UPDATE [schema].[Email] SET [CustomerId] = @CustomerId_1, [Email] = @Email_2 WHERE [Id] = @Id_3;";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(3, query.GetParameterCount());
        Assert.Equal(10, query.GetParameterValue<int>("@Id_3"));
        Assert.Equal(313, query.GetParameterValue<int>("@CustomerId_1"));
        Assert.Equal("Exterminate@GenericTinCanLand.gov", query.GetParameterValue<string>("@Email_2"));
    }

    [Fact]
    public void Procedure()
    {
        ProcedureExec procedureExec = new()
        {
            ValueColumn = "DangIt"
        };
        SqlQuery query = procedureExecGenerator.Procedure(procedureExec);

        string expectedSql = "[schema].[UpdateThing]";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal("DangIt", query.GetParameterValue<string>("@SomeValue_1"));
    }
}