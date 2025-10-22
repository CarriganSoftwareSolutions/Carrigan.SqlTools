    using System.Xml.Linq;

namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

    internal class XDocumentTest
    {
        public XDocument Value { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public XDocumentTest() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    internal XDocumentTest(XDocument value) => Value = value;
    }
