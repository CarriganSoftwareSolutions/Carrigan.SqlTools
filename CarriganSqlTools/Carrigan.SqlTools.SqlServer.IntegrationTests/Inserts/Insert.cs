using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

internal static class Insert
{
    public static IEnumerable<SqlQuery> AddressInsertStatement =>
        AddressDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<Address>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> BookInsertStatement =>
        BookDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<Book>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> BookStatsInsertStatement =>
        BookStatsDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<BookStats>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> CustomerInsertStatement =>
        CustomerDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<Customer>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> LeftInsertStatement =>
        LeftDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<Left>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> OrderInsertStatement =>
        OrderDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<Order>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> OrderedItemInsertStatement =>
        OrderedItemDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<OrderedItem>().Insert(null, null, dataSet));
    public static IEnumerable<SqlQuery> RightInsertStatement =>
        RightDataSet.Data.Chunk(100).Select(dataSet => new SqlGenerator<Right>().Insert(null, null, dataSet));
}
