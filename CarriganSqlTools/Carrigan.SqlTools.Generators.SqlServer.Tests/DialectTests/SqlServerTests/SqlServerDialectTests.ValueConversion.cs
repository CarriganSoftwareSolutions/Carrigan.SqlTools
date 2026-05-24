using Carrigan.SqlTools.Dialects.SqlServer;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void ValueConversion_Null_ReturnsDBNullValue()
    {
        SqlServerDialect dialect = new();

        object actual = dialect.ValueConversion(null);

        Assert.Same(DBNull.Value, actual);
    }

    [Fact]
    public void ValueConversion_XDocument_ReturnsXmlString()
    {
        SqlServerDialect dialect = new();
        XDocument document = new(new XElement("root", new XElement("value", "test")));

        object actual = dialect.ValueConversion(document);

        Assert.IsType<string>(actual);
        Assert.Contains("<root>", actual.ToString());
        Assert.Contains("<value>test</value>", actual.ToString());
    }

    [Fact]
    public void ValueConversion_XmlDocument_ReturnsOuterXml()
    {
        SqlServerDialect dialect = new();
        XmlDocument document = new();
        document.LoadXml("<root><value>test</value></root>");

        object actual = dialect.ValueConversion(document);

        Assert.Equal("<root><value>test</value></root>", actual);
    }

    [Fact]
    public void ValueConversion_NonXmlValue_ReturnsOriginalValue()
    {
        SqlServerDialect dialect = new();
        Guid value = Guid.NewGuid();

        object actual = dialect.ValueConversion(value);

        Assert.Equal(value, actual);
    }
}