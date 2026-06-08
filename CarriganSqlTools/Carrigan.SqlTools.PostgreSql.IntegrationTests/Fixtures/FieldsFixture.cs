//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class FieldsFixture : PostgreSqlFixtureBase
{
    public FieldsFixture()
        : base([FieldsModel.CreateTablePostgreSql])
    {
    }
}