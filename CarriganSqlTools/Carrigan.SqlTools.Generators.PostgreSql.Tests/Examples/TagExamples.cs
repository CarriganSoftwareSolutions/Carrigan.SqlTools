using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

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

        string expectedSql = """UPDATE "schema"."Phone" SET "CustomerId" = $1, "Phone" = $2 WHERE "Id" = $3;""";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 3141);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "07700 900461");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 2718);
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

        string expectedSql = """UPDATE "schema"."Email" SET "CustomerId" = $1, "Email" = $2 WHERE "Id" = $3;""";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 10);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 313);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Exterminate@GenericTinCanLand.gov");
    }

    [Fact]
    public void Procedure()
    {
        ProcedureExec procedureExec = new()
        {
            ValueColumn = "DangIt"
        };
        SqlQuery query = procedureExecGenerator.Procedure(procedureExec);

        string expectedSql =
            """
            "schema"."UpdateThing"
            """;
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        //Assert.Equal(1, query.GetParameterCount());
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "DangIt");
        //Assert.Equal("DangIt", (string?)query.GetParameterValue("$1"));
    }
}