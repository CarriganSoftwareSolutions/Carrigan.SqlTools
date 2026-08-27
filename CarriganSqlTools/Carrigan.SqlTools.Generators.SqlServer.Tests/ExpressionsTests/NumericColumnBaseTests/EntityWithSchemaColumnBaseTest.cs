using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericColumnBaseTests;

public class EntityWithSchemaColumnBaseTest : SqlServerNumericColumnBaseTest<EntityWithSchema>
{
    protected override string? SchemaName =>
        "myschema";

    protected override string TableName =>
        "EntityWithSchema";

    protected override IEnumerable<string> NumericProperties =>
        ["Id"];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Id"), NewKvp("Description")]);
}
