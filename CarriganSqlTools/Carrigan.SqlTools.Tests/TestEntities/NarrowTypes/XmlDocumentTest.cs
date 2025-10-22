    using System.Xml;

namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

    internal class XmlDocumentTest
    {
        public XmlDocument Value { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public XmlDocumentTest() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    internal XmlDocumentTest(XmlDocument value) => Value = value;
    }
