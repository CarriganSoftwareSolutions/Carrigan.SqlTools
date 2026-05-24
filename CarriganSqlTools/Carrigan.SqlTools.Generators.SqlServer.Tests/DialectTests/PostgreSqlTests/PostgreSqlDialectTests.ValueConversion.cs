using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void ValueConversion_Null_ReturnsDbNull()
    {
        object actual = Dialect.ValueConversion(null);
        Assert.Equal(DBNull.Value, actual);
    }

    [Fact]
    public void ValueConversion_XDocument_ReturnsXmlString()
    {
        XDocument value = new(new XElement("Root", new XElement("Value", "Test")));
        object actual = Dialect.ValueConversion(value);
        Assert.Equal(value.ToString(), actual);
    }

    [Fact]
    public void ValueConversion_XmlDocument_ReturnsOuterXml()
    {
        XmlDocument value = new();
        value.LoadXml("<Root><Value>Test</Value></Root>");
        object actual = Dialect.ValueConversion(value);
        Assert.Equal(value.OuterXml, actual);
    }

    [Fact]
    public void ValueConversion_OtherValue_ReturnsOriginalValue()
    {
        object value = 123;
        object actual = Dialect.ValueConversion(value);
        Assert.Same(value, actual);
    }
}
