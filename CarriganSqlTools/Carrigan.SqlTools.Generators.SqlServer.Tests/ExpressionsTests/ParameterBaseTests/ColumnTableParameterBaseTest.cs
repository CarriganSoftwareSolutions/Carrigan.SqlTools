using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ParameterBaseTests;

public class ColumnTableParameterBaseTest : SqlServerParameterBaseTest<ColumnTable>
{
    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(ColumnTable.Col1)),
            NewKvp(nameof(ColumnTable.Col2)),
            NewKvp(nameof(ColumnTable.ColA)),
            NewKvp(nameof(ColumnTable.ColB)),
            NewKvp(nameof(ColumnTable.Pizza)),
            NewKvp(nameof(ColumnTable.D000destruct0)),
            NewKvp(nameof(ColumnTable.Express))
        ]
    );
}
