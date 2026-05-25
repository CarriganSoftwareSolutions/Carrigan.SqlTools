//IGNORE SPELLING: localdb respawner

using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class ReturnFixture : PostgreSqlFixtureBase
{
    public ReturnFixture()
        : base([ReturnModel.CreateTablePostgreSql])
    {
    }
}