using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.AttributesTests;
public class ColumnIdentifierTests
{
    private static readonly SqlGenerator<ColumnIdentifiers> _generator = new();
    private static readonly ColumnIdentifiers _identifier = new ()
    {
        Id = 1,
        Property = 2,
        ColumnName = 3,
        IdentifierName = 4,
        IdentifierOverrideName = 5
    };

    [Fact]
    public void InsertTest()
    {
        SqlQuery query = _generator.Insert(_identifier);
        string actual = query.QueryText;
        string expected = "INSERT INTO [ColumnIdentifiers] ([Id], [Property], [Column], [Identifier], [IdentifierOverride]) VALUES (@Id, @Property, @Column, @Identifier, @IdentifierOverride);";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InsertAutoIdTest()
    {
        SqlQuery query = _generator.InsertAutoId(_identifier);
        string actual = query.QueryText;
        string expected = SqlGenerator<ColumnIdentifiers>.ModifyInsertQueryToReturnScalar("INSERT INTO [ColumnIdentifiers] ([Property], [Column], [Identifier], [IdentifierOverride]) VALUES (@Property, @Column, @Identifier, @IdentifierOverride);");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UpdateByIdTest()
    {
        SqlQuery query = _generator.UpdateById(_identifier);
        string actual = query.QueryText;
        string expected = "UPDATE [ColumnIdentifiers] SET [Property] = @Property, [Column] = @Column, [Identifier] = @Identifier, [IdentifierOverride] = @IdentifierOverride WHERE [Id] = @Id;";
        Assert.Equal(expected, actual);
    }
}
