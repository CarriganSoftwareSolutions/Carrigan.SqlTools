using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Generators.SqlServer;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlGenerator_SelectCountInvalidSelectTableTests
{
    [Fact]
    public void SelectCount_SelectsReferencesNonJoinedTable_InvalidTableException()
    {
        MockEncryption mockEncrypter = new("+Encrypted+");
        SqlGenerator<Customer> customerGenerator = new(mockEncrypter);

        SelectTag selectTag = SelectTag.Get<Order>("Id");

        Assert.Throws<InvalidTableException>(() => customerGenerator.SelectCount(null, selectTag, null, null));
    }
}
