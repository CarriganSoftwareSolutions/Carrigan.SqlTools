using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanColumnBaseTests;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanColumnBaseTests;

public class EntityWithSchemaColumnBaseTest : SqlServerBooleanColumnBaseTest<EntityWithSchema>
{
    protected override string? SchemaName =>
        "myschema";

    protected override string TableName =>
        "EntityWithSchema";

    protected override IEnumerable<string> BooleanProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Id"), NewKvp("Description")]);
}
