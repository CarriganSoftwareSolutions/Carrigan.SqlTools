// Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class HavingFixture : SqlFixtureBase
{
    public HavingFixture()
        : base
        (
            [Grades.CreateTableSqlServer],
            HavingInsert.GradesInsertStatements
        )
    {
    }
}
