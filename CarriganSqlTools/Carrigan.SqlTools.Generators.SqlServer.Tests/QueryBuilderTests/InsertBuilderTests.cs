using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.QueryBuilderTests;

public class InsertBuilderTests
{
    private readonly SqlGenerator<EntityWithTableAttribute> generator = new();

    [Fact]
    public void InsertBuilder_WithInsertColumnsReturnColumnsAndRecords_RendersExpectedSql()
    {
        EntityWithTableAttribute first = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1),
            When = "Now"
        };
        EntityWithTableAttribute second = new()
        {
            Name = "Test Name2",
            DateOf = new DateTime(2025, 12, 6),
            When = "Later"
        };

        InsertBuilder<EntityWithTableAttribute> insertBuilder = new()
        {
            InsertColumns = new ColumnCollection<EntityWithTableAttribute>(nameof(EntityWithTableAttribute.Name), nameof(EntityWithTableAttribute.When)),
            ReturnColumns = new ColumnCollection<EntityWithTableAttribute>(nameof(EntityWithTableAttribute.Id), nameof(EntityWithTableAttribute.DateOf)),
            Records = [first, second]
        };

        SqlQuery query = generator.Insert(insertBuilder);

        string expectedSql =
            "DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER NOT NULL, DateOf DATETIME2 NOT NULL);" + Environment.NewLine +
            "INSERT INTO [Test] ([Name], [When])" + Environment.NewLine +
            "OUTPUT INSERTED.Id, INSERTED.DateOf INTO @OutputTable" + Environment.NewLine +
            "VALUES (@Name_1, @When_2), (@Name_3, @When_4);" + Environment.NewLine +
            "SELECT Id, DateOf FROM @OutputTable;" + Environment.NewLine;

        Assert.Equal(expectedSql, query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Test Name");
        SqlQueryTestHelper.AssertParameterValue(query, "@When_2", "Now");
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_3", "Test Name2");
        SqlQueryTestHelper.AssertParameterValue(query, "@When_4", "Later");
    }
}
