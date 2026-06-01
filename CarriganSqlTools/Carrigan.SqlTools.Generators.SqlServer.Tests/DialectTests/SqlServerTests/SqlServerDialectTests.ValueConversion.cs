using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void ValueConversion_Null_ReturnsDBNullValue()
    {
        object actual = Dialect.ValueConversion(null);

        Assert.Same(DBNull.Value, actual);
    }

    [Fact]
    public void ValueConversion_XDocument_ReturnsXmlString()
    {
        XDocument document = new(new XElement("root", new XElement("value", "test")));

        object actual = Dialect.ValueConversion(document);

        Assert.IsType<string>(actual);
        Assert.Contains("<root>", actual.ToString());
        Assert.Contains("<value>test</value>", actual.ToString());
    }

    [Fact]
    public void ValueConversion_XmlDocument_ReturnsOuterXml()
    {
        XmlDocument document = new();
        document.LoadXml("<root><value>test</value></root>");

        object actual = Dialect.ValueConversion(document);

        Assert.Equal("<root><value>test</value></root>", actual);
    }

    [Fact]
    public void ValueConversion_NonXmlValue_ReturnsOriginalValue()
    {
        Guid value = Guid.NewGuid();

        object actual = Dialect.ValueConversion(value);

        Assert.Equal(value, actual);
    }
}