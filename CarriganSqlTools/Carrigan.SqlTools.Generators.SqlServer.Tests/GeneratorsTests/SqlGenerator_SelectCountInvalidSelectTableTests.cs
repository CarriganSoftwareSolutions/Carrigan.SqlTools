using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public sealed class SqlGenerator_SelectCountInvalidSelectTableTests
{
    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SelectCount_SelectsReferencesNonJoinedTable_InvalidTableException()
    {
        MockEncryption mockEncrypter = new("+Encrypted+");
        SqlGenerator<Customer> customerGenerator = new(mockEncrypter);

        SelectTagBase selectTag = SelectTagGenerator.Get<Order>("Id");

        Assert.Throws<InvalidTableException>(() => customerGenerator.SelectCount(null, selectTag, null, null));
    }
}
