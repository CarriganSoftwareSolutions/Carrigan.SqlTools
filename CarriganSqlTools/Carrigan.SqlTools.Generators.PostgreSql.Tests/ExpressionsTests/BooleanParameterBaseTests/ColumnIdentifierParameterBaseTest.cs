using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanParameterBaseTests;

public class ColumnIdentifierParameterBaseTest : PostgreSqlBooleanParameterBaseTest<ColumnIdentifiers>
{
    protected override IEnumerable<string> BooleanProperties =>
        [];

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
