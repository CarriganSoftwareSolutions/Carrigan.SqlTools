using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.BooleanColumnBaseTests;

public class NullableTestEntityColumnBaseTest : SqlServerBooleanColumnBaseTest<NullableTestEntity>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "NullableTestEntity";

    protected override IEnumerable<string> BooleanProperties =>
        [
            nameof(NullableTestEntity.BoolValue)
        ];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
    new
    (
        [
            NewKvp(nameof(NullableTestEntity.Key)),
            NewKvp(nameof(NullableTestEntity.IntValue)),
            NewKvp(nameof(NullableTestEntity.LongValue)),
            NewKvp(nameof(NullableTestEntity.ShortValue)),
            NewKvp(nameof(NullableTestEntity.ByteValue)),
            NewKvp(nameof(NullableTestEntity.BoolValue)),
            NewKvp(nameof(NullableTestEntity.DecimalValue)),
            NewKvp(nameof(NullableTestEntity.FloatValue)),
            NewKvp(nameof(NullableTestEntity.DoubleValue)),
            NewKvp(nameof(NullableTestEntity.DateTimeValue)),
            NewKvp(nameof(NullableTestEntity.GuidValue)),
            NewKvp(nameof(NullableTestEntity.CharValue)),
            NewKvp(nameof(NullableTestEntity.TimeOnlyValue)),
            NewKvp(nameof(NullableTestEntity.DateOnlyValue)),
            NewKvp(nameof(NullableTestEntity.ByteArrayValue)),
            NewKvp(nameof(NullableTestEntity.DateTimeOffsetValue)),
        ]
    );
}
