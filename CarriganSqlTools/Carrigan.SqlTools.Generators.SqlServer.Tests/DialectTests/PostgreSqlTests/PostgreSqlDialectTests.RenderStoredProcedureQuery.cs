using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderStoredProcedureQuery_ParameterFragmentsAndProcedureTag_ReturnsSqlQuery()
    {
        SqlFragmentParameter[] parameters = [new SqlFragmentParameter(new ParameterTag("SomeValue"), 123)];
        SqlQuery actual = Dialect.RenderStoredProcedureQuery(parameters, new ProcedureTag(null, "test_procedure"));
        Assert.NotNull(actual);
    }
}
