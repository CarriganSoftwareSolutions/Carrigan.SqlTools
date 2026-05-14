//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class FieldsFixture : SqlFixtureBase
{
    public FieldsFixture()
        : base(FieldsModel.CreateTableSql)
    {
    }
}