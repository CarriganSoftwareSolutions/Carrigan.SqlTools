using System.Data;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed class SqlTypeCacheTests
{
    private enum IntEnum : int
    {
        None = 0
    }

    private enum ByteEnum : byte
    {
        None = 0
    }

    private enum UIntEnum : uint
    {
        None = 0
    }

    private sealed class CustomType
    {
    }

    [Theory]
    [InlineData(typeof(Guid), SqlDbType.UniqueIdentifier)]
    [InlineData(typeof(Guid?), SqlDbType.UniqueIdentifier)]
    [InlineData(typeof(string), SqlDbType.NVarChar)]
    [InlineData(typeof(char), SqlDbType.NChar)]
    [InlineData(typeof(char?), SqlDbType.NChar)]
    [InlineData(typeof(byte[]), SqlDbType.VarBinary)]
    [InlineData(typeof(bool), SqlDbType.Bit)]
    [InlineData(typeof(bool?), SqlDbType.Bit)]
    [InlineData(typeof(byte), SqlDbType.TinyInt)]
    [InlineData(typeof(byte?), SqlDbType.TinyInt)]
    [InlineData(typeof(sbyte), SqlDbType.SmallInt)]
    [InlineData(typeof(sbyte?), SqlDbType.SmallInt)]
    [InlineData(typeof(short), SqlDbType.SmallInt)]
    [InlineData(typeof(short?), SqlDbType.SmallInt)]
    [InlineData(typeof(int), SqlDbType.Int)]
    [InlineData(typeof(int?), SqlDbType.Int)]
    [InlineData(typeof(long), SqlDbType.BigInt)]
    [InlineData(typeof(long?), SqlDbType.BigInt)]
    [InlineData(typeof(float), SqlDbType.Real)]
    [InlineData(typeof(float?), SqlDbType.Real)]
    [InlineData(typeof(double), SqlDbType.Float)]
    [InlineData(typeof(double?), SqlDbType.Float)]
    [InlineData(typeof(decimal), SqlDbType.Decimal)]
    [InlineData(typeof(decimal?), SqlDbType.Decimal)]
    [InlineData(typeof(DateTime), SqlDbType.DateTime2)]
    [InlineData(typeof(DateTime?), SqlDbType.DateTime2)]
    [InlineData(typeof(DateOnly), SqlDbType.Date)]
    [InlineData(typeof(DateOnly?), SqlDbType.Date)]
    [InlineData(typeof(TimeOnly), SqlDbType.Time)]
    [InlineData(typeof(TimeOnly?), SqlDbType.Time)]
    [InlineData(typeof(DateTimeOffset), SqlDbType.DateTimeOffset)]
    [InlineData(typeof(DateTimeOffset?), SqlDbType.DateTimeOffset)]
    [InlineData(typeof(System.Xml.Linq.XDocument), SqlDbType.Xml)]
    [InlineData(typeof(System.Xml.XmlDocument), SqlDbType.Xml)]
    public void GetSqlDbType_KnownTypes(Type type, SqlDbType expectedSqlDbType)
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbType(type);

        Assert.Equal(expectedSqlDbType, sqlDbType);
    }

    [Theory]
    [InlineData(typeof(IntEnum), SqlDbType.Int)]
    [InlineData(typeof(ByteEnum), SqlDbType.TinyInt)]
    [InlineData(typeof(UIntEnum), SqlDbType.Variant)]
    public void GetSqlDbType_EnumTypesUseUnderlyingType(Type enumType, SqlDbType expectedSqlDbType)
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbType(enumType);

        Assert.Equal(expectedSqlDbType, sqlDbType);
    }

    [Fact]
    public void GetSqlDbType_UnknownTypeReturnsVariant()
    {
        Type type = typeof(CustomType);

        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbType(type);

        Assert.Equal(SqlDbType.Variant, sqlDbType);
    }

    [Fact]
    public void GetSqlDbType_Null_Exception() =>
        Assert.Throws<ArgumentNullException>(() => SqlTypeCache.GetSqlDbType(null!));

    [Fact]
    public void GetSqlDbTypeFromValue_NullReturnsVariant()
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbTypeFromValue(null);

        Assert.Equal(SqlDbType.Variant, sqlDbType);
    }

    [Theory]
    [InlineData((byte)1, SqlDbType.TinyInt)]
    [InlineData((sbyte)1, SqlDbType.SmallInt)]
    [InlineData((short)1, SqlDbType.SmallInt)]
    [InlineData(123, SqlDbType.Int)]
    [InlineData(123L, SqlDbType.BigInt)]
    [InlineData(1.0f, SqlDbType.Real)]
    [InlineData(1.0d, SqlDbType.Float)]
    [InlineData('A', SqlDbType.NChar)]
    [InlineData("ABC", SqlDbType.NVarChar)]
    [InlineData(true, SqlDbType.Bit)]
    [InlineData(false, SqlDbType.Bit)]
    public void GetSqlDbTypeFromValue_KnownValues(object value, SqlDbType expectedSqlDbType)
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbTypeFromValue(value);

        Assert.Equal(expectedSqlDbType, sqlDbType);
    }

    [Fact]
    public void GetSqlDbTypeFromValue_GuidMapsToUniqueIdentifier()
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbTypeFromValue(Guid.NewGuid());

        Assert.Equal(SqlDbType.UniqueIdentifier, sqlDbType);
    }

    [Fact]
    public void GetSqlDbTypeFromValue_DateTimeMapsToDateTime2()
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbTypeFromValue(DateTime.UtcNow);

        Assert.Equal(SqlDbType.DateTime2, sqlDbType);
    }

    [Fact]
    public void GetSqlDbTypeFromValue_ByteArrayMapsToVarBinary()
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbTypeFromValue(new byte[] { 1, 2, 3 });

        Assert.Equal(SqlDbType.VarBinary, sqlDbType);
    }

    [Fact]
    public void GetSqlDbTypeFromValue_DecimalMapsToDecimal()
    {
        SqlDbType sqlDbType = SqlTypeCache.GetSqlDbTypeFromValue(decimal.MaxValue);

        Assert.Equal(SqlDbType.Decimal, sqlDbType);
    }

    [Theory]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(Guid?))]

    [InlineData(typeof(string))]
    [InlineData(typeof(char))]
    [InlineData(typeof(char?))]

    [InlineData(typeof(byte[]))]

    [InlineData(typeof(bool))]
    [InlineData(typeof(bool?))]

    [InlineData(typeof(byte))]
    [InlineData(typeof(byte?))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(sbyte?))]
    [InlineData(typeof(short))]
    [InlineData(typeof(short?))]
    [InlineData(typeof(int))]
    [InlineData(typeof(int?))]
    [InlineData(typeof(long))]
    [InlineData(typeof(long?))]

    [InlineData(typeof(float))]
    [InlineData(typeof(float?))]
    [InlineData(typeof(double))]
    [InlineData(typeof(double?))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(decimal?))]

    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTime?))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(DateOnly?))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(TimeOnly?))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateTimeOffset?))]

    [InlineData(typeof(object))]
    [InlineData(typeof(System.Xml.Linq.XDocument))]
    [InlineData(typeof(System.Xml.XmlDocument))]
    public void GetAllCSharpTypes_ContainsExpectedMappedType(Type expectedType)
    {
        IEnumerable<Type> types = SqlTypeCache.GetAllCSharpTypes();

        Assert.Contains(expectedType, types);
    }
}
