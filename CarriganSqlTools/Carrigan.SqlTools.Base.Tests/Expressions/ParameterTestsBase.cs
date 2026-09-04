using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Reflection;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class ParameterTestsBase<modelT> where modelT : class
{
    protected abstract ISqlDialects Dialect { get; }

    internal abstract Dictionary<string, ParameterTag> ExpectedPropertyParameterTag { get; }

    protected virtual IEnumerable<string> NotMappedProperties => [];

    protected KeyValuePair<string, ParameterTag> NewKvp(string propertyName, string parameterName) =>
        new(propertyName, new ParameterTag(parameterName));

    protected KeyValuePair<string, ParameterTag> NewKvp(string propertyName) =>
        new(propertyName, new ParameterTag(propertyName));

    protected static object GetValue(string propertyName)
    {
        PropertyInfo propertyInfo = typeof(modelT).GetProperty(propertyName) ?? throw new InvalidOperationException($"{propertyName} is not a property on {typeof(modelT).Name}.");
        Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

        if (type == typeof(short)) return (short)42;
        if (type == typeof(int)) return 42;
        if (type == typeof(long)) return 42L;
        if (type == typeof(byte)) return (byte)42;
        if (type == typeof(float)) return 3.141f;
        if (type == typeof(double)) return 1.618d;
        if (type == typeof(decimal)) return 2.71828m;
        if (type == typeof(bool)) return true;
        if (type == typeof(string)) return "Test Value";
        if (type == typeof(DateTime)) return new DateTime(2024, 11, 6, 1, 14, 1, 2, 3);
        if (type == typeof(Guid)) return new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695");
        if (type == typeof(char)) return 'A';
        if (type == typeof(TimeOnly)) return new TimeOnly(1, 2, 3);
        if (type == typeof(DateOnly)) return new DateOnly(1776, 7, 4);
        if (type == typeof(byte[])) return new byte[] { 0x01, 0x02, 0x03 };
        if (type == typeof(DateTimeOffset)) return new DateTimeOffset(2024, 11, 6, 1, 14, 1, TimeSpan.Zero);

        throw new NotSupportedException($"Type '{propertyInfo.PropertyType.FullName}' is not supported by the parameter model tests.");
    }

    protected FieldProperties GetExpectedFieldProperties(string propertyName)
    {
        PropertyInfo propertyInfo = typeof(modelT).GetProperty(propertyName) ?? throw new InvalidOperationException($"{propertyName} is not a property on {typeof(modelT).Name}.");
        SqlTypeAttribute? sqlTypeAttribute = SqlTypeAttribute.GetSqlTypeAttribute(propertyInfo);
        return sqlTypeAttribute?.FieldProperties ?? Dialect.GetDefaultFieldPropertiesByClrType(propertyInfo.PropertyType);
    }

    protected static void AssertFieldProperties(FieldProperties expected, FieldProperties? actual)
    {
        FieldProperties actualFieldProperties = Assert.IsType<FieldProperties>(actual);
        Assert.Equal(expected.Length, actualFieldProperties.Length);
        Assert.Equal(expected.IsMax, actualFieldProperties.IsMax);
        Assert.Equal(expected.IsUnicode, actualFieldProperties.IsUnicode);
        Assert.Equal(expected.IsFixedLength, actualFieldProperties.IsFixedLength);
        Assert.Equal(expected.Precision, actualFieldProperties.Precision);
        Assert.Equal(expected.Scale, actualFieldProperties.Scale);
        Assert.Equal(expected.FractionalSecondsPrecision, actualFieldProperties.FractionalSecondsPrecision);
        Assert.Equal(expected.IsNullable, actualFieldProperties.IsNullable);
        Assert.Equal(expected.ProviderTypeName, actualFieldProperties.ProviderTypeName);
        Assert.Equal(expected.BaseType, actualFieldProperties.BaseType);
        Assert.Equal(expected.IsArray, actualFieldProperties.IsArray);
    }

    protected abstract void ValidateSqlFragment(string propertyName);

    protected abstract void ValidateExpectedPropertyParameterTag(string propertyName);

    protected abstract void ValidateFieldProperties(string propertyName);

    protected abstract void ValidateValue(string propertyName);

    protected abstract void ValidateNoDescendantParameters(string propertyName);

    protected abstract void ValidateNoDescendantColumns(string propertyName);

    protected abstract void ValidateNotMapped();

    protected void RunValidationMethod(Action<string> action)
    {
        foreach (string propertyName in ExpectedPropertyParameterTag.Keys)
            action(propertyName);
    }

    [Fact]
    public void Run_ValidateSqlFragment() =>
        RunValidationMethod(ValidateSqlFragment);

    [Fact]
    public void Run_ValidateExpectedPropertyParameterTag() =>
        RunValidationMethod(ValidateExpectedPropertyParameterTag);

    [Fact]
    public void Run_ValidateFieldProperties() =>
        RunValidationMethod(ValidateFieldProperties);

    [Fact]
    public void Run_ValidateValue() =>
        RunValidationMethod(ValidateValue);

    [Fact]
    public void Run_ValidateNoDescendantParameters() =>
        RunValidationMethod(ValidateNoDescendantParameters);

    [Fact]
    public void Run_ValidateNoDescendantColumns() =>
        RunValidationMethod(ValidateNoDescendantColumns);

    [Fact]
    public void Run_ValidateNotMapped() =>
        ValidateNotMapped();
}
