using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Types;
using System.Data;

//Ignore Spelling: enums

namespace Carrigan.SqlTools.Tests.ReflectorCacheTests;

public sealed class SqlTypeCacheTests
{
    [Theory]
    [InlineData(typeof(int), SqlDbType.Int)]
    [InlineData(typeof(double), SqlDbType.Float)]
    [InlineData(typeof(float), SqlDbType.Real)]
    [InlineData(typeof(decimal), SqlDbType.Decimal)]
    [InlineData(typeof(string), SqlDbType.NVarChar)]
    [InlineData(typeof(byte[]), SqlDbType.VarBinary)]
    [InlineData(typeof(DateTime), SqlDbType.DateTime2)]
    [InlineData(typeof(DateOnly), SqlDbType.Date)]
    [InlineData(typeof(TimeOnly), SqlDbType.Time)]
    [InlineData(typeof(DateTimeOffset), SqlDbType.DateTimeOffset)]
    [InlineData(typeof(Guid), SqlDbType.UniqueIdentifier)]
    public void GetSqlDbType_Primitives(Type type, SqlDbType expected) =>
        Assert.Equal(expected, SqlTypeCache.GetSqlDbType(type));

    [Theory]
    [InlineData(typeof(int?), SqlDbType.Int)]
    [InlineData(typeof(double?), SqlDbType.Float)]
    [InlineData(typeof(float?), SqlDbType.Real)]
    [InlineData(typeof(decimal?), SqlDbType.Decimal)]
    [InlineData(typeof(DateTime?), SqlDbType.DateTime2)]
    [InlineData(typeof(DateOnly?), SqlDbType.Date)]
    [InlineData(typeof(TimeOnly?), SqlDbType.Time)]
    [InlineData(typeof(DateTimeOffset?), SqlDbType.DateTimeOffset)]
    [InlineData(typeof(Guid?), SqlDbType.UniqueIdentifier)]
    public void GetSqlDbType_Nullables(Type type, SqlDbType expected) =>
        Assert.Equal(expected, SqlTypeCache.GetSqlDbType(type));

    [Theory]
    [InlineData(typeof(SortDirectionEnum))]
    [InlineData(typeof(SortDirectionEnum?))]
    public void GetSqlDbType_Enums(Type type) =>
        Assert.Equal(SqlDbType.Int, SqlTypeCache.GetSqlDbType(type));

    [Theory]
    [InlineData(typeof(StandardEntity))]
    [InlineData(typeof(TimeSpan))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(ushort))]
    public void GetSqlDbType_Unmapped(Type type) =>
        Assert.Equal(SqlDbType.Variant, SqlTypeCache.GetSqlDbType(type));

    [Fact]
    public void GetSqlDbTypeFromValue_Null() =>
        Assert.Equal(SqlDbType.Variant, SqlTypeCache.GetSqlDbTypeFromValue(null));

    [Fact]
    public void GetSqlDbTypeFromValue_ByteArray()
    {
        byte[] data = new byte[1];
        Assert.Equal(SqlDbType.VarBinary, SqlTypeCache.GetSqlDbTypeFromValue(data));
    }

    [Theory]
    [InlineData(123, SqlDbType.Int)]
    [InlineData(123L, SqlDbType.BigInt)]
    [InlineData((short)5, SqlDbType.SmallInt)]
    [InlineData((byte)7, SqlDbType.TinyInt)]
    [InlineData(true, SqlDbType.Bit)]
    public void GetSqlDbTypeFromValue_SimpleScalars(object value, SqlDbType expected) =>
        Assert.Equal(expected, SqlTypeCache.GetSqlDbTypeFromValue(value));

    [Fact]
    public void GetSqlDbTypeFromValue_DateTime()
    {
        DateTime dateTime = DateTime.UtcNow;
        Assert.Equal(SqlDbType.DateTime2, SqlTypeCache.GetSqlDbTypeFromValue(dateTime));
    }

    [Fact]
    public void GetSqlDbTypeFromValue_DateOnly()
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
        Assert.Equal(SqlDbType.Date, SqlTypeCache.GetSqlDbTypeFromValue(date));
    }

    [Fact]
    public void GetSqlDbTypeFromValue_TimeOnly()
    {
        TimeOnly time = TimeOnly.FromDateTime(DateTime.UtcNow);
        Assert.Equal(SqlDbType.Time, SqlTypeCache.GetSqlDbTypeFromValue(time));
    }

    [Fact]
    public void GetSqlDbTypeFromValue_DateTimeOffset()
    {
        DateTimeOffset value = DateTimeOffset.Now;
        Assert.Equal(SqlDbType.DateTimeOffset, SqlTypeCache.GetSqlDbTypeFromValue(value));
    }

    [Fact]
    public void GetSqlDbTypeFromValue_Enum_ReturnsInt()
    {
        SortDirectionEnum value = SortDirectionEnum.Ascending;
        Assert.Equal(SqlDbType.Int, SqlTypeCache.GetSqlDbTypeFromValue(value));
    }
}
