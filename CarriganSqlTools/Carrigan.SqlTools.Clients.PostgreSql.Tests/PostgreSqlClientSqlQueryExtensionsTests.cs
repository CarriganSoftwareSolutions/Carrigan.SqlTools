using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Types;
using NpgsqlTypes;
using System.Reflection;

namespace Carrigan.SqlTools.Clients.PostgreSql.Tests;

public sealed class PostgreSqlClientSqlQueryExtensionsTests
{
    [Theory]
    [InlineData("INTEGER", NpgsqlDbType.Integer)]
    [InlineData("TEXT", NpgsqlDbType.Text)]
    [InlineData("UUID", NpgsqlDbType.Uuid)]
    [InlineData("DOUBLE PRECISION", NpgsqlDbType.Double)]
    [InlineData("NUMERIC", NpgsqlDbType.Numeric)]
    public void FieldPropertiesToNpgsqlDbType_ScalarType_ReturnsScalarNpgsqlDbType(string providerTypeName, NpgsqlDbType expected)
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = providerTypeName,
            IsArray = false
        };

        NpgsqlDbType actual = InvokeFieldPropertiesToNpgsqlDbType(fieldProperties);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("INTEGER", NpgsqlDbType.Integer)]
    [InlineData("TEXT", NpgsqlDbType.Text)]
    [InlineData("UUID", NpgsqlDbType.Uuid)]
    [InlineData("DOUBLE PRECISION", NpgsqlDbType.Double)]
    [InlineData("NUMERIC", NpgsqlDbType.Numeric)]
    public void FieldPropertiesToNpgsqlDbType_ArrayType_ReturnsArrayNpgsqlDbType(string providerTypeName, NpgsqlDbType expectedElementType)
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = providerTypeName,
            IsArray = true
        };

        NpgsqlDbType actual = InvokeFieldPropertiesToNpgsqlDbType(fieldProperties);

        Assert.Equal(NpgsqlDbType.Array | expectedElementType, actual);
    }

    private static NpgsqlDbType InvokeFieldPropertiesToNpgsqlDbType(FieldProperties fieldProperties)
    {
        Type sqlQueryExtensionsType = typeof(Commands).Assembly.GetType("Carrigan.SqlTools.Clients.PostgreSql.SqlQueryExtensions")!;
        MethodInfo methodInfo = sqlQueryExtensionsType.GetMethod("FieldPropertiesToNpgsqlDbType", BindingFlags.NonPublic | BindingFlags.Static)!;

        return (NpgsqlDbType)methodInfo.Invoke(null, [fieldProperties])!;
    }
}
