using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericParameterBaseTests;

public class SqlTypeEntityParameterBaseTest : SqlServerNumericParameterBaseTest<SqlTypeEntity>
{
    protected override IEnumerable<string> NumericProperties =>
        [
            nameof(SqlTypeEntity.IntValue),
            nameof(SqlTypeEntity.LongValue),
            nameof(SqlTypeEntity.ShortValue),
            nameof(SqlTypeEntity.ByteValue),
            nameof(SqlTypeEntity.DecimalValue),
            nameof(SqlTypeEntity.FloatValue),
            nameof(SqlTypeEntity.DoubleValue)
        ];

    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(SqlTypeEntity.IntValue)),
            NewKvp(nameof(SqlTypeEntity.LongValue)),
            NewKvp(nameof(SqlTypeEntity.ShortValue)),
            NewKvp(nameof(SqlTypeEntity.ByteValue)),
            NewKvp(nameof(SqlTypeEntity.BoolValue)),
            NewKvp(nameof(SqlTypeEntity.DecimalValue)),
            NewKvp(nameof(SqlTypeEntity.FloatValue)),
            NewKvp(nameof(SqlTypeEntity.DoubleValue)),
            NewKvp(nameof(SqlTypeEntity.StringValue)),
            NewKvp(nameof(SqlTypeEntity.DateTimeValue)),
            NewKvp(nameof(SqlTypeEntity.GuidValue)),
            NewKvp(nameof(SqlTypeEntity.ByteArrayValue)),
            NewKvp(nameof(SqlTypeEntity.CharValue)),
            NewKvp(nameof(SqlTypeEntity.TimeOnlyValue)),
            NewKvp(nameof(SqlTypeEntity.DateOnlyValue)),
            NewKvp(nameof(SqlTypeEntity.DateTimeOffsetValue))
        ]
    );
}
