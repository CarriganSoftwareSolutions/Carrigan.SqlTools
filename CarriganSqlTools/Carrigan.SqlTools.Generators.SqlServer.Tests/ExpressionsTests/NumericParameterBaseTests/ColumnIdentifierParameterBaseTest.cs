using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericParameterBaseTests;

public class ColumnIdentifierParameterBaseTest : SqlServerNumericParameterBaseTest<ColumnIdentifiers>
{
    protected override IEnumerable<string> NumericProperties =>
        [
            nameof(ColumnIdentifiers.Id),
            nameof(ColumnIdentifiers.Property),
            nameof(ColumnIdentifiers.ColumnName),
            nameof(ColumnIdentifiers.IdentifierName),
            nameof(ColumnIdentifiers.IdentifierOverrideName)
        ];

    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(ColumnIdentifiers.Id), "IdParameter"),
            NewKvp(nameof(ColumnIdentifiers.Property), "PropertyParameter"),
            NewKvp(nameof(ColumnIdentifiers.ColumnName), "ColumnParameter"),
            NewKvp(nameof(ColumnIdentifiers.IdentifierName), "IdentifierParameter"),
            NewKvp(nameof(ColumnIdentifiers.IdentifierOverrideName), "IdentifierOverrideParameter")
        ]
    );
}
