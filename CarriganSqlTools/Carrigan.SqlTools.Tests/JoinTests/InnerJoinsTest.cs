using SqlTools.Exceptions;
using SqlTools.Joins;
using SqlTools.Predicates;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.JoinTests;

public class InnerJoinsTest
{
    [Fact]
    public void InnerJoinTests_ToSql()
    {
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));

        string actual = ((IJoins)new InnerJoin<JoinLeftTable, JoinRightTable>(id)).ToSql();
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void InnerJoinTests_ArgumentException_InvalidColumnTable()
    {
        PredicatesBase id = new Equal(new Columns<JoinLeftTable>("RightId"), new Columns<JoinRightTable>("Id"));
        Assert.Throws<SqlIdentifierException>(() => new InnerJoin<JoinLeftTable, ColumnTable>(id));
    }
}
