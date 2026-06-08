
using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class ArrayFieldsFixture : PostgreSqlFixtureBase
{
    public ArrayFieldsFixture()
        : base([ArrayFieldsModel.CreateTablePostgreSql])
    {
    }
}
