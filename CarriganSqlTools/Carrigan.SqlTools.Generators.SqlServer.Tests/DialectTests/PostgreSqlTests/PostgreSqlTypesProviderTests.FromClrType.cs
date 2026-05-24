using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Theory]
    [InlineData(typeof(Guid), "UUID", false)]
    [InlineData(typeof(Guid?), "UUID", true)]
    [InlineData(typeof(string), "TEXT", false)]
    [InlineData(typeof(char), "CHAR", false)]
    [InlineData(typeof(char?), "CHAR", true)]
    [InlineData(typeof(byte[]), "BYTEA", false)]
    [InlineData(typeof(bool), "BOOLEAN", false)]
    [InlineData(typeof(bool?), "BOOLEAN", true)]
    [InlineData(typeof(byte), "SMALLINT", false)]
    [InlineData(typeof(byte?), "SMALLINT", true)]
    [InlineData(typeof(sbyte), "SMALLINT", false)]
    [InlineData(typeof(sbyte?), "SMALLINT", true)]
    [InlineData(typeof(short), "SMALLINT", false)]
    [InlineData(typeof(short?), "SMALLINT", true)]
    [InlineData(typeof(ushort), "INTEGER", false)]
    [InlineData(typeof(ushort?), "INTEGER", true)]
    [InlineData(typeof(int), "INTEGER", false)]
    [InlineData(typeof(int?), "INTEGER", true)]
    [InlineData(typeof(uint), "BIGINT", false)]
    [InlineData(typeof(uint?), "BIGINT", true)]
    [InlineData(typeof(long), "BIGINT", false)]
    [InlineData(typeof(long?), "BIGINT", true)]
    [InlineData(typeof(ulong), "NUMERIC", false)]
    [InlineData(typeof(ulong?), "NUMERIC", true)]
    [InlineData(typeof(float), "REAL", false)]
    [InlineData(typeof(float?), "REAL", true)]
    [InlineData(typeof(double), "DOUBLE PRECISION", false)]
    [InlineData(typeof(double?), "DOUBLE PRECISION", true)]
    [InlineData(typeof(decimal), "NUMERIC", false)]
    [InlineData(typeof(decimal?), "NUMERIC", true)]
    [InlineData(typeof(DateTime), "TIMESTAMP WITHOUT TIME ZONE", false)]
    [InlineData(typeof(DateTime?), "TIMESTAMP WITHOUT TIME ZONE", true)]
    [InlineData(typeof(DateOnly), "DATE", false)]
    [InlineData(typeof(DateOnly?), "DATE", true)]
    [InlineData(typeof(TimeOnly), "TIME WITHOUT TIME ZONE", false)]
    [InlineData(typeof(TimeOnly?), "TIME WITHOUT TIME ZONE", true)]
    [InlineData(typeof(DateTimeOffset), "TIMESTAMP WITH TIME ZONE", false)]
    [InlineData(typeof(DateTimeOffset?), "TIMESTAMP WITH TIME ZONE", true)]
    [InlineData(typeof(XmlDocument), "XML", false)]
    [InlineData(typeof(XDocument), "XML", false)]
    [InlineData(typeof(object), "TEXT", false)]
    public void FromClrType_ReturnsExpectedProviderTypeNameAndNullability(Type clrType, string expectedProviderTypeName, bool expectedNullable)
    {
        FieldProperties actual = PostgreSqlTypesProvider.FromClrType(clrType);

        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    [Fact]
    public void FromClrType_NullType_Exception() => 
        Assert.Throws<ArgumentNullException>(() => PostgreSqlTypesProvider.FromClrType(null!));
}
