//IGNORE SPELLING: localdb respawner dbo

using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class ReturnFixture : SqlFixtureBase
{
    public ReturnFixture()
        : base([ReturnModel.CreateTableSqlServer])
    {
    }
}