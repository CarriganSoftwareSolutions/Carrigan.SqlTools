using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class InnerJoinsTest
{
    [Fact]
    public void InnerJoinTests_ToSql()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = (new Joins<JoinLeftTable>(new InnerJoin<JoinRightTable>(id))).ToSql();
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }
}
