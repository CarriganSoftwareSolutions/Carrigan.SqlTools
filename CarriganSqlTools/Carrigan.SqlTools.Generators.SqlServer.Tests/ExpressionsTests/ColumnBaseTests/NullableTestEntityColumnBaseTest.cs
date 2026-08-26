using Carrigan.SqlTools.Base.Tests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;

public class NullableTestEntityColumnBaseTest : SqlServerColumnBaseTest<NullableTestEntity>
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

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<NullableTestEntity>(propertyName);
    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<NullableTestEntity>(propertyName);
}
