using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.AttributesTests;
public class ColumnIdentifierTests
{
    private static readonly SqlGenerator<ColumnIdentifiers> _generator = new();
    private static readonly ColumnIdentifiers _entity = new ()
    {
        Id = 1,
        Property = 2,
        ColumnName = 3,
        IdentifierName = 4,
        IdentifierOverrideName = 5
    };
    private static readonly ColumnIdentifiers _model = new()
    {
        Id = 2,
        Property = 3,
        ColumnName = 4,
        IdentifierName = 5,
        IdentifierOverrideName = 6
    };

    private static readonly ColumnIdentifiers _updateValues = new()
    {
        Property = 31,
        ColumnName = 41,
        IdentifierName = 51,
        IdentifierOverrideName = 61
    };


    private static readonly IEnumerable<ColumnIdentifiers> _entities = [_entity, _model];

    [Fact]
    public void DeleteTest()
    {
        SqlQuery query = _generator.Delete(_entity);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] WHERE [Id] = @Id;";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InsertTest()
    {
        SqlQuery query = _generator.Insert(_entity);
        string actual = query.QueryText;
        string expected = "INSERT INTO [ColumnIdentifiers] ([Id], [Property], [Column], [Identifier], [IdentifierOverride]) VALUES (@Id, @Property, @Column, @Identifier, @IdentifierOverride);";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InsertAutoIdTest()
    {
        SqlQuery query = _generator.InsertAutoId(_entity);
        string actual = query.QueryText;
        string expected = SqlGenerator<ColumnIdentifiers>.ModifyInsertQueryToReturnScalar("INSERT INTO [ColumnIdentifiers] ([Property], [Column], [Identifier], [IdentifierOverride]) VALUES (@Property, @Column, @Identifier, @IdentifierOverride);");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UpdateByIdTest()
    {
        SqlQuery query = _generator.UpdateById(_entity);
        string actual = query.QueryText;
        string expected = "UPDATE [ColumnIdentifiers] SET [Property] = @Property, [Column] = @Column, [Identifier] = @Identifier, [IdentifierOverride] = @IdentifierOverride WHERE [Id] = @Id;";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UpdateByIdsTest()
    {
        SqlQuery query = _generator.UpdateByIds(_updateValues, null, _entities);
        string actual = query.QueryText;
        string expected = "UPDATE [ColumnIdentifiers] SET [ColumnIdentifiers].[Property] = @ParameterSet_Property, [ColumnIdentifiers].[Column] = @ParameterSet_Column, [ColumnIdentifiers].[Identifier] = @ParameterSet_Identifier, [ColumnIdentifiers].[IdentifierOverride] = @ParameterSet_IdentifierOverride FROM [ColumnIdentifiers] WHERE (([ColumnIdentifiers].[Id] = @Parameter_0_R_Id) OR ([ColumnIdentifiers].[Id] = @Parameter_1_R_Id))";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WhereTest()
    {
        ColumnValues<ColumnIdentifiers> whereIdEquals = new(nameof(ColumnIdentifiers.Id), 1);
        SqlQuery query = _generator.Delete(null, whereIdEquals);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Id] = @Parameter_Id)";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinTest()
    {
        InnerJoin<ColumnIdentifiers, JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        SqlQuery query = _generator.Delete(join, null);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] INNER JOIN [Right] ON ([ColumnIdentifiers].[Id] = [Right].[Id])";
        Assert.Equal(expected, actual);
    }
}
