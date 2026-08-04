using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

internal static class HavingInsert
{
    public static IEnumerable<SqlQuery> GradesInsertStatements =>
        GradesDataSet.Data
            .Chunk(100)
            .Select(dataSet => new SqlGenerator<Grades>().Insert(null, null, dataSet));
}
