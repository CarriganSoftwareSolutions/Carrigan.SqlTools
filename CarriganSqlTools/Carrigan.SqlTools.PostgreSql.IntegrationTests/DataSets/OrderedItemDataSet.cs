using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;

public static class OrderedItemDataSet
{
    public static IEnumerable<OrderedItem> Data =>
    [
        new()
        {
            OrderId = 1,
            BookId = 4,
            Price = 9.99m,
        },
        new()
        {
            OrderId = 1,
            BookId = 5,
            Price = 11.27m,
        },
        new()
        {
            OrderId = 1,
            BookId = 6,
            Price = 11.65m,
        },
        new()
        {
            OrderId = 2,
            BookId = 7,
            Price = 13.99m,
        },
        new()
        {
            OrderId = 2,
            BookId = 8,
            Price = 8.45m,
        },
        new()
        {
            OrderId = 3,
            BookId = 10,
            Price = 19.99m,
        },
        new()
        {
            OrderId = 4,
            BookId = 3,
            Price = 12.99m,
        },
        new()
        {
            OrderId = 4,
            BookId = 4,
            Price = 9.39m,
        },
        new()
        {
            OrderId = 4,
            BookId = 5,
            Price = 12.71m,
        },
        new()
        {
            OrderId = 5,
            BookId = 6,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 5,
            BookId = 7,
            Price = 13.15m,
        },
        new()
        {
            OrderId = 6,
            BookId = 9,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 7,
            BookId = 2,
            Price = 18.99m,
        },
        new()
        {
            OrderId = 8,
            BookId = 5,
            Price = 11.99m,
        },
        new()
        {
            OrderId = 8,
            BookId = 6,
            Price = 10.33m,
        },
        new()
        {
            OrderId = 9,
            BookId = 8,
            Price = 8.99m,
        },
        new()
        {
            OrderId = 9,
            BookId = 9,
            Price = 10.33m,
        },
        new()
        {
            OrderId = 9,
            BookId = 10,
            Price = 21.19m,
        },
        new()
        {
            OrderId = 10,
            BookId = 1,
            Price = 14.99m,
        },
        new()
        {
            OrderId = 11,
            BookId = 4,
            Price = 9.99m,
        },
        new()
        {
            OrderId = 11,
            BookId = 5,
            Price = 11.27m,
        },
        new()
        {
            OrderId = 12,
            BookId = 7,
            Price = 13.99m,
        },
        new()
        {
            OrderId = 13,
            BookId = 10,
            Price = 19.99m,
        },
        new()
        {
            OrderId = 14,
            BookId = 3,
            Price = 12.99m,
        },
        new()
        {
            OrderId = 14,
            BookId = 4,
            Price = 9.39m,
        },
        new()
        {
            OrderId = 15,
            BookId = 6,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 15,
            BookId = 7,
            Price = 13.15m,
        },
        new()
        {
            OrderId = 15,
            BookId = 8,
            Price = 9.53m,
        },
        new()
        {
            OrderId = 16,
            BookId = 9,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 17,
            BookId = 2,
            Price = 18.99m,
        },
        new()
        {
            OrderId = 18,
            BookId = 5,
            Price = 11.99m,
        },
        new()
        {
            OrderId = 18,
            BookId = 6,
            Price = 10.33m,
        },
        new()
        {
            OrderId = 19,
            BookId = 8,
            Price = 8.99m,
        },
        new()
        {
            OrderId = 20,
            BookId = 1,
            Price = 14.99m,
        },
        new()
        {
            OrderId = 21,
            BookId = 4,
            Price = 9.99m,
        },
        new()
        {
            OrderId = 22,
            BookId = 7,
            Price = 13.99m,
        },
        new()
        {
            OrderId = 22,
            BookId = 8,
            Price = 8.45m,
        },
        new()
        {
            OrderId = 23,
            BookId = 10,
            Price = 19.99m,
        },
        new()
        {
            OrderId = 24,
            BookId = 3,
            Price = 12.99m,
        },
        new()
        {
            OrderId = 25,
            BookId = 6,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 26,
            BookId = 9,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 26,
            BookId = 10,
            Price = 18.79m,
        },
        new()
        {
            OrderId = 27,
            BookId = 2,
            Price = 18.99m,
        },
        new()
        {
            OrderId = 28,
            BookId = 5,
            Price = 11.99m,
        },
        new()
        {
            OrderId = 29,
            BookId = 8,
            Price = 8.99m,
        },
        new()
        {
            OrderId = 30,
            BookId = 1,
            Price = 14.99m,
        },
        new()
        {
            OrderId = 30,
            BookId = 2,
            Price = 17.85m,
        },
        new()
        {
            OrderId = 31,
            BookId = 4,
            Price = 9.99m,
        },
        new()
        {
            OrderId = 32,
            BookId = 7,
            Price = 13.99m,
        },
        new()
        {
            OrderId = 33,
            BookId = 10,
            Price = 19.99m,
        },
        new()
        {
            OrderId = 34,
            BookId = 3,
            Price = 12.99m,
        },
        new()
        {
            OrderId = 34,
            BookId = 4,
            Price = 9.39m,
        },
        new()
        {
            OrderId = 35,
            BookId = 6,
            Price = 10.99m,
        },
        new()
        {
            OrderId = 36,
            BookId = 9,
            Price = 10.99m,
        }
    ];

    public static IEnumerable<SqlQuery> InsertStatement =>
        Data.Chunk(100).Select(dataSet => new SqlGenerator<OrderedItem>().Insert(null, null, dataSet));
}

/*
Footer - Dataset Methodology
This ordered item dataset gives each order between one and three book records, using a half-bell style distribution where one book is most common. Book selections are deterministic and do not repeat within a single order. Ordered item prices were generated within a narrow range around the corresponding Book.Price value so that tests can validate decimal values without relying on exact retail history. Any relationship between these records and real-world persons, purchases, or transactions is purely coincidental.
*/
