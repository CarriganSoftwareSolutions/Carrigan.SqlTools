namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Theory]
    [InlineData(typeof(Guid[]))]
    [InlineData(typeof(Guid?[]))]
    [InlineData(typeof(string[]))]
    [InlineData(typeof(char[]))]
    [InlineData(typeof(char?[]))]
    [InlineData(typeof(byte[][]))]
    [InlineData(typeof(bool[]))]
    [InlineData(typeof(bool?[]))]
    [InlineData(typeof(byte?[]))]
    [InlineData(typeof(sbyte[]))]
    [InlineData(typeof(sbyte?[]))]
    [InlineData(typeof(short[]))]
    [InlineData(typeof(short?[]))]
    [InlineData(typeof(ushort[]))]
    [InlineData(typeof(ushort?[]))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(int?[]))]
    [InlineData(typeof(uint[]))]
    [InlineData(typeof(uint?[]))]
    [InlineData(typeof(long[]))]
    [InlineData(typeof(long?[]))]
    [InlineData(typeof(ulong[]))]
    [InlineData(typeof(ulong?[]))]
    [InlineData(typeof(float[]))]
    [InlineData(typeof(float?[]))]
    [InlineData(typeof(double[]))]
    [InlineData(typeof(double?[]))]
    [InlineData(typeof(decimal[]))]
    [InlineData(typeof(decimal?[]))]
    [InlineData(typeof(DateTime[]))]
    [InlineData(typeof(DateTime?[]))]
    [InlineData(typeof(DateOnly[]))]
    [InlineData(typeof(DateOnly?[]))]
    [InlineData(typeof(TimeOnly[]))]
    [InlineData(typeof(TimeOnly?[]))]
    [InlineData(typeof(TimeSpan[]))]
    [InlineData(typeof(TimeSpan?[]))]
    [InlineData(typeof(DateTimeOffset[]))]
    [InlineData(typeof(DateTimeOffset?[]))]
    [InlineData(typeof(System.Xml.Linq.XDocument[]))]
    [InlineData(typeof(System.Xml.XmlDocument[]))]
    [InlineData(typeof(object[]))]
    public void SupportedTypes_IncludesPostgreSqlArrayTypes(Type type) =>
        Assert.Contains(type, Dialect.SupportedTypes());

    [Fact]
    public void SupportedTypes_TreatsByteArrayAsByteaScalarType() =>
        Assert.Contains(typeof(byte[]), Dialect.SupportedTypes());
}
