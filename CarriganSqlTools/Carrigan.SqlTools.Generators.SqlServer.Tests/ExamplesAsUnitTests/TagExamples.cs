using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class TagExamples
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
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 3141);
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_2", "07700 900461");
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 2718);
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
        //Assert.Equal(3, query.GetParameterCount());
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 10);
        //Assert.Equal(10, (int?)query.GetParameterValue("@Id_3"));
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 313);
        //Assert.Equal(313, (int?)query.GetParameterValue("@CustomerId_1"));
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Exterminate@GenericTinCanLand.gov");
        //Assert.Equal("Exterminate@GenericTinCanLand.gov", (string?)query.GetParameterValue("@Email_2"));
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
        //Assert.Equal(1, query.GetParameterCount());
        SqlQueryTestHelper.AssertParameterValue(query, "@SomeValue_1", "DangIt");
        //Assert.Equal("DangIt", (string?)query.GetParameterValue("@SomeValue_1"));
    }
}