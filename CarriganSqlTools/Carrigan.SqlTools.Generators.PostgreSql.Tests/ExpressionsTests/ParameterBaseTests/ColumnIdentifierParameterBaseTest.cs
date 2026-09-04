using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ParameterBaseTests;

public class ColumnIdentifierParameterBaseTest : PostgreSqlParameterBaseTest<ColumnIdentifiers>
{
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
