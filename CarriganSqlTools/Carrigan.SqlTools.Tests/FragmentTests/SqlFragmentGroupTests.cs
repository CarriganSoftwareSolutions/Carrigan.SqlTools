using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.Helpers;
using System;


namespace Carrigan.SqlTools.Tests.FragmentTests;


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
        Parameter parameter1 = new("Name", "Jonathan");
        Parameter parameter2 = new("Status", "Active");

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
