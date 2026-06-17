using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;
public class ColumnEqualsColumnTests
{
    private static readonly SqlGenerator<JoinLeftTable> leftGenerator = new();
    private static readonly SqlGenerator<JoinRightTable> rightGenerator = new();
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    public void NewLeftRight()
    {
        ColumnEqualsColumn<JoinLeftTable, JoinRightTable> columnEqualsColumn = new(nameof(JoinLeftTable.RightId), nameof(JoinRightTable.Id));
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    public void NewRightLast()
    {
        ColumnEqualsColumn<JoinRightTable, JoinLastTable> columnEqualsColumn = new(nameof(JoinRightTable.LastId), nameof(JoinRightTable.Id));
    }

    [Fact]
    public void LeftInvalid() => 
        Assert.Throws<InvalidPropertyException<JoinLeftTable>>(() => { ColumnEqualsColumn<JoinLeftTable, JoinRightTable> columnEqualsColumn = new("QWERTY", nameof(JoinRightTable.Id)); });

    [Fact]
    public void RightInvalid() => 
        Assert.Throws<InvalidPropertyException<JoinRightTable>>(() => { ColumnEqualsColumn<JoinLeftTable, JoinRightTable> columnEqualsColumn = new(nameof(JoinLeftTable.RightId), "ASWD"); });

    [Fact]
    public void PredicateColumnEqualsColumn()
    {
        //This error should be thrown, because JoinRightTable isn't being joined in the select statement.
        ColumnEqualsColumn<JoinLeftTable, JoinRightTable> columnEqualsColumn = new(nameof(JoinLeftTable.RightId), nameof(JoinRightTable.Id));
        Assert.Throws<InvalidTableException>(() =>
        {
            SqlQuery query = leftGenerator.Select(null, null, null, null, columnEqualsColumn, null, null, null);
        });
    }

    [Fact]
    public void LeftRightSql()
    {
        ColumnEqualsColumn<JoinLeftTable, JoinRightTable> columnEqualsColumn = new(nameof(JoinLeftTable.RightId), nameof(JoinRightTable.Id));
        Joins<JoinLeftTable> relations = new (new Join<JoinRightTable>(columnEqualsColumn));
        SqlQuery query = leftGenerator.Select(null, null, null, relations, null, null, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void RightLastSql()
    {
        ColumnEqualsColumn<JoinRightTable, JoinLastTable> columnEqualsColumn = new(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));
        Joins<JoinRightTable> relations = new (new Join<JoinLastTable>(columnEqualsColumn));
        SqlQuery query = rightGenerator.Select(null, null, null, relations, null, null, null, null);

        Assert.Equal("SELECT [Right].* FROM [Right] JOIN [Last] ON ([Right].[LastId] = [Last].[Id])", query.QueryText);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void LeftEmpty_ThrowsInvalidPropertyException() =>
    Assert.Throws<InvalidPropertyException<JoinLeftTable>>(() =>
        new ColumnEqualsColumn<JoinLeftTable, JoinRightTable>(string.Empty, nameof(JoinRightTable.Id)));

    [Fact]
    public void RightEmpty_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<JoinRightTable>>(() =>
            new ColumnEqualsColumn<JoinLeftTable, JoinRightTable>(nameof(JoinLeftTable.RightId), string.Empty));

    [Fact]
    public void LeftNull_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<JoinLeftTable>>(() =>
            new ColumnEqualsColumn<JoinLeftTable, JoinRightTable>(null!, nameof(JoinRightTable.Id)));

    [Fact]
    public void RightNull_ThrowsInvalidPropertyException() =>
        Assert.Throws<InvalidPropertyException<JoinRightTable>>(() =>
            new ColumnEqualsColumn<JoinLeftTable, JoinRightTable>(nameof(JoinLeftTable.RightId), null!));

}
