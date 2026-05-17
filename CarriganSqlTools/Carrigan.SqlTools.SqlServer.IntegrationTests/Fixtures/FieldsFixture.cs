//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class FieldsFixture : SqlFixtureBase
{
    public FieldsFixture()
        : base([FieldsModel.CreateTableSql])
    {
    }
}