using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using System;


namespace Carrigan.SqlTools.Tests.FragmentTests;


public class SqlFragmentGroupTests
{
    [Fact]
    public void SqlFragmentGroup_ToSql_ConcatenatesChildFragments()
    {
        SqlFragmentGroup group = new(
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentText("* "),
            new SqlFragmentText("FROM Users")
        ]);

        string sql = group.ToSql();

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

        Parameter[] parameters = [.. outerGroup.GetParameters()];

        Assert.Equal([parameter1, parameter2], parameters);
    }
}
