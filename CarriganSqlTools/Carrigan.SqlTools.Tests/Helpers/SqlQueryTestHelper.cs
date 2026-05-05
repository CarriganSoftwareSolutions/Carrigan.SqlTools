using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.Helpers;

internal static class SqlQueryTestHelper
{
    internal static void AssertParameterValue(SqlQuery sqlQuery, string parameterName, object? expectedValue)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(parameterName);

        SqlFragmentParameter enumerableParameter = Assert.Single
        (
            sqlQuery.Parameters,
            parameter => parameter.ParameterTag == parameterName
        );

        Assert.Equal(expectedValue, enumerableParameter.Value);

        KeyValuePair<ParameterTag, object?> dictionaryParameter = Assert.Single
        (
            sqlQuery.ParametersAsDictionary,
            parameter => parameter.Key == parameterName
        );

        Assert.Equal(expectedValue, dictionaryParameter.Value);
    }

    internal static void AssertParameterCount(SqlQuery sqlQuery, int expectedCount)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);

        Assert.Equal(expectedCount, sqlQuery.Parameters.Count());
    }

    internal static void AssertParameterDoesNotExist(SqlQuery sqlQuery, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(parameterName);

        Assert.DoesNotContain
        (
            sqlQuery.Parameters,
            parameter => parameter.ParameterTag == parameterName
        );

        Assert.DoesNotContain
        (
            sqlQuery.ParametersAsDictionary,
            parameter => parameter.Key == parameterName
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
            parameter => parameter.ParameterTag == parameterName
        );

        Assert.Single
        (
            sqlQuery.ParametersAsDictionary,
            parameter => parameter.Key == parameterName
        );
    }

    internal static void AssertSingleParameterValue(SqlQuery sqlQuery, object? expectedValue)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);

        Assert.Single(sqlQuery.Parameters);
        Assert.Single(sqlQuery.ParametersAsDictionary);
        Assert.Equal(expectedValue, sqlQuery.Parameters.Single().Value);
        Assert.Equal(expectedValue, sqlQuery.ParametersAsDictionary.Single().Value);
    }

    internal static void AssertSingleParameterName(SqlQuery sqlQuery, string expectedParameterName)
    {
        ArgumentNullException.ThrowIfNull(sqlQuery);
        ArgumentNullException.ThrowIfNull(expectedParameterName);

        Assert.Single(sqlQuery.Parameters);
        Assert.Single(sqlQuery.ParametersAsDictionary);
        Assert.Equal(expectedParameterName, sqlQuery.Parameters.Single().ParameterTag.ToString());
        Assert.Equal(expectedParameterName, sqlQuery.ParametersAsDictionary.Single().Key.ToString());
    }

    internal static object? GetParameterValue(SqlQuery sqlQuery, string parameterName) =>
        sqlQuery.GetParameterValue(parameterName);
    internal static void AssertParameterValue(IEnumerable<SqlFragmentParameter> parameters, string parameterName, object? expectedValue)
    {
        SqlFragmentParameter parameter = Assert.Single
        (
            parameters,
            parameter => parameter.ParameterTag == parameterName
        );

        Assert.Equal(expectedValue, parameter.Value);
    }
}
