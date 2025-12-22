using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;
//IGNORE SPELLING:
public class SqlXmlTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }

    [Fact]
    public void XDocument_Simple()
    {
        XDocument xmlDocument = new(new XElement("root", new XElement("name", "Carrigan")));
        Dictionary<string, object?> data = GetTestData(xmlDocument);
        XDocumentTest actual = Invoker<XDocumentTest>.Invoke(data);
        Assert.Equal("Carrigan", actual.Value.Root?.Element("name")?.Value);
    }

    [Fact]
    public void XmlDocument_Simple()
    {
        XmlDocument x = new();
        x.LoadXml("<root><name>Carrigan</name></root>");
        Dictionary<string, object?> data = GetTestData(x);
        XmlDocumentTest actual = Invoker<XmlDocumentTest>.Invoke(data);
        XmlNode? node = actual.Value.SelectSingleNode("/root/name");
        Assert.NotNull(node);
        Assert.Equal("Carrigan", node!.InnerText);
    }

    private static SqlXml CreateSqlXml(string xml)
    {
        using StringReader stringReader = new(xml);
        using XmlReader xmlReader = XmlReader.Create(stringReader);
        return new SqlXml(xmlReader);
    }

    [Fact]
    public void XDocument_FromSqlXml_Simple()
    {
        SqlXml sqlXml = CreateSqlXml("<root><name>Carrigan</name></root>");
        Dictionary<string, object?> data = GetTestData(sqlXml);

        XDocumentTest actual = Invoker<XDocumentTest>.Invoke(data);

        Assert.Equal("Carrigan", actual.Value.Root?.Element("name")?.Value);
    }

    [Fact]
    public void XmlDocument_FromSqlXml_Simple()
    {
        SqlXml sqlXml = CreateSqlXml("<root><name>Carrigan</name></root>");
        Dictionary<string, object?> data = GetTestData(sqlXml);

        XmlDocumentTest actual = Invoker<XmlDocumentTest>.Invoke(data);

        XmlNode? node = actual.Value.SelectSingleNode("/root/name");
        Assert.NotNull(node);
        Assert.Equal("Carrigan", node!.InnerText);
    }
}
