//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class FieldsFixture : PostgreSqlFixtureBase
{
    public FieldsFixture()
        : base(FieldsModel.CreateTableSql)
    {
    }
}