//Ignore Spelling: PostgreSql, XDocument, XmlDocument

using Carrigan.SqlTools.IntegrationTests.Models;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class ArrayFieldsDataSet
{
    private static readonly Guid MaxGuid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
    private static readonly TimeOnly MaxTimeOnly = new (23, 59, 59, 999, 999); // not using TimeOnly.MaxValue, do to precision differences
    private static readonly DateTime MaxTimestamp = DateTime.MaxValue;
    private static readonly DateTimeOffset MinTimestampWithTimeZone = new (0001, 01, 02, 00, 00, 00, TimeSpan.Zero);
    private static readonly DateTimeOffset MaxTimestampWithTimeZone = new (9999, 12, 30, 00, 00, 00, TimeSpan.Zero);
    private static readonly TimeSpan MinTimeSpan = TimeSpan.FromTicks((TimeSpan.MinValue.Ticks / 10) * 10);
    private static readonly TimeSpan MaxTimeSpan = TimeSpan.FromTicks((TimeSpan.MaxValue.Ticks / 10) * 10);

    public static TheoryData<ArrayFieldsModel> EmptyArrays => new([CreateEmptyArraysModel()]);
    public static TheoryData<ArrayFieldsModel> NullMinMaxArrays => new([CreateNullMinMaxArraysModel()]);

    private static ArrayFieldsModel CreateEmptyArraysModel() =>
        new()
        {
            GuidArrayValue = [],
            GuidNullableArrayValue = [],
            StringArrayValue = [],
            CharArrayValue = [],
            CharNullableArrayValue = [],
            BytesArrayValue = [],
            BoolArrayValue = [],
            BoolNullableArrayValue = [],
            ByteNullableArrayValue = [],
            SByteArrayValue = [],
            SByteNullableArrayValue = [],
            ShortArrayValue = [],
            ShortNullableArrayValue = [],
            UShortArrayValue = [],
            UShortNullableArrayValue = [],
            IntArrayValue = [],
            IntNullableArrayValue = [],
            UIntArrayValue = [],
            UIntNullableArrayValue = [],
            LongArrayValue = [],
            LongNullableArrayValue = [],
            ULongArrayValue = [],
            ULongNullableArrayValue = [],
            FloatArrayValue = [],
            FloatNullableArrayValue = [],
            DoubleArrayValue = [],
            DoubleNullableArrayValue = [],
            DecimalArrayValue = [],
            DecimalNullableArrayValue = [],
            DateOnlyArrayValue = [],
            DateOnlyNullableArrayValue = [],
            TimeOnlyArrayValue = [],
            TimeOnlyNullableArrayValue = [],
            DateTimeArrayValue = [],
            DateTimeNullableArrayValue = [],
            TimeSpanArrayValue = [],
            TimeSpanNullableArrayValue = [],
            DateTimeOffsetArrayValue = [],
            DateTimeOffsetNullableArrayValue = [],
            XDocumentArrayValue = [],
            XmlDocumentArrayValue = [],
            ObjectArrayValue = []
        };

    private static ArrayFieldsModel CreateNullMinMaxArraysModel() =>
        new()
        {
            GuidArrayValue = [Guid.Empty, MaxGuid],
            GuidNullableArrayValue = [null, Guid.Empty, MaxGuid],
            StringArrayValue = [null, string.Empty, "lorem ipsum"],
            CharArrayValue = ['\u0001', char.MaxValue],
            CharNullableArrayValue = [null, '\u0001', char.MaxValue],
            BytesArrayValue = [null, [], [0x00, 0xFF]],
            BoolArrayValue = [false, true],
            BoolNullableArrayValue = [null, false, true],
            ByteNullableArrayValue = [null, byte.MinValue, byte.MaxValue],
            SByteArrayValue = [sbyte.MinValue, sbyte.MaxValue],
            SByteNullableArrayValue = [null, sbyte.MinValue, sbyte.MaxValue],
            ShortArrayValue = [short.MinValue, short.MaxValue],
            ShortNullableArrayValue = [null, short.MinValue, short.MaxValue],
            UShortArrayValue = [ushort.MinValue, ushort.MaxValue],
            UShortNullableArrayValue = [null, ushort.MinValue, ushort.MaxValue],
            IntArrayValue = [int.MinValue, int.MaxValue],
            IntNullableArrayValue = [null, int.MinValue, int.MaxValue],
            UIntArrayValue = [uint.MinValue, uint.MaxValue],
            UIntNullableArrayValue = [null, uint.MinValue, uint.MaxValue],
            LongArrayValue = [long.MinValue, long.MaxValue],
            LongNullableArrayValue = [null, long.MinValue, long.MaxValue],
            ULongArrayValue = [ulong.MinValue, ulong.MaxValue],
            ULongNullableArrayValue = [null, ulong.MinValue, ulong.MaxValue],
            FloatArrayValue = [float.MinValue, float.MaxValue],
            FloatNullableArrayValue = [null, float.MinValue, float.MaxValue],
            DoubleArrayValue = [double.MinValue, double.MaxValue],
            DoubleNullableArrayValue = [null, double.MinValue, double.MaxValue],
            DecimalArrayValue = [decimal.MinValue, decimal.MaxValue],
            DecimalNullableArrayValue = [null, decimal.MinValue, decimal.MaxValue],
            DateOnlyArrayValue = [DateOnly.MinValue, DateOnly.MaxValue],
            DateOnlyNullableArrayValue = [null, DateOnly.MinValue, DateOnly.MaxValue],
            TimeOnlyArrayValue = [TimeOnly.MinValue, MaxTimeOnly],
            TimeOnlyNullableArrayValue = [null, TimeOnly.MinValue, MaxTimeOnly],
            DateTimeArrayValue = [DateTime.MinValue, MaxTimestamp],
            DateTimeNullableArrayValue = [null, DateTime.MinValue, MaxTimestamp],
            TimeSpanArrayValue = [MinTimeSpan, MaxTimeSpan],
            TimeSpanNullableArrayValue = [null, MinTimeSpan, MaxTimeSpan],
            DateTimeOffsetArrayValue = [MinTimestampWithTimeZone, MaxTimestampWithTimeZone],
            DateTimeOffsetNullableArrayValue = [null, MinTimestampWithTimeZone, MaxTimestampWithTimeZone],
            XDocumentArrayValue = [null, CreateXDocument("min"), CreateXDocument("max")],
            XmlDocumentArrayValue = [null, CreateXmlDocument("min"), CreateXmlDocument("max")],
            ObjectArrayValue = new string?[] { null, string.Empty, "lorem ipsum" }
        };

    private static XDocument CreateXDocument(string value) => new(new XElement("root", new XElement("value", value)));

    private static XmlDocument CreateXmlDocument(string value)
    {
        XmlDocument xmlDocument = new();
        xmlDocument.LoadXml($"<root><value>{value}</value></root>");
        return xmlDocument;
    }
}
