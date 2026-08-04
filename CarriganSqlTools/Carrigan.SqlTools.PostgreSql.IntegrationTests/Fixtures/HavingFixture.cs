// Ignore Spelling: Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class HavingFixture : PostgreSqlFixtureBase
{
    public HavingFixture()
        : base
        (
            [Grades.CreateTablePostgreSql],
            HavingInsert.GradesInsertStatements
        )
    {
    }
}
