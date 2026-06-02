using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;
using System.Reflection;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlTypeAttributeTests
{
    private sealed class TestModelWithAttribute
    {
        [PostgreSqlChar(StorageTypeEnum.Var, 255)]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestModelWithoutAttribute
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenAttributeIsPresent()
    {
        PropertyInfo propertyInfo = typeof(TestModelWithAttribute).GetProperty(nameof(TestModelWithAttribute.Name))!;

        SqlTypeAttribute? attribute = SqlTypeAttribute.GetSqlTypeAttribute(propertyInfo);

        Assert.NotNull(attribute);
        Assert.IsType<PostgreSqlCharAttribute>(attribute);
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenAttributeIsNotPresent()
    {
        PropertyInfo propertyInfo = typeof(TestModelWithoutAttribute).GetProperty(nameof(TestModelWithoutAttribute.Name))!;

        SqlTypeAttribute? attribute = SqlTypeAttribute.GetSqlTypeAttribute(propertyInfo);

        Assert.Null(attribute);
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenPropertyInfoIsNull_Exception() =>
        Assert.Throws<ArgumentNullException>(() => SqlTypeAttribute.GetSqlTypeAttribute(null!));
}
