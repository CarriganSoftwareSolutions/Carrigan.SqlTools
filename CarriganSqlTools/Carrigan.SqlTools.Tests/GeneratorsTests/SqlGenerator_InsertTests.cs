using Carrigan.SqlTools.Query;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using System.Text;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

//IGNORE SPELLING: myschema shhh

public class SqlGenerator_InsertTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;
    private readonly SqlGenerator<SqlTypeEntity> _sqlGeneratorForSqlTypeEntity;
    private readonly SqlGenerator<NullableTestEntity> _sqlGeneratorForNullableTestEntity;
    private readonly SqlGenerator<EntityWithEncryption> _sqlGeneratorForEntityWithEncryption;

    public SqlGenerator_InsertTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>();
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>();
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>();
        _sqlGeneratorForSqlTypeEntity = new SqlGenerator<SqlTypeEntity>();
        _sqlGeneratorForNullableTestEntity = new SqlGenerator<NullableTestEntity>();
        _sqlGeneratorForEntityWithEncryption = new SqlGenerator<EntityWithEncryption>(_mockEncrypter);
    }

    private static string ModifyInsertQueryToReturnScalar(string queryText)
    {
        // Build the final query using a temporary table to store the GUID
        StringBuilder sqlQuery = new();
        sqlQuery.AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);");
        sqlQuery.AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"));
        sqlQuery.AppendLine("SELECT InsertedId FROM @OutputTable;");
        return sqlQuery.ToString();
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectSql_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id, @Name, @DateOf, @When);";

        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectParameters()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id, @Name, @DateOf, @When);";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "DateOf").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "When").Single().Value); // When
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectSql_WithTableAttribute_WithAutoId()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.InsertAutoId(testEntity);

        string expectedSql = ModifyInsertQueryToReturnScalar("INSERT INTO [Test] ([Name], [DateOf], [When]) VALUES (@Name, @DateOf, @When);");

        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectParameters_AutoId()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.InsertAutoId(testEntity);

        string expectedSql = ModifyInsertQueryToReturnScalar("INSERT INTO [Test] ([Name], [DateOf], [When]) VALUES (@Name, @DateOf, @When);");

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "DateOf").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "When").Single().Value); // WhenF"
    }

    [Fact]
    public void SqlInsertString_ExcludesNotMappedProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            Where = "Here", // Should be excluded
            HideTimeFlag = true, // Should be excluded
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id, @Name, @DateOf, @When);";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "DateOf").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "When").Single().Value); // When


        Assert.DoesNotContain("Where", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Where");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "HideTimeFlag");
    }

    [Fact]
    public void SqlInsertString_HandlesNullValues()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = null!, // Nullable property
            When = null, // Nullable property
            DateOf = DateTime.UtcNow
        };


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(testEntity);

        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "When").Single().Value); // When
    }

    [Fact]
    public void SqlInsertString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.Insert(entityWithoutTableAttribute);

        string expectedSql = "INSERT INTO [EntityWithoutTableAttribute] ([Id], [Description]) VALUES (@Id, @Description);";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlInsert_Multi_Insert()
    {
        EntityWithoutTableAttribute entity1 = new()
        {
            Id = 1,
            Description = "1a"
        };
        EntityWithoutTableAttribute entity2 = new()
        {
            Id = 2,
            Description = "2b"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.Insert([entity1, entity2]);
        string expectedSql = "INSERT INTO [EntityWithoutTableAttribute] ([Id], [Description]) VALUES (@Id_0, @Description_0), (@Id_1, @Description_1);";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedInt = 1;
        int actualInt = (int)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "Id_0").Single()];
        Assert.Equal(expectedInt, actualInt);

        expectedInt = 2;
        actualInt = (int)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "Id_1").Single()];
        Assert.Equal(expectedInt, actualInt);

        string expectedString = "1a";
        string actualString = (string)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "Description_0").Single()];
        Assert.Equal(expectedString, actualString);

        expectedString = "2b";
        actualString = (string)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "Description_1").Single()];
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void SqlInsertString_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithSchema.Insert(entityWithSchema);

        string expectedSql = "INSERT INTO [myschema].[EntityWithSchema] ([Id], [Description]) VALUES (@Id, @Description);";

        Assert.Equal(expectedSql, query.QueryText);
    }


    [Fact]
    public void SqlInsertString_IgnoresClassTypeProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1),
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id, @Name, @DateOf, @When);";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);


        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();

        SqlQuery query = _sqlGeneratorForSqlTypeEntity.Insert(entity);

        string expectedSql = "INSERT INTO [TestSqlTypes] ([IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [StringValue], [DateTimeValue], [GuidValue], [ByteArrayValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [DateTimeOffsetValue]) VALUES (@IntValue, @LongValue, @ShortValue, @ByteValue, @BoolValue, @DecimalValue, @FloatValue, @DoubleValue, @StringValue, @DateTimeValue, @GuidValue, @ByteArrayValue, @CharValue, @TimeOnlyValue, @DateOnlyValue, @DateTimeOffsetValue);";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(16, query.Parameters.Count);

        Assert.Equal(42, query.Parameters.Where(param => param.Key == "IntValue").Single().Value);                                                // IntValue
        Assert.Equal(1234567890L, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);                                      // LongValue
        Assert.Equal((short)32000, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);                                    // ShortValue
        Assert.Equal((byte)255, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);                                        // ByteValue
        Assert.Equal(true, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);                                             // BoolValue
        Assert.Equal(99.99m, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);                                        // DecimalValue
        Assert.Equal(3.14f, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);                                           // FloatValue
        Assert.Equal(123.456, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);                                        // DoubleValue
        Assert.Equal("Test String", query.Parameters.Where(param => param.Key == "StringValue").Single().Value);                                  // StringValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value);    // DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "GuidValue").Single().Value); // GuidValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);             // ByteArrayValue
        Assert.Equal('A', query.Parameters.Where(param => param.Key == "CharValue").Single().Value);                                              // CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value);                        // TimeOnlyValue
        Assert.Equal(new DateOnly(1776, 7, 4), query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value);                     // DateOnlyValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value);                // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlInsertStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();

        SqlQuery query = _sqlGeneratorForNullableTestEntity.Insert(entity);

        string expectedSql = "INSERT INTO [NullableTestEntity] ([Key], [IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [DateTimeValue], [GuidValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [ByteArrayValue], [DateTimeOffsetValue]) VALUES (@Key, @IntValue, @LongValue, @ShortValue, @ByteValue, @BoolValue, @DecimalValue, @FloatValue, @DoubleValue, @DateTimeValue, @GuidValue, @CharValue, @TimeOnlyValue, @DateOnlyValue, @ByteArrayValue, @DateTimeOffsetValue);";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "Key").Single().Value);                                             // Key
        Assert.Equal((int?)1, query.Parameters.Where(param => param.Key == "IntValue").Single().Value);                                           // Nullable IntValue
        Assert.Equal((long?)123456789L, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);                                // Nullable LongValue
        Assert.Equal((short?)123, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);                                     // Nullable ShortValue
        Assert.Equal((byte?)255, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);                                       // Nullable ByteValue
        Assert.Equal((bool?)true, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);                                      // Nullable BoolValue
        Assert.Equal((decimal?)99.99m, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);                              // Nullable DecimalValue
        Assert.Equal((float?)3.14f, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);                                   // Nullable FloatValue
        Assert.Equal((double?)1.618, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);                                 // Nullable DoubleValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value);    // Nullable DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "GuidValue").Single().Value); // Nullable GuidValue
        Assert.Equal((char?)'A', query.Parameters.Where(param => param.Key == "CharValue").Single().Value);                                       // Nullable CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value);                        // Nullable TimeOnlyValue
        Assert.Equal(new DateOnly(1, 12, 25), query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value);                      // Nullable DateOnlyValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);             // Nullable ByteArrayValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value);                // Nullable DateTimeOffsetValue

    }


    [Fact]
    public void TestSqlInsertStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();

        SqlQuery query = _sqlGeneratorForNullableTestEntity.Insert(entity);

        string expectedSql = "INSERT INTO [NullableTestEntity] ([Key], [IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [DateTimeValue], [GuidValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [ByteArrayValue], [DateTimeOffsetValue]) VALUES (@Key, @IntValue, @LongValue, @ShortValue, @ByteValue, @BoolValue, @DecimalValue, @FloatValue, @DoubleValue, @DateTimeValue, @GuidValue, @CharValue, @TimeOnlyValue, @DateOnlyValue, @ByteArrayValue, @DateTimeOffsetValue);";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "Key").Single().Value);                   // Key
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "IntValue").Single().Value);            // Nullable IntValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);           // Nullable LongValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);          // Nullable ShortValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);           // Nullable ByteValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);           // Nullable BoolValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);        // Nullable DecimalValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);          // Nullable FloatValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);         // Nullable DoubleValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value);       // Nullable DateTimeValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "GuidValue").Single().Value);           // Nullable GuidValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "CharValue").Single().Value);           // Nullable CharValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value);       // Nullable TimeOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value);       // Nullable DateOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);      // Nullable ByteArrayValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value); // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void SqlInsertString_Encrypted()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = "Shhh..."
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.InsertAutoId(entity);

        string expectedSql = ModifyInsertQueryToReturnScalar("INSERT INTO [Test] ([NotSensitiveData], [SensitiveData], [KeyVersion]) VALUES (@NotSensitiveData, @SensitiveData, @KeyVersion);");

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "NotSensitiveData").Single().Value);
        Assert.NotEqual("Shhh...", query.Parameters.Where(param => param.Key == "SensitiveData").Single().Value);
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(query.Parameters.Where(param => param.Key == "SensitiveData").Single().Value.ToString()));
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "KeyVersion").Single().Value);
    }

    [Fact]
    public void SqlInsertString_Encrypted_Null()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = null
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.InsertAutoId(entity);

        string expectedSql = ModifyInsertQueryToReturnScalar("INSERT INTO [Test] ([NotSensitiveData], [SensitiveData], [KeyVersion]) VALUES (@NotSensitiveData, @SensitiveData, @KeyVersion);");

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "NotSensitiveData").Single().Value);
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "SensitiveData").Single().Value);
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "KeyVersion").Single().Value);
    }
}
