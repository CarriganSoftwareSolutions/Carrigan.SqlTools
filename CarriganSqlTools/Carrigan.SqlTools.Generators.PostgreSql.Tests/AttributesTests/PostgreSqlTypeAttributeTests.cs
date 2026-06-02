using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
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

    private sealed class TestModelWithArrayAttributes
    {
        [PostgreSqlChar(StorageTypeEnum.Fixed, 10)]
        public char[] FixedCharacters { get; set; } = [];

        [PostgreSqlChar(StorageTypeEnum.Var, 50)]
        public string[] VariableCharacters { get; set; } = [];

        [PostgreSqlDate]
        public DateOnly[] Dates { get; set; } = [];

        [PostgreSqlFloat(24)]
        public float[] Floats { get; set; } = [];

        [PostgreSqlMoney]
        public decimal[] MoneyValues { get; set; } = [];

        [PostgreSqlNumeric(18, 2)]
        public decimal[] NumericValues { get; set; } = [];

        [PostgreSqlTime(6)]
        public TimeOnly[] Times { get; set; } = [];

        [PostgreSqlTimestamp(6)]
        public DateTime[] Timestamps { get; set; } = [];

        [PostgreSqlTimestampUtc(6)]
        public DateTimeOffset[] TimestampUtcValues { get; set; } = [];
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

    [Theory]
    [InlineData(nameof(TestModelWithArrayAttributes.FixedCharacters), "CHAR(10)[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.VariableCharacters), "VARCHAR(50)[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.Dates), "DATE[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.Floats), "FLOAT(24)[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.MoneyValues), "MONEY[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.NumericValues), "NUMERIC(18, 2)[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.Times), "TIME(6) WITHOUT TIME ZONE[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.Timestamps), "TIMESTAMP(6) WITHOUT TIME ZONE[] NOT NULL")]
    [InlineData(nameof(TestModelWithArrayAttributes.TimestampUtcValues), "TIMESTAMP(6) WITH TIME ZONE[] NOT NULL")]
    public void ColumnInfo_WhenAttributeIsAppliedToArrayProperty_SetsFieldPropertiesIsArray(string propertyName, string expectedDeclaration)
    {
        PropertyInfo propertyInfo = typeof(TestModelWithArrayAttributes).GetProperty(propertyName)!;
        ColumnInfo columnInfo = new(null, new TableName(nameof(TestModelWithArrayAttributes)), propertyInfo, []);
        PostgreSqlDialect dialect = new();

        Assert.NotNull(columnInfo.FieldProperties);
        FieldProperties fieldProperties = columnInfo.FieldProperties!;

        Assert.Equal((bool?)true, fieldProperties.IsArray);
        Assert.Equal(expectedDeclaration, dialect.RenderFieldProperties(fieldProperties));
    }

    [Fact]
    public void GetSqlTypeAttribute_WhenPropertyInfoIsNull_Exception() =>
        Assert.Throws<ArgumentNullException>(() => SqlTypeAttribute.GetSqlTypeAttribute(null!));
}
