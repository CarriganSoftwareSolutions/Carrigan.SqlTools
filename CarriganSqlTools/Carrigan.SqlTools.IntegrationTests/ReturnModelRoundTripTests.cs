//Ignore Spelling: SqlTools, Localdb, Respawn, Respawner, Carrigan, SqlServer

using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IntegrationTests.Fixtures;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Carrigan.SqlTools.IntegrationTests;

public sealed class ReturnModelRoundTripTests : IClassFixture<ReturnFixture>
{
    private readonly ReturnFixture _fixture;
    private readonly SqlGenerator<ReturnModel> _generator;

    public ReturnModelRoundTripTests(ReturnFixture fixture)
    {
        _fixture = fixture;
        _generator = new(new SqlServerDialect());
    }

    [Fact]
    public async Task Insert_WithReturnColumns_RoundTrips_AllValues()
    {
        await _fixture.ResetAsync();

        // Arrange: three entities, only NotKey1-3 will be inserted explicitly
        ReturnModel[] toInsert =
        [
            new() { NotKey1 = 10, NotKey2 = 20, NotKey3 = 30 },
            new() { NotKey1 = 11, NotKey2 = 21, NotKey3 = 31 },
            new() { NotKey1 = 12, NotKey2 = 22, NotKey3 = 32 }
        ];

        // Only these columns are in the INSERT column list
        ColumnCollection<ReturnModel> insertColumns =
            new("NotKey1", "NotKey2", "NotKey3");

        // These columns are returned via OUTPUT
        ColumnCollection<ReturnModel> returnColumns =
            new("Id1", "Id2", "DateTime", "Status", "DeletedFlag");

        SqlQuery insertQuery = _generator.Insert(insertColumns, returnColumns, toInsert);

        SqlConnection connection = new(_fixture.ConnectionString);

        // Act 1: execute INSERT and capture returned values
        List<ReturnModel> returnedRows =
            [.. (await CommandsAsync.ExecuteReaderAsync<ReturnModel>(insertQuery,transaction: null, connection))];

        // Assert: we got three rows back
        Assert.Equal(3, returnedRows.Count);

        // Test returned values against expectations
        foreach (ReturnModel row in returnedRows)
        {
            // Id1 should be a positive identity value
            Assert.True(row.Id1 > 0);

            // Guid should be non-empty (non-null Guid)
            Assert.NotEqual(Guid.Empty, row.Id2);

            // Defaults
            Assert.Equal("Pending", row.Status);
            Assert.False(row.DeletedFlag);

            // We do NOT assert DateTime against a clock; it is validated against DB below.
        }

        // Act 2: select the rows from the database and compare to insert results
        SqlQuery selectQuery = _generator.SelectById(returnedRows);

        List<ReturnModel> dbRows = [.. (await CommandsAsync.ExecuteReaderAsync<ReturnModel>(selectQuery, transaction: null, connection))];

        // We should see the same three rows in the database
        Assert.Equal(3, dbRows.Count);

        // Compare per-row: DB vs values returned by the INSERT
        for (int i = 0; i < dbRows.Count; i++)
        {
            ReturnModel fromInsert = returnedRows[i];
            ReturnModel fromDb = dbRows[i];

            // Primary keys
            Assert.Equal(fromInsert.Id1, fromDb.Id1);
            Assert.Equal(fromInsert.Id2, fromDb.Id2);

            // Inserted values (NotKey1-3) should match original entities
            Assert.Equal(toInsert[i].NotKey1, fromDb.NotKey1);
            Assert.Equal(toInsert[i].NotKey2, fromDb.NotKey2);
            Assert.Equal(toInsert[i].NotKey3, fromDb.NotKey3);

            // Defaults: values returned from INSERT must match stored values
            Assert.Equal(fromInsert.Status, fromDb.Status);
            Assert.Equal(fromInsert.DeletedFlag, fromDb.DeletedFlag);

            // DateTime: ensure the value from INSERT matches what is actually in the DB
            Assert.Equal(fromInsert.DateTime, fromDb.DateTime);
        }

        connection.Dispose();
    }
}
