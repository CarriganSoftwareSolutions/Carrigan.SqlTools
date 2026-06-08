//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class FieldsFixture : SqlFixtureBase
{
    public FieldsFixture()
        : base([FieldsModel.CreateTableSqlServer])
    {
    }
}