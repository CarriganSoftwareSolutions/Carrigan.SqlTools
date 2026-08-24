using Carrigan.SqlTools.Base.Tests;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;

public abstract class SqlServerColumnBaseTest<modelT> : ColumnBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new SqlServerDialect();

    protected override string ExpectSqlFragment(string expectedColumnName) =>
        SchemaName is null
            ? string.Format("[{0}].[{1}]", TableName, expectedColumnName)
            : string.Format("[{0}].[{1}].[{2}]", SchemaName, TableName, expectedColumnName);
}
