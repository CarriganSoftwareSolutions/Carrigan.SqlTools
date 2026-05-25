using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.QueryBuilderTests;

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
            "INSERT INTO \"Test\" (\"Name\", \"When\")" + Environment.NewLine +
            "VALUES ($1, $2), ($3, $4)" + Environment.NewLine +
            "RETURNING \"Id\", \"DateOf\";" + Environment.NewLine;

        Assert.Equal(expectedSql, query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Test Name");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Now");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "Test Name2");
        SqlQueryTestHelper.AssertParameterValue(query, "$4", "Later");
    }
}
