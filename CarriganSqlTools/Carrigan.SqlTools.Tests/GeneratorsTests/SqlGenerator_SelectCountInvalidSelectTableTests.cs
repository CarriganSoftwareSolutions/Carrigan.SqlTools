using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;
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

        // Selects include [Order] but we provide no joins for [Order]
        SelectTags selectTags =
            SelectTags.Get<Customer>("Id", "CustomerId")
                .Append<Order>("Id", "OrderId");

        Assert.Throws<InvalidTableException>(() => customerGenerator.SelectCount(selectTags, null, null));
    }
}
