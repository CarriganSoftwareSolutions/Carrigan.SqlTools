using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Helpers;

internal static class SqlQueryTestHelper
{
    internal static void AssertParameterValue(SqlQuery sqlQuery, string parameterName, object? expectedValue)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(parameterName);

        SqlFragmentParameter parameter = Assert.Single
        (
            sqlQuery.Parameters,
            parameter => parameter.ParameterTag.ToString() == parameterName
        );

        Assert.Equal(expectedValue, parameter.Value);
    }

    internal static void AssertParameterCount(SqlQuery sqlQuery, int expectedCount)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);

        Assert.Equal(expectedCount, sqlQuery.Parameters.Count());
    }

    /// <summary>
    /// Returns the number of rendered parameters for unit tests.
    /// </summary>
    internal static int GetParameterCount(this SqlQuery sqlQuery)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        return sqlQuery.Parameters.Count();
    }

    internal static void AssertParameterDoesNotExist(SqlQuery sqlQuery, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(parameterName);

        Assert.DoesNotContain
        (
            sqlQuery.Parameters,
            parameter => parameter.ParameterTag.ToString() == parameterName
        );

        if (parameterName.StartsWith('@') is false)
            AssertParameterDoesNotExist(sqlQuery, $"@{parameterName}");
    }

    internal static void AssertParameterExists(SqlQuery sqlQuery, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(parameterName);

        Assert.Single
        (
            sqlQuery.Parameters,
            parameter => parameter.ParameterTag.ToString() == parameterName
        );
    }

    internal static void AssertSingleParameterValue(SqlQuery sqlQuery, object? expectedValue)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);

        SqlFragmentParameter parameter = Assert.Single(sqlQuery.Parameters);
        Assert.Equal(expectedValue, parameter.Value);
    }

    internal static void AssertSingleParameterName(SqlQuery sqlQuery, string expectedParameterName)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(expectedParameterName);

        SqlFragmentParameter parameter = Assert.Single(sqlQuery.Parameters);
        Assert.Equal(expectedParameterName, parameter.ParameterTag.ToString());
    }

    internal static object? GetParameterValue(SqlQuery sqlQuery, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(parameterName);

        ParameterTag tag = new(parameterName);
        SqlFragmentParameter? parameter = sqlQuery.Parameters.SingleOrDefault(parameter => parameter.ParameterTag == tag);

        if (parameter is null)
            throw new KeyNotFoundException($"Parameter '{parameterName}' was not found.");

        return parameter.Value;
    }

    internal static void AssertParameterValue(IEnumerable<SqlFragmentParameter> parameters, string parameterName, object? expectedValue)
    {
        SqlFragmentParameter parameter = Assert.Single
        (
            parameters,
            parameter => parameter.ParameterTag.ToString() == parameterName
        );

        Assert.Equal(expectedValue, parameter.Value);
    }
}
