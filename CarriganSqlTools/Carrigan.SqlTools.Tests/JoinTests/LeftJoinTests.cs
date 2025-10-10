using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class LeftJoinsTest
{
    [Fact]
    public void LeftJoinTests_ToSql()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinLeftTable, JoinRightTable> leftJoin = new(id);

        string actual = ((IJoins)leftJoin).ToSql();
        string expected = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoinTests_ArgumentException_InvalidColumnTable()
    {
        PredicatesLogic.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Assert.Throws<InvalidColumnException>(() => new LeftJoin<JoinLeftTable, ColumnTable>(id));
    }
}
