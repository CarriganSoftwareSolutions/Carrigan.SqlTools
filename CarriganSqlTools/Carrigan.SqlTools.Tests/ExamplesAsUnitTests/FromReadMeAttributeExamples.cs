using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using Carrigan.SqlTools.Tests.Helpers;
using Carrigan.SqlTools.Generators.SqlServer;


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
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 2718);
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 3141);
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_2", "07700 900461");
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
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 313);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Exterminate@GenericTinCanLand.gov");
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 10);
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
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@SomeValue_1", "DangIt");
    }
}