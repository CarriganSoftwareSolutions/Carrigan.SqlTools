//Ignore Spelling: SqlServer

using Carrigan.SqlTools.SqlServer.IntegrationTests.SqlServerModels;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class SqlAttributeFieldsFixture : SqlFixtureBase
{
    public SqlAttributeFieldsFixture()
        : base([SqlAttributeFieldsModel.CreateTableSqlServer])
    {
    }
}
