using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;

public static class BookDataSet
{
    public static void Validate(IEnumerable<Book> actualBooks, int expectedId)
    {
        Book actual = actualBooks.Where(book => book.Id == expectedId).Single();
        Validate(actual, expectedId);
    }
    public static void Validate(Book actual, int expectedId)
    {
        Book expected = Data.Where(book => book.Id == expectedId).Single();

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Author, actual.Author);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Price, actual.Price);
        Assert.Equal(expected.Pages, actual.Pages);
        Assert.Equal(expected.YearPublished, actual.YearPublished);
    }

    public static readonly IEnumerable<Book> Data =
    [
        new()
        {
            Id = 1,
            Title = "Pride and Prejudice",
            Author = "Jane Austen",
            Description = "A social comedy about manners, marriage, reputation, and personal judgment in Regency England.",
            Price = 14.99m,
            Pages = 432,
            YearPublished = 1813,
        },
        new()
        {
            Id = 2,
            Title = "Moby-Dick",
            Author = "Herman Melville",
            Description = "A sea voyage becomes an obsessive pursuit of the white whale that once maimed Captain Ahab.",
            Price = 18.99m,
            Pages = 635,
            YearPublished = 1851,
        },
        new()
        {
            Id = 3,
            Title = "Adventures of Huckleberry Finn",
            Author = "Mark Twain",
            Description = "A young runaway travels the Mississippi River while confronting friendship, freedom, and conscience.",
            Price = 12.99m,
            Pages = 366,
            YearPublished = 1884,
        },
        new()
        {
            Id = 4,
            Title = "Alice's Adventures in Wonderland",
            Author = "Lewis Carroll",
            Description = "A girl follows a white rabbit into a strange world of riddles, wordplay, and impossible creatures.",
            Price = 9.99m,
            Pages = 200,
            YearPublished = 1865,
        },
        new()
        {
            Id = 5,
            Title = "The Adventures of Sherlock Holmes",
            Author = "Arthur Conan Doyle",
            Description = "A collection of detective cases solved through observation, deduction, and the methods of Sherlock Holmes.",
            Price = 11.99m,
            Pages = 307,
            YearPublished = 1892,
        },
        new()
        {
            Id = 6,
            Title = "Frankenstein",
            Author = "Mary Shelley",
            Description = "A scientist creates life and faces the tragic moral consequences of ambition and isolation.",
            Price = 10.99m,
            Pages = 280,
            YearPublished = 1818,
        },
        new()
        {
            Id = 7,
            Title = "Dracula",
            Author = "Bram Stoker",
            Description = "A gothic novel of journals and letters chronicling the effort to stop Count Dracula in England.",
            Price = 13.99m,
            Pages = 418,
            YearPublished = 1897,
        },
        new()
        {
            Id = 8,
            Title = "The Time Machine",
            Author = "H. G. Wells",
            Description = "An inventor travels far into the future and discovers the divided descendants of humanity.",
            Price = 8.99m,
            Pages = 118,
            YearPublished = 1895,
        },
        new()
        {
            Id = 9,
            Title = "The Picture of Dorian Gray",
            Author = "Oscar Wilde",
            Description = "A young man remains outwardly beautiful while his hidden portrait records the corruption of his choices.",
            Price = 10.99m,
            Pages = 254,
            YearPublished = 1890,
        },
        new()
        {
            Id = 10,
            Title = "The Count of Monte Cristo",
            Author = "Alexandre Dumas",
            Description = "A wrongfully imprisoned sailor escapes, finds treasure, and pursues justice and revenge.",
            Price = 19.99m,
            Pages = 1276,
            YearPublished = 1844,
        },
        new()
        {
            Id = 11,
            Title = "The Scarlet Pimpernel",
            Author = "Baroness Orczy",
            Description = "A public domain adventure novel about a mysterious English nobleman who rescues French aristocrats during the Reign of Terror.",
            Price = null,
            Pages = 320,
            YearPublished = 1905,
        }
    ];

    public static IEnumerable<SqlQuery> InsertStatement =>
        Data.Chunk(100).Select(dataSet => new SqlGenerator<Book>().Insert(null, null, dataSet));
}

/*
Footer - Dataset Methodology
This book dataset contains ten deterministic records selected from older literary works commonly available through public-domain collections. Public domain titles were specifically chosen so that the integration-test data avoids current copyrighted catalog entries. Prices, page counts, descriptions, and stock-adjacent values are synthetic values intended for repeatable software tests and should not be treated as current retail, bibliographic, or publishing data.
*/
