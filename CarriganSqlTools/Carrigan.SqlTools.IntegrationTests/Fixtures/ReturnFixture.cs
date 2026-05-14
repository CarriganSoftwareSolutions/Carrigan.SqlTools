//IGNORE SPELLING: localdb respawner dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class ReturnFixture : SqlFixtureBase
{
    public ReturnFixture()
        : base(ReturnModel.CreateTableSql)
    {
    }
}