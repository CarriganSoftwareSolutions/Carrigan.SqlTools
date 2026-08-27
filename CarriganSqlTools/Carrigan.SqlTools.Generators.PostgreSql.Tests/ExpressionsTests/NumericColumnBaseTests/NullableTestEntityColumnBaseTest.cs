using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.NumericColumnBaseTests;

public class NullableTestEntityColumnBaseTest : SqlServerNumericColumnBaseTest<NullableTestEntity>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "NullableTestEntity";

    protected override IEnumerable<string> NumericProperties =>
        [
            nameof(NullableTestEntity.IntValue),
            nameof(NullableTestEntity.LongValue),
            nameof(NullableTestEntity.ShortValue),
            nameof(NullableTestEntity.ByteValue),
            nameof(NullableTestEntity.DecimalValue),
            nameof(NullableTestEntity.FloatValue),
            nameof(NullableTestEntity.DoubleValue)
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
