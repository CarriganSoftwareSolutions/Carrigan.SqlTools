using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

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
        System.Xml.Linq.XDocument xmlDocument = new(new System.Xml.Linq.XElement("root", new System.Xml.Linq.XElement("name", "Carrigan")));
        Dictionary<string, object?> data = GetTestData(xmlDocument);
        XDocumentTest actual = Invoker<XDocumentTest>.Invoke(data);
        Assert.Equal("Carrigan", actual.Value.Root?.Element("name")?.Value);
    }

    [Fact]
    public void XmlDocument_Simple()
    {
        System.Xml.XmlDocument x = new();
        x.LoadXml("<root><name>Carrigan</name></root>");
        Dictionary<string, object?> data = GetTestData(x);
        XmlDocumentTest actual = Invoker<XmlDocumentTest>.Invoke(data);
        System.Xml.XmlNode? node = actual.Value.SelectSingleNode("/root/name");
        Assert.NotNull(node);
        Assert.Equal("Carrigan", node!.InnerText);
    }
}
