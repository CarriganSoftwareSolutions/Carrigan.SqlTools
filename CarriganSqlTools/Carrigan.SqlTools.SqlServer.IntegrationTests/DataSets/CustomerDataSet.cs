using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;

public static class CustomerDataSet
{
    public static IEnumerable<Customer> Data =>
    [
        new()
        {
            Id = 1,
            FirstName = "James",
            LastName = "Smith",
            Age = 64,
            Gender = 'M',
        },
        new()
        {
            Id = 2,
            FirstName = "Mary",
            LastName = "Johnson",
            Age = 58,
            Gender = 'F',
        },
        new()
        {
            Id = 3,
            FirstName = "John",
            LastName = "Williams",
            Age = 45,
            Gender = 'M',
        },
        new()
        {
            Id = 4,
            FirstName = "Patricia",
            LastName = "Brown",
            Age = 72,
            Gender = 'F',
        },
        new()
        {
            Id = 5,
            FirstName = "Robert",
            LastName = "Jones",
            Age = 39,
            Gender = 'M',
        },
        new()
        {
            Id = 6,
            FirstName = "Jennifer",
            LastName = "Garcia",
            Age = 34,
            Gender = 'F',
        },
        new()
        {
            Id = 7,
            FirstName = "Michael",
            LastName = "Miller",
            Age = 51,
            Gender = 'M',
        },
        new()
        {
            Id = 8,
            FirstName = "Linda",
            LastName = "Davis",
            Age = 67,
            Gender = 'F',
        },
        new()
        {
            Id = 9,
            FirstName = "William",
            LastName = "Rodriguez",
            Age = 29,
            Gender = 'M',
        },
        new()
        {
            Id = 10,
            FirstName = "Elizabeth",
            LastName = "Martinez",
            Age = 43,
            Gender = 'F',
        },
        new()
        {
            Id = 11,
            FirstName = "David",
            LastName = "Hernandez",
            Age = 37,
            Gender = 'M',
        },
        new()
        {
            Id = 12,
            FirstName = "Barbara",
            LastName = "Lopez",
            Age = 79,
            Gender = 'F',
        },
        new()
        {
            Id = 13,
            FirstName = "Richard",
            LastName = "Gonzalez",
            Age = 56,
            Gender = 'M',
        },
        new()
        {
            Id = 14,
            FirstName = "Susan",
            LastName = "Wilson",
            Age = 61,
            Gender = 'F',
        },
        new()
        {
            Id = 15,
            FirstName = "Joseph",
            LastName = "Anderson",
            Age = 48,
            Gender = 'M',
        },
        new()
        {
            Id = 16,
            FirstName = "Jessica",
            LastName = "Thomas",
            Age = 27,
            Gender = 'F',
        },
        new()
        {
            Id = 17,
            FirstName = "Thomas",
            LastName = "Taylor",
            Age = 83,
            Gender = 'M',
        },
        new()
        {
            Id = 18,
            FirstName = "Sarah",
            LastName = "Moore",
            Age = 24,
            Gender = 'F',
        },
        new()
        {
            Id = 19,
            FirstName = "Taylor",
            LastName = "Jackson",
            Age = 31,
            Gender = 'F',
        },
        new()
        {
            Id = 20,
            FirstName = "Jordan",
            LastName = "Martin",
            Age = 22,
            Gender = 'M',
        },
        new()
        {
            Id = 21,
            FirstName = "Charles",
            LastName = "Thompson",
            Age = 54,
            Gender = 'M',
        },
        new()
        {
            Id = 22,
            FirstName = "Karen",
            LastName = "White",
            Age = 46,
            Gender = 'F',
        },
        new()
        {
            Id = 23,
            FirstName = "Christopher",
            LastName = "Harris",
            Age = 33,
            Gender = 'M',
        },
        new()
        {
            Id = 24,
            FirstName = "Nancy",
            LastName = "Clark",
            Age = 69,
            Gender = 'F',
        },
        new()
        {
            Id = 25,
            FirstName = "Daniel",
            LastName = "Lewis",
            Age = 41,
            Gender = 'M',
        }
    ];

    public static IEnumerable<SqlQuery> InsertStatement =>
        Data.Chunk(100).Select(dataSet => new SqlGenerator<Customer>().Insert(null, null, dataSet));
}

/*
Footer - Dataset Methodology
This customer dataset contains twenty deterministic records with unique first-name and last-name combinations. First names and last names were selected from common United States naming patterns, with gender assigned consistently for conventionally male and female names and deterministically for gender-neutral names. Ages are all adult values and were distributed to roughly resemble a broad adult customer population rather than a uniform range. Any relationship between these records and real-world persons is purely coincidental.
*/
