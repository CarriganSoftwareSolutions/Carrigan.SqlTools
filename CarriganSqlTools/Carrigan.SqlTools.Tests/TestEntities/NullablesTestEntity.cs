
using System.ComponentModel.DataAnnotations;

namespace SqlToolsTests.TestEntities;

public class NullablesTestEntity
{
    public static readonly DateTimeOffset DateTimeOffsetTestValue;

    static NullablesTestEntity()
    {
        DateTimeOffsetTestValue = DateTimeOffset.Now;
    }

    [Key]
    public Guid Key { get; set; }                   //key
    public int? IntValue { get; set; }              // Nullable int
    public long? LongValue { get; set; }            // Nullable long
    public short? ShortValue { get; set; }          // Nullable short
    public byte? ByteValue { get; set; }            // Nullable byte
    public bool? BoolValue { get; set; }            // Nullable bool
    public decimal? DecimalValue { get; set; }      // Nullable decimal
    public float? FloatValue { get; set; }          // Nullable float
    public double? DoubleValue { get; set; }        // Nullable double
    public DateTime? DateTimeValue { get; set; }    // Nullable DateTime
    public Guid? GuidValue { get; set; }            // Nullable Guid
    public char? CharValue { get; set; }            // Nullable char
    public TimeOnly? TimeOnlyValue { get; set; }    // Nullable Time
    public DateOnly? DateOnlyValue { get; set; }    // Nullable Date
    public byte[]? ByteArrayValue { get; set; }    // Nullable Byte Array
    public DateTimeOffset? DateTimeOffsetValue { get; set; }    // Nullable DateTimeOffset

    public static NullablesTestEntity GetStandardTestSet() => new()
    {
        Key = Guid.Empty,                                               // Key
        IntValue = (int?)1,                                             // Nullable int
        LongValue = (long?)123456789L,                                  // Nullable long
        ShortValue = (short?)123,                                       // Nullable short
        ByteValue = (byte?)255,                                         // Nullable byte
        BoolValue = (bool?)true,                                        // Nullable bool
        DecimalValue = (decimal?)99.99m,                                // Nullable decimal
        FloatValue = (float?)3.14f,                                     // Nullable FloatValue
        DoubleValue = (double?)1.618,                                   // Nullable DoubleValue
        DateTimeValue = new DateTime(2024, 11, 6, 1, 14, 1, 2, 3),      // Nullable DateTimeValue
        GuidValue = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),   // Nullable GuidValue
        CharValue = (char?)'A',                                         // Nullable CharValue
        TimeOnlyValue = new TimeOnly(1, 2, 0),                          // Nullable TimeOnlyValue
        DateOnlyValue = new DateOnly(1, 12, 25),                        // Nullable DateOnlyValue
        ByteArrayValue = [0x01, 0x02, 0x03],                            // Nullable ByteArrayValue
        DateTimeOffsetValue = DateTimeOffsetTestValue                   // Nullable DateTimeOffsetValue
    };

    public static NullablesTestEntity GetNullTestSet() => new()
    {
            Key = Guid.Empty,               // Key
            IntValue = null,                // Nullable int
            LongValue = null,               // Nullable long
            ShortValue = null,              // Nullable short
            ByteValue = null,               // Nullable byte
            BoolValue = null,               // Nullable bool
            DecimalValue = null,            // Nullable decimal
            FloatValue = null,              // Nullable FloatValue
            DoubleValue = null,             // Nullable DoubleValue
            DateTimeValue = null,           // Nullable DateTimeValue
            GuidValue = null,               // Nullable GuidValue
            CharValue = null,               // Nullable CharValue
            TimeOnlyValue = null,           // Nullable TimeOnlyValue
            DateOnlyValue = null,           // Nullable DateOnlyValue
            ByteArrayValue = null,          // Nullable ByteArrayValue
            DateTimeOffsetValue = null      // Nullable ByteArrayValue
        };
}
