using Carrigan.SqlTools.IntegrationTests.Models;

//IGNORE SPELLING: Lakeview Hillcrest

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class AddressDataSet
{
    public static IEnumerable<Address> Data =>
    [
        new()
        {
            Id = 1,
            StreetName = "Main Street",
            StreetNumber = 1061,
            ZipCode = "10001",
            City = "New York",
            State = "NY",
        },
        new()
        {
            Id = 2,
            StreetName = "Highland Avenue",
            StreetNumber = 2487,
            ZipCode = "02108",
            City = "Boston",
            State = "MA",
        },
        new()
        {
            Id = 3,
            StreetName = "Maple Avenue",
            StreetNumber = 3920,
            ZipCode = "60601",
            City = "Chicago",
            State = "IL",
        },
        new()
        {
            Id = 4,
            StreetName = "Oak Street",
            StreetNumber = 4741,
            ZipCode = "90001",
            City = "Los Angeles",
            State = "CA",
        },
        new()
        {
            Id = 5,
            StreetName = "Park Avenue",
            StreetNumber = 5128,
            ZipCode = "92101",
            City = "San Diego",
            State = "CA",
        },
        new()
        {
            Id = 6,
            StreetName = "Cedar Lane",
            StreetNumber = 6166,
            ZipCode = "77002",
            City = "Houston",
            State = "TX",
        },
        new()
        {
            Id = 7,
            StreetName = "Pine Street",
            StreetNumber = 7022,
            ZipCode = "75201",
            City = "Dallas",
            State = "TX",
        },
        new()
        {
            Id = 8,
            StreetName = "Washington Street",
            StreetNumber = 7987,
            ZipCode = "85001",
            City = "Phoenix",
            State = "AZ",
        },
        new()
        {
            Id = 9,
            StreetName = "Lakeview Drive",
            StreetNumber = 8410,
            ZipCode = "98101",
            City = "Seattle",
            State = "WA",
        },
        new()
        {
            Id = 10,
            StreetName = "Hillcrest Road",
            StreetNumber = 9295,
            ZipCode = "80202",
            City = "Denver",
            State = "CO",
        },
        new()
        {
            Id = 11,
            StreetName = "Church Street",
            StreetNumber = 10341,
            ZipCode = "37203",
            City = "Nashville",
            State = "TN",
        },
        new()
        {
            Id = 12,
            StreetName = "River Road",
            StreetNumber = 11845,
            ZipCode = "33101",
            City = "Miami",
            State = "FL",
        },
        new()
        {
            Id = 13,
            StreetName = "Elm Street",
            StreetNumber = 13202,
            ZipCode = "30303",
            City = "Atlanta",
            State = "GA",
        },
        new()
        {
            Id = 14,
            StreetName = "Spring Street",
            StreetNumber = 14677,
            ZipCode = "28202",
            City = "Charlotte",
            State = "NC",
        },
        new()
        {
            Id = 15,
            StreetName = "Walnut Street",
            StreetNumber = 15530,
            ZipCode = "46204",
            City = "Indianapolis",
            State = "IN",
        },
        new()
        {
            Id = 16,
            StreetName = "Sunset Boulevard",
            StreetNumber = 16342,
            ZipCode = "43215",
            City = "Columbus",
            State = "OH",
        },
        new()
        {
            Id = 17,
            StreetName = "Mill Road",
            StreetNumber = 17890,
            ZipCode = "19103",
            City = "Philadelphia",
            State = "PA",
        },
        new()
        {
            Id = 18,
            StreetName = "Meadow Lane",
            StreetNumber = 18641,
            ZipCode = "32202",
            City = "Jacksonville",
            State = "FL",
        },
        new()
        {
            Id = 19,
            StreetName = "Ridge Road",
            StreetNumber = 20192,
            ZipCode = "89101",
            City = "Las Vegas",
            State = "NV",
        },
        new()
        {
            Id = 20,
            StreetName = "Center Street",
            StreetNumber = 22583,
            ZipCode = "55401",
            City = "Minneapolis",
            State = "MN",
        },
        new()
        {
            Id = 21,
            StreetName = "North Street",
            StreetNumber = 25744,
            ZipCode = "87101",
            City = "Albuquerque",
            State = "NM",
        },
        new()
        {
            Id = 22,
            StreetName = "South Street",
            StreetNumber = 28216,
            ZipCode = "70112",
            City = "New Orleans",
            State = "LA",
        },
        new()
        {
            Id = 23,
            StreetName = "Forest Avenue",
            StreetNumber = 31904,
            ZipCode = "84101",
            City = "Salt Lake City",
            State = "UT",
        },
        new()
        {
            Id = 24,
            StreetName = "Madison Avenue",
            StreetNumber = 36311,
            ZipCode = "64106",
            City = "Kansas City",
            State = "MO",
        },
        new()
        {
            Id = 25,
            StreetName = "Jefferson Street",
            StreetNumber = 42178,
            ZipCode = "63101",
            City = "St. Louis",
            State = "MO",
        },
        new()
        {
            Id = 26,
            StreetName = "Broadway",
            StreetNumber = 48625,
            ZipCode = "97205",
            City = "Portland",
            State = "OR",
        },
        new()
        {
            Id = 27,
            StreetName = "Franklin Road",
            StreetNumber = 55739,
            ZipCode = "99501",
            City = "Anchorage",
            State = "AK",
        }
    ];
}

/*
Footer - Dataset Methodology
This address dataset contains at least one address for each generated customer, with no generated address intended to belong to more than one customer. Street names were selected from generic United States street-name patterns, street numbers were assigned with a center-heavy distribution, and ZIP code, city, and state combinations were selected from valid United States locations with nationwide geographic variety. Any resemblance to a real-world person, household, or address is purely coincidental.
*/
