using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class SqlExpressionExtensionsTests
{
    [Fact]
    public void AsSelectTag_AliasName_ReturnsSelectTagWithExpressionAndAlias()
    {
        SqlExpression sqlExpression = new Parameter(123, "Value");
        AliasName aliasName = new("Result");

        SelectTag selectTag = sqlExpression.AsSelectTag(aliasName);

        Assert.Same(sqlExpression, selectTag.SqlExpression);
        Assert.Equal(new SelectTag(sqlExpression, aliasName), selectTag);
    }

    [Fact]
    public void AsSelectTag_String_ReturnsSelectTagWithExpressionAndAlias()
    {
        SqlExpression sqlExpression = new Parameter(123, "Value");

        SelectTag selectTag = sqlExpression.AsSelectTag("Result");

        Assert.Same(sqlExpression, selectTag.SqlExpression);
        Assert.Equal(new SelectTag(sqlExpression, new AliasName("Result")), selectTag);
    }
}
