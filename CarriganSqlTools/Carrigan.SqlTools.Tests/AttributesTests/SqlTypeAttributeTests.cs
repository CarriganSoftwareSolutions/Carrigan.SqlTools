using System.Reflection;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlTypeAttributeTests
{
    private sealed class TestModelWithAttribute
    {
        [SqlVarBinaryMax] 
        public byte[] Data { get; set; } = [];
    }

    private sealed class TestModelWithoutAttribute
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenAttributeIsPresent()
    {
        PropertyInfo propertyInfo = typeof(TestModelWithAttribute)
            .GetProperty(nameof(TestModelWithAttribute.Data))!;

        SqlTypeAttribute? attribute = SqlTypeAttribute.GetSqlTypeAttribute(propertyInfo);

        Assert.NotNull(attribute);
        Assert.IsType<SqlVarBinaryMaxAttribute>(attribute);
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenAttributeIsNotPresent()
    {
        PropertyInfo propertyInfo = typeof(TestModelWithoutAttribute)
            .GetProperty(nameof(TestModelWithoutAttribute.Name))!;

        SqlTypeAttribute? attribute = SqlTypeAttribute.GetSqlTypeAttribute(propertyInfo);

        Assert.Null(attribute);
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenPropertyInfoIsNull_Exception() =>
    Assert.Throws<ArgumentNullException>(() => SqlTypeAttribute.GetSqlTypeAttribute(null!));
}
