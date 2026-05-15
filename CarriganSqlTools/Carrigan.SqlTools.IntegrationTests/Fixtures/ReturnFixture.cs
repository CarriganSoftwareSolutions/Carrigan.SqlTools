//IGNORE SPELLING: localdb respawner dbo

using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class ReturnFixture : SqlFixtureBase
{
    public ReturnFixture()
        : base([ReturnModel.CreateTableSql])
    {
    }
}