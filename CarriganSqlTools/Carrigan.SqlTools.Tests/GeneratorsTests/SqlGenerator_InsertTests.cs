using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.Helpers;
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
    private readonly SqlGenerator<CompositePrimaryKeyTable> _sqlGeneratorForCompositePrimaryKeyTable;

    public SqlGenerator_InsertTests()
    {
        _mockEncrypter = new("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new();
        _sqlGeneratorForEntityWithoutTableAttribute = new();
        _sqlGeneratorForEntityWithSchema = new();
        _sqlGeneratorForSqlTypeEntity = new();
        _sqlGeneratorForNullableTestEntity = new();
        _sqlGeneratorForEntityWithEncryption = new(_mockEncrypter);
        _sqlGeneratorForCompositePrimaryKeyTable = new();
    }

    private static string ModifyInsertQueryWithReturn(string queryPart1, string queryPart2, string type)
    {
        StringBuilder queryBuilder = new();
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id {type});");
        queryBuilder.AppendLine(queryPart1);
        queryBuilder.AppendLine("OUTPUT INSERTED.Id INTO @OutputTable");
        queryBuilder.AppendLine(queryPart2);
        queryBuilder.AppendLine("SELECT Id FROM @OutputTable;");
        return queryBuilder.ToString();
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

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(null, null, testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id_1, @Name_2, @DateOf_3, @When_4);";

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

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(null, null, testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id_1, @Name_2, @DateOf_3, @When_4);";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"));
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Test Name");
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_3", new DateTime(2023, 10, 1));
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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([Name], [DateOf], [When])", "VALUES (@Name_1, @DateOf_2, @When_3);", "UNIQUEIDENTIFIER NOT NULL");

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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([Name], [DateOf], [When])", "VALUES (@Name_1, @DateOf_2, @When_3);", "UNIQUEIDENTIFIER NOT NULL");

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Test Name");
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_2", new DateTime(2023, 10, 1));
        SqlQueryTestHelper.AssertParameterValue(query, "@When_3", "Now");
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


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(null, null, testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id_1, @Name_2, @DateOf_3, @When_4);";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"));
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Test Name");
        SqlQueryTestHelper.AssertParameterValue(query, "@DateOf_3", new DateTime(2023, 10, 1));
        SqlQueryTestHelper.AssertParameterValue(query, "@When_4", "Now");


        Assert.DoesNotContain("Where", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Where");
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "HideTimeFlag");
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


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(null, null, testEntity);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", null!);

        SqlQueryTestHelper.AssertParameterValue(query, "@When_4", null!);
    }

    [Fact]
    public void SqlInsertString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.Insert(null, null, entityWithoutTableAttribute);

        string expectedSql = "INSERT INTO [EntityWithoutTableAttribute] ([Id], [Description]) VALUES (@Id_1, @Description_2);";
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

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.Insert(null, null, [entity1, entity2]);
        string expectedSql = "INSERT INTO [EntityWithoutTableAttribute] ([Id], [Description]) VALUES (@Id_1, @Description_2), (@Id_3, @Description_4);";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Description_2", "1a");
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@Description_4", "2b");
    }

    [Fact]
    public void SqlInsertString_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithSchema.Insert(null, null, entityWithSchema);

        string expectedSql = "INSERT INTO [myschema].[EntityWithSchema] ([Id], [Description]) VALUES (@Id_1, @Description_2);";

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


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(null, null, testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [DateOf], [When]) VALUES (@Id_1, @Name_2, @DateOf_3, @When_4);";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);


        Assert.DoesNotContain("Address", query.QueryText);

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address");
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();

        SqlQuery query = _sqlGeneratorForSqlTypeEntity.Insert(null, null, entity);

        string expectedSql = "INSERT INTO [TestSqlTypes] ([IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [StringValue], [DateTimeValue], [GuidValue], [ByteArrayValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [DateTimeOffsetValue]) VALUES (@IntValue_1, @LongValue_2, @ShortValue_3, @ByteValue_4, @BoolValue_5, @DecimalValue_6, @FloatValue_7, @DoubleValue_8, @StringValue_9, @DateTimeValue_10, @GuidValue_11, @ByteArrayValue_12, @CharValue_13, @TimeOnlyValue_14, @DateOnlyValue_15, @DateTimeOffsetValue_16);";
        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 16);

        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_1", 42);                                             // IntValue

        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_2", 1234567890L);                                 // LongValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_3", (short)32000);                                    // ShortValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_4", (byte)255);                                      // ByteValue

        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_5", true);                                        // BoolValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_6", 99.99m);                                      // DecimalValue

        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_7", 3.14f);                                         // FloatValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_8", 123.456);                                     // DoubleValue

        SqlQueryTestHelper.AssertParameterValue(query, "@StringValue_9", "Test String");                                 // StringValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_10", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));    // DateTimeValue

        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_11", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // GuidValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_12", new byte[] { 0x01, 0x02, 0x03 });          // ByteArrayValue

        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_13", 'A');                                              // CharValue

        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_14", new TimeOnly(1, 2, 0));                        // TimeOnlyValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_15", new DateOnly(1776, 7, 4));                     // DateOnlyValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_16", dateTimeOffsetTestValue);               // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlInsertStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();

        SqlQuery query = _sqlGeneratorForNullableTestEntity.Insert(null, null, entity);

        string expectedSql = "INSERT INTO [NullableTestEntity] ([Key], [IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [DateTimeValue], [GuidValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [ByteArrayValue], [DateTimeOffsetValue]) VALUES (@Key_1, @IntValue_2, @LongValue_3, @ShortValue_4, @ByteValue_5, @BoolValue_6, @DecimalValue_7, @FloatValue_8, @DoubleValue_9, @DateTimeValue_10, @GuidValue_11, @CharValue_12, @TimeOnlyValue_13, @DateOnlyValue_14, @ByteArrayValue_15, @DateTimeOffsetValue_16);";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_1", Guid.Empty);                                            // Key

        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_2", (int?)1);                                           // Nullable IntValue

        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_3", (long?)123456789L);                                // Nullable LongValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_4", (short?)123);                                     // Nullable ShortValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_5", (byte?)255);                                     // Nullable ByteValue

        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_6", (bool?)true);;                                      // Nullable BoolValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_7", (decimal?)99.99m);                              // Nullable DecimalValue

        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_8", (float?)3.14f);                                   // Nullable FloatValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_9", (double?)1.618);                                // Nullable DoubleValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_10", new DateTime(2024, 11, 6, 1, 14, 1, 2, 3));   // Nullable DateTimeValue

        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_11", new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695")); // Nullable GuidValue

        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_12", (char?)'A');                                     // Nullable CharValue

        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_13", new TimeOnly(1, 2, 0));                       // Nullable TimeOnlyValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_14", new DateOnly(1, 12, 25));                     // Nullable DateOnlyValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_15", new byte[] { 0x01, 0x02, 0x03 });             // Nullable ByteArrayValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_16", dateTimeOffsetTestValue);               // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void TestSqlInsertStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();

        SqlQuery query = _sqlGeneratorForNullableTestEntity.Insert(null, null, entity);

        string expectedSql = "INSERT INTO [NullableTestEntity] ([Key], [IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [DateTimeValue], [GuidValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [ByteArrayValue], [DateTimeOffsetValue]) VALUES (@Key_1, @IntValue_2, @LongValue_3, @ShortValue_4, @ByteValue_5, @BoolValue_6, @DecimalValue_7, @FloatValue_8, @DoubleValue_9, @DateTimeValue_10, @GuidValue_11, @CharValue_12, @TimeOnlyValue_13, @DateOnlyValue_14, @ByteArrayValue_15, @DateTimeOffsetValue_16);";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        SqlQueryTestHelper.AssertParameterValue(query, "@Key_1", Guid.Empty);                   // Key

        SqlQueryTestHelper.AssertParameterValue(query, "@IntValue_2", null!);;            // Nullable IntValue

        SqlQueryTestHelper.AssertParameterValue(query, "@LongValue_3", null!);         // Nullable LongValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ShortValue_4", null!);          // Nullable ShortValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ByteValue_5", null!);           // Nullable ByteValue

        SqlQueryTestHelper.AssertParameterValue(query, "@BoolValue_6", null!);          // Nullable BoolValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DecimalValue_7", null!);        // Nullable DecimalValue

        SqlQueryTestHelper.AssertParameterValue(query, "@FloatValue_8", null!);          // Nullable FloatValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DoubleValue_9", null!);         // Nullable DoubleValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeValue_10", null!);       // Nullable DateTimeValue

        SqlQueryTestHelper.AssertParameterValue(query, "@GuidValue_11", null!);           // Nullable GuidValue

        SqlQueryTestHelper.AssertParameterValue(query, "@CharValue_12", null!);           // Nullable CharValue

        SqlQueryTestHelper.AssertParameterValue(query, "@TimeOnlyValue_13", null!);       // Nullable TimeOnlyValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateOnlyValue_14", null!);       // Nullable DateOnlyValue

        SqlQueryTestHelper.AssertParameterValue(query, "@ByteArrayValue_15", null!);      // Nullable ByteArrayValue

        SqlQueryTestHelper.AssertParameterValue(query, "@DateTimeOffsetValue_16", null!); // Nullable DateTimeOffsetValue
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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([NotSensitiveData], [SensitiveData], [KeyVersion])", "VALUES (@NotSensitiveData_1, @SensitiveData_2, @KeyVersion_3);", "INT NOT NULL");

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 3);

        SqlQueryTestHelper.AssertParameterValue(query, "@NotSensitiveData_1", "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...");

        object? sensitiveDataValue = SqlQueryTestHelper.GetParameterValue(query, "@SensitiveData_2");
        Assert.NotEqual("Shhh...", sensitiveDataValue);

        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(sensitiveDataValue?.ToString()));

        SqlQueryTestHelper.AssertParameterValue(query, "@KeyVersion_3", 1);
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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([NotSensitiveData], [SensitiveData], [KeyVersion])", "VALUES (@NotSensitiveData_1, @SensitiveData_2, @KeyVersion_3);", "INT NOT NULL");

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 3);
        //Assert.Equal(3, query.Parameters.Count);

        SqlQueryTestHelper.AssertParameterValue(query, "@NotSensitiveData_1", "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...");

        SqlQueryTestHelper.AssertParameterValue(query, "@SensitiveData_2", null!); //conversion to DBNull is now done in the commend code.

        SqlQueryTestHelper.AssertParameterValue(query, "@KeyVersion_3", 1);
    }

    [Fact]
    public void Insert_ColumnCollection()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1), // Should be ignored
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };

        ColumnCollection<EntityWithTableAttribute> insertColumns = new("Id", "Name", "When");


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(insertColumns, null, testEntity);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [When]) VALUES (@Id_1, @Name_2, @When_3);";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 3);


        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain("DateOf", query.QueryText);

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "@Address");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "@DateOf");
        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "DateOf");
    }

    [Fact]
    public void Insert_ColumnCollection_Multiple()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1), // Should be ignored
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };
        EntityWithTableAttribute testEntity2 = new()
        {
            Name = "Test Name2",
            DateOf = new DateTime(2025, 12, 6), // Should be ignored
            When = "Now",
            Address = new Address { Street = "123 Fake St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };

        ColumnCollection<EntityWithTableAttribute> insertColumns = new("Id", "Name", "When");


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(insertColumns, null, testEntity, testEntity2);

        string expectedSql = "INSERT INTO [Test] ([Id], [Name], [When]) VALUES (@Id_1, @Name_2, @When_3), (@Id_4, @Name_5, @When_6);";

        Assert.Equal(expectedSql, query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 6);


        Assert.DoesNotContain("Address_0", query.QueryText);
        Assert.DoesNotContain("DateOf_0", query.QueryText);
        Assert.Contains("@Id_1", query.QueryText);
        Assert.Contains("@Name_2", query.QueryText);
        Assert.Contains("@When_3", query.QueryText);

        Assert.DoesNotContain("Address_1", query.QueryText);
        Assert.DoesNotContain("DateOf_1", query.QueryText);
        Assert.Contains("@Id_4", query.QueryText);
        Assert.Contains("@Name_5", query.QueryText);
        Assert.Contains("@When_6", query.QueryText);

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "@Address_0");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "D@ateOf_0");
        //Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf_0");

        SqlQueryTestHelper.AssertParameterExists(query, "@Id_1");

        SqlQueryTestHelper.AssertParameterExists(query, "@When_3");

        SqlQueryTestHelper.AssertParameterExists(query, "@Name_2");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address_1");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "DateOf_1");

        SqlQueryTestHelper.AssertParameterExists(query, "@Id_4");

        SqlQueryTestHelper.AssertParameterExists(query, "@When_6");

        SqlQueryTestHelper.AssertParameterExists(query, "@Name_5");
    }

    [Fact]
    public void Insert_ColumnCollection_Multiple_Return_Multiple()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1), // Should be ignored
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };
        EntityWithTableAttribute testEntity2 = new()
        {
            Name = "Test Name2",
            DateOf = new DateTime(2025, 12, 6), // Should be ignored
            When = "Now",
            Address = new Address { Street = "123 Fake St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };

        ColumnCollection<EntityWithTableAttribute> insertColumns = new("Name", "When");
        ColumnCollection<EntityWithTableAttribute> returnColumns = new("Id", "DateOf");

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(insertColumns, returnColumns, testEntity, testEntity2);

        StringBuilder queryBuilder = new();
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER NOT NULL, DateOf DATETIME2 NOT NULL);");
        queryBuilder.AppendLine("INSERT INTO [Test] ([Name], [When])");
        queryBuilder.AppendLine("OUTPUT INSERTED.Id, INSERTED.DateOf INTO @OutputTable");
        queryBuilder.AppendLine("VALUES (@Name_1, @When_2), (@Name_3, @When_4);");
        queryBuilder.AppendLine("SELECT Id, DateOf FROM @OutputTable;");

        Assert.Equal(queryBuilder.ToString(), query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 4);


        Assert.DoesNotContain("Address_0", query.QueryText);
        Assert.DoesNotContain("DateOf_0", query.QueryText);
        Assert.DoesNotContain("Id_0", query.QueryText);
        Assert.Contains("@Name_1", query.QueryText);
        Assert.Contains("@When_2", query.QueryText);

        Assert.DoesNotContain("Address_1", query.QueryText);
        Assert.DoesNotContain("DateOf_1", query.QueryText);
        Assert.DoesNotContain("Id_1", query.QueryText);
        Assert.Contains("@Name_3", query.QueryText);
        Assert.Contains("@When_4", query.QueryText);

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address_0");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "DateOf_0");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Id_0");

        SqlQueryTestHelper.AssertParameterExists(query, "@When_2");

        SqlQueryTestHelper.AssertParameterExists(query, "@Name_1");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Address_1");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "DateOf_1");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Id_1");

        SqlQueryTestHelper.AssertParameterExists(query, "@When_4");

        SqlQueryTestHelper.AssertParameterExists(query, "@Name_3");
    }

    [Fact]
    public void Insert_AutoId_CompositePrimaryKeyTable()
    {
        CompositePrimaryKeyTable testEntity = new()
        {
            NotKey1 = 1,
            NotKey2 = 2,
            NotKey3 = 3
        };
        CompositePrimaryKeyTable testEntity2 = new()
        {
            NotKey1 = 1,
            NotKey2 = 2,
            NotKey3 = 3
        };

        SqlQuery query = _sqlGeneratorForCompositePrimaryKeyTable.InsertAutoId(testEntity, testEntity2);

        StringBuilder queryBuilder = new();
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id1 INT NOT NULL, Id2 INT NOT NULL);");
        queryBuilder.AppendLine("INSERT INTO [Ck] ([NotKey1], [NotKey2], [NotKey3])");
        queryBuilder.AppendLine("OUTPUT INSERTED.Id1, INSERTED.Id2 INTO @OutputTable");
        queryBuilder.AppendLine("VALUES (@NotKey1_1, @NotKey2_2, @NotKey3_3), (@NotKey1_4, @NotKey2_5, @NotKey3_6);");
        queryBuilder.AppendLine("SELECT Id1, Id2 FROM @OutputTable;");

        Assert.Equal(queryBuilder.ToString(), query.QueryText);

        SqlQueryTestHelper.AssertParameterCount(query, 6);

        Assert.DoesNotContain("Id1_0", query.QueryText);
        Assert.DoesNotContain("Id2_0", query.QueryText);
        Assert.Contains("@NotKey1_1", query.QueryText);
        Assert.Contains("@NotKey2_2", query.QueryText);
        Assert.Contains("@NotKey3_3", query.QueryText);

        Assert.DoesNotContain("Id1_1", query.QueryText);
        Assert.DoesNotContain("Id2_1", query.QueryText);
        Assert.Contains("@NotKey1_4", query.QueryText);
        Assert.Contains("@NotKey2_5", query.QueryText);
        Assert.Contains("@NotKey3_6", query.QueryText);

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Id1_0");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Id2_0");

        SqlQueryTestHelper.AssertParameterExists(query, "@NotKey1_1");

        SqlQueryTestHelper.AssertParameterExists(query, "@NotKey2_2");

        SqlQueryTestHelper.AssertParameterExists(query, "@NotKey3_3");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Id1_1");

        SqlQueryTestHelper.AssertParameterDoesNotExist(query, "Id2_1");

        SqlQueryTestHelper.AssertParameterExists(query, "@NotKey1_4");

        SqlQueryTestHelper.AssertParameterExists(query, "@NotKey2_5");

        SqlQueryTestHelper.AssertParameterExists(query, "@NotKey3_6");
    }

    [Fact]
    public void InsertAutoId_NullNullableGuidParameter_RetainsColumnFieldProperties()
    {
        NullableTestEntity testEntity = NullableTestEntity.GetNullTestSet();

        SqlQuery query = _sqlGeneratorForNullableTestEntity.InsertAutoId(testEntity);

        SqlFragmentParameter parameter =
            query.Parameters.Single(parameter => parameter.ParameterTag.ToString().StartsWith("@GuidValue_"));

        Assert.Null(parameter.Value);
        Assert.NotNull(parameter.FieldProperties);
        Assert.Equal("UNIQUEIDENTIFIER", parameter.FieldProperties.ProviderTypeName);
        Assert.True(parameter.FieldProperties.IsNullable);
    }
}