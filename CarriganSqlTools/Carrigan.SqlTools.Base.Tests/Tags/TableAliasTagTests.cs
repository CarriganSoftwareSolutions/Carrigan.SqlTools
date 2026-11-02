using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Tags;

public class TableAliasTagTests
{
    [Fact]
    public void Constructor_AliasTag_PreservesAlias()
    {
        SqlTools.Tags.ColumnAlias aliasTag = new(new IdentifierTypes.ColumnAlias("DerivedTable"));

        TableAliasTag tableAliasTag = new(aliasTag);

        Assert.Same(aliasTag, tableAliasTag.TableAlias);
        Assert.Equal("DerivedTable", tableAliasTag.ToSql(NeutralDialect.Instance));
    }
}
