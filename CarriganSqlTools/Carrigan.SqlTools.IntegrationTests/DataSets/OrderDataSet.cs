using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class OrderDataSet
{
    public static IEnumerable<Order> Data =>
    [
        new()
        {
            Id = 1,
            CustomerId = 1,
            AddressId = 1,
            Date = DateOnly.Parse("2008-03-14"),
            SalesTaxPercent = 8.8750m,
        },
        new()
        {
            Id = 2,
            CustomerId = 1,
            AddressId = 1,
            Date = DateOnly.Parse("2012-07-21"),
            SalesTaxPercent = 8.8750m,
        },
        new()
        {
            Id = 3,
            CustomerId = 1,
            AddressId = 1,
            Date = DateOnly.Parse("2016-09-05"),
            SalesTaxPercent = 8.8750m,
        },
        new()
        {
            Id = 4,
            CustomerId = 1,
            AddressId = 2,
            Date = DateOnly.Parse("2020-11-18"),
            SalesTaxPercent = 6.2500m,
        },
        new()
        {
            Id = 5,
            CustomerId = 1,
            AddressId = 3,
            Date = DateOnly.Parse("2024-04-12"),
            SalesTaxPercent = 10.2500m,
        },
        new()
        {
            Id = 6,
            CustomerId = 2,
            AddressId = 4,
            Date = DateOnly.Parse("2009-05-08"),
            SalesTaxPercent = 9.5000m,
        },
        new()
        {
            Id = 7,
            CustomerId = 2,
            AddressId = 4,
            Date = DateOnly.Parse("2014-10-27"),
            SalesTaxPercent = 9.5000m,
        },
        new()
        {
            Id = 8,
            CustomerId = 2,
            AddressId = 4,
            Date = DateOnly.Parse("2018-02-16"),
            SalesTaxPercent = 9.5000m,
        },
        new()
        {
            Id = 9,
            CustomerId = 2,
            AddressId = 5,
            Date = DateOnly.Parse("2022-08-03"),
            SalesTaxPercent = 7.7500m,
        },
        new()
        {
            Id = 10,
            CustomerId = 3,
            AddressId = 6,
            Date = DateOnly.Parse("2011-01-19"),
            SalesTaxPercent = 8.2500m,
        },
        new()
        {
            Id = 11,
            CustomerId = 3,
            AddressId = 6,
            Date = DateOnly.Parse("2017-06-11"),
            SalesTaxPercent = 8.2500m,
        },
        new()
        {
            Id = 12,
            CustomerId = 3,
            AddressId = 6,
            Date = DateOnly.Parse("2023-09-29"),
            SalesTaxPercent = 8.2500m,
        },
        new()
        {
            Id = 13,
            CustomerId = 4,
            AddressId = 7,
            Date = DateOnly.Parse("2005-12-02"),
            SalesTaxPercent = 8.2500m,
        },
        new()
        {
            Id = 14,
            CustomerId = 4,
            AddressId = 7,
            Date = DateOnly.Parse("2013-04-22"),
            SalesTaxPercent = 8.2500m,
        },
        new()
        {
            Id = 15,
            CustomerId = 4,
            AddressId = 8,
            Date = DateOnly.Parse("2019-08-15"),
            SalesTaxPercent = 8.6000m,
        },
        new()
        {
            Id = 16,
            CustomerId = 5,
            AddressId = 9,
            Date = DateOnly.Parse("2010-07-04"),
            SalesTaxPercent = 10.3500m,
        },
        new()
        {
            Id = 17,
            CustomerId = 5,
            AddressId = 9,
            Date = DateOnly.Parse("2021-03-09"),
            SalesTaxPercent = 10.3500m,
        },
        new()
        {
            Id = 18,
            CustomerId = 6,
            AddressId = 10,
            Date = DateOnly.Parse("2015-05-17"),
            SalesTaxPercent = 8.8100m,
        },
        new()
        {
            Id = 19,
            CustomerId = 6,
            AddressId = 10,
            Date = DateOnly.Parse("2023-10-06"),
            SalesTaxPercent = 8.8100m,
        },
        new()
        {
            Id = 20,
            CustomerId = 7,
            AddressId = 11,
            Date = DateOnly.Parse("2006-09-20"),
            SalesTaxPercent = 9.2500m,
        },
        new()
        {
            Id = 21,
            CustomerId = 7,
            AddressId = 12,
            Date = DateOnly.Parse("2016-12-12"),
            SalesTaxPercent = 7.0000m,
        },
        new()
        {
            Id = 22,
            CustomerId = 8,
            AddressId = 13,
            Date = DateOnly.Parse("2004-02-28"),
            SalesTaxPercent = 8.9000m,
        },
        new()
        {
            Id = 23,
            CustomerId = 8,
            AddressId = 13,
            Date = DateOnly.Parse("2018-07-07"),
            SalesTaxPercent = 8.9000m,
        },
        new()
        {
            Id = 24,
            CustomerId = 9,
            AddressId = 14,
            Date = DateOnly.Parse("2020-06-18"),
            SalesTaxPercent = 7.2500m,
        },
        new()
        {
            Id = 25,
            CustomerId = 9,
            AddressId = 14,
            Date = DateOnly.Parse("2024-01-25"),
            SalesTaxPercent = 7.2500m,
        },
        new()
        {
            Id = 26,
            CustomerId = 10,
            AddressId = 15,
            Date = DateOnly.Parse("2014-03-31"),
            SalesTaxPercent = 7.0000m,
        },
        new()
        {
            Id = 27,
            CustomerId = 11,
            AddressId = 17,
            Date = DateOnly.Parse("2013-11-14"),
            SalesTaxPercent = 8.0000m,
        },
        new()
        {
            Id = 28,
            CustomerId = 12,
            AddressId = 18,
            Date = DateOnly.Parse("2003-08-19"),
            SalesTaxPercent = 7.5000m,
        },
        new()
        {
            Id = 29,
            CustomerId = 13,
            AddressId = 19,
            Date = DateOnly.Parse("2012-05-23"),
            SalesTaxPercent = 8.3750m,
        },
        new()
        {
            Id = 30,
            CustomerId = 14,
            AddressId = 21,
            Date = DateOnly.Parse("2009-09-09"),
            SalesTaxPercent = 7.6250m,
        },
        new()
        {
            Id = 31,
            CustomerId = 15,
            AddressId = 22,
            Date = DateOnly.Parse("2016-06-30"),
            SalesTaxPercent = 9.4500m,
        },
        new()
        {
            Id = 32,
            CustomerId = 16,
            AddressId = 23,
            Date = DateOnly.Parse("2020-12-10"),
            SalesTaxPercent = 7.7500m,
        },
        new()
        {
            Id = 33,
            CustomerId = 17,
            AddressId = 24,
            Date = DateOnly.Parse("2002-04-05"),
            SalesTaxPercent = 9.9750m,
        },
        new()
        {
            Id = 34,
            CustomerId = 18,
            AddressId = 25,
            Date = DateOnly.Parse("2023-07-16"),
            SalesTaxPercent = 9.6790m,
        },
        new()
        {
            Id = 35,
            CustomerId = 19,
            AddressId = 26,
            Date = DateOnly.Parse("2019-10-22"),
            SalesTaxPercent = 0.0000m,
        },
        new()
        {
            Id = 36,
            CustomerId = 20,
            AddressId = 27,
            Date = DateOnly.Parse("2024-05-27"),
            SalesTaxPercent = 0.0000m,
        }
    ];
}

/*
Footer - Dataset Methodology
This order dataset gives every generated customer at least one order. Order counts follow a half-bell style distribution where one order is most common and the highest-volume customer has five orders. Order dates were assigned between 2000 and 2025 and checked against the customer's generated age so that each customer would have been at least eighteen during the order year under the dataset's approximate age model. SalesTaxPercent values are best-effort synthetic current-rate approximations for the order address ZIP/city/state combinations, distributed nationwide and not limited to no-sales-tax jurisdictions. They are suitable for integration testing but are not audit-grade historical sales-tax records. Any relationship between these records and real-world persons or transactions is purely coincidental.
*/
