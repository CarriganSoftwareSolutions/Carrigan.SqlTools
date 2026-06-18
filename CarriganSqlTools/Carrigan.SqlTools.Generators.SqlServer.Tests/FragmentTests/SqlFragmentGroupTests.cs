using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;


namespace Carrigan.SqlTools.Generators.SqlServer.Tests.FragmentTests;


public class SqlFragmentGroupTests
{
    private readonly ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void SqlFragmentGroup_ToSql_ConcatenatesChildFragments()
    {
        SqlFragmentGroup group = new(
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentText("* "),
            new SqlFragmentText("FROM Users")
        ]);

        string sql = group.ToSql(Dialect);

        Assert.Equal("SELECT * FROM Users", sql);
    }

    [Fact]
    public void SqlFragmentGroup_GetParameters_ReturnsParametersFromNestedGroups()
    {
        Parameter parameter1 = new("Jonathan", "Name");
        Parameter parameter2 = new("Active", "Status");

        SqlFragmentGroup innerGroup = new(
        [
            new SqlFragmentText("Status = "),
            new SqlFragmentParameter(parameter2)
        ]);

        SqlFragmentGroup outerGroup = new(
        [
            new SqlFragmentText("Name = "),
            new SqlFragmentParameter(parameter1),
            new SqlFragmentText(" AND "),
            innerGroup
        ]);

        IEnumerable<SqlFragmentParameter> parameters = [.. outerGroup.GetSqlFragmentParameters()];

        //These are pre-final rendered, so they lack the @ and the _#
        int count = parameters.Where(p => p.ParameterTag == "Name" || p.ParameterTag == "Status").Count();

        Assert.Equal(2, count);
    }
}
