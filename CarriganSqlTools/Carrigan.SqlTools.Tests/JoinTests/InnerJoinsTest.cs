using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class InnerJoinsTest
{
    [Fact]
    public void InnerJoinTests_ToSql()
    {
        Predicates.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = ((IJoins)new InnerJoin<JoinLeftTable, JoinRightTable>(id)).ToSql();
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void InnerJoinTests_ArgumentException_InvalidColumnTable()
    {
        Predicates.Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Assert.Throws<InvalidColumnException>(() => new InnerJoin<JoinLeftTable, ColumnTable>(id));
    }
}
