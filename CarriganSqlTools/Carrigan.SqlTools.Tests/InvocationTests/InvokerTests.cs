using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.InvocationTests;

public class InvokerTests
{
    private static Guid guid = new("bf08ee23-82af-4640-8e21-3de23bbc2a51");
    private static Dictionary<string, object?> StandardInvocation => new        (
            [
                new("IntValue", int.MaxValue),
                new("LongValue", long.MaxValue),
                new("ShortValue", short.MaxValue),
                new("ByteValue", byte.MaxValue),
                new("BoolValue", true),
                new("DecimalValue", decimal.MaxValue),
                new("FloatValue", float.E),
                new("DoubleValue", double.Pi),
                new("StringValue", "Hello World!"),
                new("DateTimeValue", new DateTime(1969,7, 20, 20,17, 0)),
                new("GuidValue", guid),
                new("ByteArrayValue", new byte[] {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x20, 0x30, 0x40, 0x50,0x60, 0x70, 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0, 0xF0}),
                new("CharValue", char.MaxValue),
                new("TimeOnlyValue", new TimeOnly(20,17, 0)),
                new("DateOnlyValue", new DateOnly(1985, 3, 10))
            ]
        );
    private static Dictionary<string, object?> NullInvocation => new        (
            [
                new("Key", guid),
                new("IntValue", null),
                new("LongValue", null),
                new("ShortValue", null),
                new("ByteValue", null),
                new("BoolValue", null),
                new("DecimalValue", null),
                new("FloatValue", null),
                new("DoubleValue", null),
                new("DateTimeValue", null),
                new("GuidValue", null),
                new("CharValue", null),
                new("TimeOnlyValue", null),
                new("DateOnlyValue", null),
                new("ByteArrayValue", null)
            ]
        );
    private static Dictionary<string, object?> DbNullInvocation => new        (
            [
                new("Key", guid),
                new("IntValue", DBNull.Value),
                new("LongValue", DBNull.Value),
                new("ShortValue", DBNull.Value),
                new("ByteValue", DBNull.Value),
                new("BoolValue", DBNull.Value),
                new("DecimalValue", DBNull.Value),
                new("FloatValue", DBNull.Value),
                new("DoubleValue", DBNull.Value),
                new("DateTimeValue", DBNull.Value),
                new("GuidValue", DBNull.Value),
                new("CharValue", DBNull.Value),
                new("TimeOnlyValue", DBNull.Value),
                new("DateOnlyValue", DBNull.Value),
                new("ByteArrayValue", DBNull.Value)
            ]
        );
    private static Dictionary<string, object?> StandardNullableInvocation => new        (
            [
                new("Key", guid),
                new("IntValue", int.MaxValue),
                new("LongValue", long.MaxValue),
                new("ShortValue", short.MaxValue),
                new("ByteValue", byte.MaxValue),
                new("BoolValue", true),
                new("DecimalValue", decimal.MaxValue),
                new("FloatValue", float.E),
                new("DoubleValue", double.Pi),
                new("DateTimeValue", new DateTime(1969,7, 20, 20,17, 0)),
                new("GuidValue", guid),
                new("CharValue", char.MaxValue),
                new("TimeOnlyValue", new TimeOnly(20,17, 0)),
                new("DateOnlyValue", new DateOnly(1985, 3, 10)),
                new("ByteArrayValue", Array.Empty<byte>())
            ]
        );
    [Fact]
    public void Standard_Invocation_Test()
    {
        Dictionary<string, object?> expectedValues = StandardInvocation;
        StandardEntity entity = Invoker<StandardEntity>.Invoke(expectedValues);


        Assert.Equal(expectedValues["BoolValue"], entity.BoolValue);
        Assert.Equal(expectedValues["ByteArrayValue"], entity.ByteArrayValue);
        Assert.Equal(expectedValues["ByteValue"], entity.ByteValue);
        Assert.Equal(expectedValues["CharValue"], entity.CharValue);
        Assert.Equal(expectedValues["DateOnlyValue"], entity.DateOnlyValue);
        Assert.Equal(expectedValues["DateTimeValue"], entity.DateTimeValue);
        Assert.Equal(expectedValues["DecimalValue"], entity.DecimalValue);
        Assert.Equal(expectedValues["DoubleValue"], entity.DoubleValue);
        Assert.Equal(expectedValues["FloatValue"], entity.FloatValue);
        Assert.Equal(expectedValues["GuidValue"], entity.GuidValue);
        Assert.Equal(expectedValues["IntValue"], entity.IntValue);
        Assert.Equal(expectedValues["LongValue"], entity.LongValue);
        Assert.Equal(expectedValues["ShortValue"], entity.ShortValue);
        Assert.Equal(expectedValues["StringValue"], entity.StringValue);
        Assert.Equal(expectedValues["TimeOnlyValue"], entity.TimeOnlyValue);
    }
    [Fact]
    public void Null_Test()
    {
        Dictionary<string, object?> expectedValues = NullInvocation;
        NullableTestEntity entity = Invoker<NullableTestEntity>.Invoke(expectedValues);


        Assert.Equal(expectedValues["Key"], entity.Key);
        Assert.Equal(expectedValues["BoolValue"], entity.BoolValue);
        Assert.Equal(expectedValues["ByteArrayValue"], entity.ByteArrayValue);
        Assert.Equal(expectedValues["ByteValue"], entity.ByteValue);
        Assert.Equal(expectedValues["CharValue"], entity.CharValue);
        Assert.Equal(expectedValues["DateOnlyValue"], entity.DateOnlyValue);
        Assert.Equal(expectedValues["DateTimeValue"], entity.DateTimeValue);
        Assert.Equal(expectedValues["DecimalValue"], entity.DecimalValue);
        Assert.Equal(expectedValues["DoubleValue"], entity.DoubleValue);
        Assert.Equal(expectedValues["FloatValue"], entity.FloatValue);
        Assert.Equal(expectedValues["GuidValue"], entity.GuidValue);
        Assert.Equal(expectedValues["IntValue"], entity.IntValue);
        Assert.Equal(expectedValues["LongValue"], entity.LongValue);
        Assert.Equal(expectedValues["ShortValue"], entity.ShortValue);
        Assert.Equal(expectedValues["TimeOnlyValue"], entity.TimeOnlyValue);
    }
    [Fact]
    public void Standard_Nullable_Test()
    {
        Dictionary<string, object?> expectedValues = StandardNullableInvocation;
        NullableTestEntity entity = Invoker<NullableTestEntity>.Invoke(expectedValues);


        Assert.Equal(expectedValues["Key"], entity.Key);
        Assert.Equal(expectedValues["BoolValue"], entity.BoolValue);
        Assert.Equal(expectedValues["ByteArrayValue"], entity.ByteArrayValue);
        Assert.Equal(expectedValues["ByteValue"], entity.ByteValue);
        Assert.Equal(expectedValues["CharValue"], entity.CharValue);
        Assert.Equal(expectedValues["DateOnlyValue"], entity.DateOnlyValue);
        Assert.Equal(expectedValues["DateTimeValue"], entity.DateTimeValue);
        Assert.Equal(expectedValues["DecimalValue"], entity.DecimalValue);
        Assert.Equal(expectedValues["DoubleValue"], entity.DoubleValue);
        Assert.Equal(expectedValues["FloatValue"], entity.FloatValue);
        Assert.Equal(expectedValues["GuidValue"], entity.GuidValue);
        Assert.Equal(expectedValues["IntValue"], entity.IntValue);
        Assert.Equal(expectedValues["LongValue"], entity.LongValue);
        Assert.Equal(expectedValues["ShortValue"], entity.ShortValue);
        Assert.Equal(expectedValues["TimeOnlyValue"], entity.TimeOnlyValue);
    }
    [Fact]
    public void DB_Null_Test()
    {
        Dictionary<string, object?> expectedValues = DbNullInvocation;
        NullableTestEntity entity = Invoker<NullableTestEntity>.Invoke(expectedValues);


        Assert.Equal(expectedValues["Key"], entity.Key);
        Assert.Null(entity.ByteArrayValue);
        Assert.Null(entity.ByteValue);
        Assert.Null(entity.CharValue);
        Assert.Null(entity.DateOnlyValue);
        Assert.Null(entity.DateTimeValue);
        Assert.Null(entity.DecimalValue);
        Assert.Null(entity.DoubleValue);
        Assert.Null(entity.FloatValue);
        Assert.Null(entity.GuidValue);
        Assert.Null(entity.IntValue);
        Assert.Null(entity.LongValue);
        Assert.Null(entity.ShortValue);
        Assert.Null(entity.TimeOnlyValue);
    }
}
