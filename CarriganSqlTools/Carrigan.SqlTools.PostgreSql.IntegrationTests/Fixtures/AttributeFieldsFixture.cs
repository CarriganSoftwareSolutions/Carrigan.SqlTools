
using Carrigan.SqlTools.PostgreSql.IntegrationTests.PostgreSqlModels;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class AttributeFieldsFixture : PostgreSqlFixtureBase
{
    public AttributeFieldsFixture()
        : base([AttributeFieldsModel.CreateTablePostgreSql])
    {
    }
}
