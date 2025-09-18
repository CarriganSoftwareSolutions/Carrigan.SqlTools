using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;

namespace Carrigan.SqlTools.Tests.IdentifierTests;
public class ColumnIdentifierTests
{
    private static readonly SqlGenerator<ColumnIdentifiers> _generator = new();
    private static readonly ColumnIdentifiers _identifier = new ColumnIdentifiers()
    {
        Id = 1,
        Property = 2,
        ColumnName = 3,
        IdentifierName = 4,
        IdentifierOverrideName = 5
    };

    [Fact]
    public void PropertyNameTest()
    {
        SqlQuery query = _generator.UpdateById(_identifier);
        string actual = query.QueryText;
        string expected = "UPDATE [ColumnIdentifiers] SET [Property] = @Property, [Column] = @Column, [Identifier] = @Identifier, [IdentifierOverride] = @IdentifierOverride WHERE [Id] = @Id;";
        Assert.Equal(expected, actual);
    }
}
