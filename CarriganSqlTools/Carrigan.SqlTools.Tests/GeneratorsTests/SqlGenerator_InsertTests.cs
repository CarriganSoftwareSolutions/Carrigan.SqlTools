using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Sets;
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

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@Id_1").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_2").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_3").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_4").Single().Value); // When
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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([Name], [DateOf], [When])", "VALUES (@Name_1, @DateOf_2, @When_3);", "UNIQUEIDENTIFIER");

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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([Name], [DateOf], [When])", "VALUES (@Name_1, @DateOf_2, @When_3);", "UNIQUEIDENTIFIER");

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_1").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_2").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_3").Single().Value); // WhenF"
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

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@Id_1").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_2").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_3").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_4").Single().Value); // When


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


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(null, null, testEntity);

        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@Name_2").Single().Value); // Name
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@When_4").Single().Value); // When
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

        int expectedInt = 1;
        int actualInt = (int)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "@Id_1").Single()];
        Assert.Equal(expectedInt, actualInt);

        expectedInt = 2;
        actualInt = (int)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "@Id_3").Single()];
        Assert.Equal(expectedInt, actualInt);

        string expectedString = "1a";
        string actualString = (string)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "@Description_2").Single()];
        Assert.Equal(expectedString, actualString);

        expectedString = "2b";
        actualString = (string)query.Parameters[query.Parameters.Keys.Where(parameter => parameter == "@Description_4").Single()];
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

        Assert.Equal(4, query.Parameters.Count);


        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
    }

    [Fact]
    public void SqlInsertString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();

        SqlQuery query = _sqlGeneratorForSqlTypeEntity.Insert(null, null, entity);

        string expectedSql = "INSERT INTO [TestSqlTypes] ([IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [StringValue], [DateTimeValue], [GuidValue], [ByteArrayValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [DateTimeOffsetValue]) VALUES (@IntValue_1, @LongValue_2, @ShortValue_3, @ByteValue_4, @BoolValue_5, @DecimalValue_6, @FloatValue_7, @DoubleValue_8, @StringValue_9, @DateTimeValue_10, @GuidValue_11, @ByteArrayValue_12, @CharValue_13, @TimeOnlyValue_14, @DateOnlyValue_15, @DateTimeOffsetValue_16);";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(16, query.Parameters.Count);

        Assert.Equal(42, query.Parameters.Where(param => param.Key == "@IntValue_1").Single().Value);                                                // IntValue
        Assert.Equal(1234567890L, query.Parameters.Where(param => param.Key == "@LongValue_2").Single().Value);                                      // LongValue
        Assert.Equal((short)32000, query.Parameters.Where(param => param.Key == "@ShortValue_3").Single().Value);                                    // ShortValue
        Assert.Equal((byte)255, query.Parameters.Where(param => param.Key == "@ByteValue_4").Single().Value);                                        // ByteValue
        Assert.Equal(true, query.Parameters.Where(param => param.Key == "@BoolValue_5").Single().Value);                                             // BoolValue
        Assert.Equal(99.99m, query.Parameters.Where(param => param.Key == "@DecimalValue_6").Single().Value);                                        // DecimalValue
        Assert.Equal(3.14f, query.Parameters.Where(param => param.Key == "@FloatValue_7").Single().Value);                                           // FloatValue
        Assert.Equal(123.456, query.Parameters.Where(param => param.Key == "@DoubleValue_8").Single().Value);                                        // DoubleValue
        Assert.Equal("Test String", query.Parameters.Where(param => param.Key == "@StringValue_9").Single().Value);                                  // StringValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "@DateTimeValue_10").Single().Value);    // DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@GuidValue_11").Single().Value); // GuidValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "@ByteArrayValue_12").Single().Value);             // ByteArrayValue
        Assert.Equal('A', query.Parameters.Where(param => param.Key == "@CharValue_13").Single().Value);                                              // CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "@TimeOnlyValue_14").Single().Value);                        // TimeOnlyValue
        Assert.Equal(new DateOnly(1776, 7, 4), query.Parameters.Where(param => param.Key == "@DateOnlyValue_15").Single().Value);                     // DateOnlyValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_16").Single().Value);                // DateTimeOffsetValue
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
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "@Key_1").Single().Value);                                             // Key
        Assert.Equal((int?)1, query.Parameters.Where(param => param.Key == "@IntValue_2").Single().Value);                                           // Nullable IntValue
        Assert.Equal((long?)123456789L, query.Parameters.Where(param => param.Key == "@LongValue_3").Single().Value);                                // Nullable LongValue
        Assert.Equal((short?)123, query.Parameters.Where(param => param.Key == "@ShortValue_4").Single().Value);                                     // Nullable ShortValue
        Assert.Equal((byte?)255, query.Parameters.Where(param => param.Key == "@ByteValue_5").Single().Value);                                       // Nullable ByteValue
        Assert.Equal((bool?)true, query.Parameters.Where(param => param.Key == "@BoolValue_6").Single().Value);                                      // Nullable BoolValue
        Assert.Equal((decimal?)99.99m, query.Parameters.Where(param => param.Key == "@DecimalValue_7").Single().Value);                              // Nullable DecimalValue
        Assert.Equal((float?)3.14f, query.Parameters.Where(param => param.Key == "@FloatValue_8").Single().Value);                                   // Nullable FloatValue
        Assert.Equal((double?)1.618, query.Parameters.Where(param => param.Key == "@DoubleValue_9").Single().Value);                                 // Nullable DoubleValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "@DateTimeValue_10").Single().Value);    // Nullable DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@GuidValue_11").Single().Value); // Nullable GuidValue
        Assert.Equal((char?)'A', query.Parameters.Where(param => param.Key == "@CharValue_12").Single().Value);                                       // Nullable CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "@TimeOnlyValue_13").Single().Value);                        // Nullable TimeOnlyValue
        Assert.Equal(new DateOnly(1, 12, 25), query.Parameters.Where(param => param.Key == "@DateOnlyValue_14").Single().Value);                      // Nullable DateOnlyValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "@ByteArrayValue_15").Single().Value);             // Nullable ByteArrayValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_16").Single().Value);                // Nullable DateTimeOffsetValue

    }


    [Fact]
    public void TestSqlInsertStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();

        SqlQuery query = _sqlGeneratorForNullableTestEntity.Insert(null, null, entity);

        string expectedSql = "INSERT INTO [NullableTestEntity] ([Key], [IntValue], [LongValue], [ShortValue], [ByteValue], [BoolValue], [DecimalValue], [FloatValue], [DoubleValue], [DateTimeValue], [GuidValue], [CharValue], [TimeOnlyValue], [DateOnlyValue], [ByteArrayValue], [DateTimeOffsetValue]) VALUES (@Key_1, @IntValue_2, @LongValue_3, @ShortValue_4, @ByteValue_5, @BoolValue_6, @DecimalValue_7, @FloatValue_8, @DoubleValue_9, @DateTimeValue_10, @GuidValue_11, @CharValue_12, @TimeOnlyValue_13, @DateOnlyValue_14, @ByteArrayValue_15, @DateTimeOffsetValue_16);";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "@Key_1").Single().Value);                   // Key
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@IntValue_2").Single().Value);            // Nullable IntValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@LongValue_3").Single().Value);           // Nullable LongValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ShortValue_4").Single().Value);          // Nullable ShortValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ByteValue_5").Single().Value);           // Nullable ByteValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@BoolValue_6").Single().Value);           // Nullable BoolValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DecimalValue_7").Single().Value);        // Nullable DecimalValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@FloatValue_8").Single().Value);          // Nullable FloatValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DoubleValue_9").Single().Value);         // Nullable DoubleValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateTimeValue_10").Single().Value);       // Nullable DateTimeValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@GuidValue_11").Single().Value);           // Nullable GuidValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@CharValue_12").Single().Value);           // Nullable CharValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@TimeOnlyValue_13").Single().Value);       // Nullable TimeOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateOnlyValue_14").Single().Value);       // Nullable DateOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ByteArrayValue_15").Single().Value);      // Nullable ByteArrayValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_16").Single().Value); // Nullable DateTimeOffsetValue
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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([NotSensitiveData], [SensitiveData], [KeyVersion])", "VALUES (@NotSensitiveData_1, @SensitiveData_2, @KeyVersion_3);", "INT");

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "@NotSensitiveData_1").Single().Value);
        Assert.NotEqual("Shhh...", query.Parameters.Where(param => param.Key == "@SensitiveData_2").Single().Value);
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(query.Parameters.Where(param => param.Key == "@SensitiveData_2").Single().Value.ToString()));
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "@KeyVersion_3").Single().Value);
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

        string expectedSql = ModifyInsertQueryWithReturn("INSERT INTO [Test] ([NotSensitiveData], [SensitiveData], [KeyVersion])", "VALUES (@NotSensitiveData_1, @SensitiveData_2, @KeyVersion_3);", "INT");

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(3, query.Parameters.Count);
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "@NotSensitiveData_1").Single().Value);
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@SensitiveData_2").Single().Value);
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "@KeyVersion_3").Single().Value);
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

        Assert.Equal(3, query.Parameters.Count);


        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain("DateOf", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf");
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

        Assert.Equal(6, query.Parameters.Count);


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

        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address_0");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf_0");
        Assert.Contains(query.Parameters, param => param.Key == "@Id_1");
        Assert.Contains(query.Parameters, param => param.Key == "@When_3");
        Assert.Contains(query.Parameters, param => param.Key == "@Name_2");

        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address_1");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf_1");
        Assert.Contains(query.Parameters, param => param.Key == "@Id_4");
        Assert.Contains(query.Parameters, param => param.Key == "@When_6");
        Assert.Contains(query.Parameters, param => param.Key == "@Name_5");
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
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER, DateOf DATETIME2);");
        queryBuilder.AppendLine("INSERT INTO [Test] ([Name], [When])");
        queryBuilder.AppendLine("OUTPUT INSERTED.Id, INSERTED.DateOf INTO @OutputTable");
        queryBuilder.AppendLine("VALUES (@Name_1, @When_2), (@Name_3, @When_4);");
        queryBuilder.AppendLine("SELECT Id, DateOf FROM @OutputTable;");

        Assert.Equal(queryBuilder.ToString(), query.QueryText);

        Assert.Equal(4, query.Parameters.Count);


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

        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address_0");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf_0");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Id_0");
        Assert.Contains(query.Parameters, param => param.Key == "@When_2");
        Assert.Contains(query.Parameters, param => param.Key == "@Name_1");

        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address_1");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf_1");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Id_1");
        Assert.Contains(query.Parameters, param => param.Key == "@When_4");
        Assert.Contains(query.Parameters, param => param.Key == "@Name_3");
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
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id1 INT, Id2 INT);");
        queryBuilder.AppendLine("INSERT INTO [Ck] ([NotKey1], [NotKey2], [NotKey3])");
        queryBuilder.AppendLine("OUTPUT INSERTED.Id1, INSERTED.Id2 INTO @OutputTable");
        queryBuilder.AppendLine("VALUES (@NotKey1_1, @NotKey2_2, @NotKey3_3), (@NotKey1_4, @NotKey2_5, @NotKey3_6);");
        queryBuilder.AppendLine("SELECT Id1, Id2 FROM @OutputTable;");

        Assert.Equal(queryBuilder.ToString(), query.QueryText);

        Assert.Equal(6, query.Parameters.Count);

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

        Assert.DoesNotContain(query.Parameters, param => param.Key == "Id1_0");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Id2_0");
        Assert.Contains(query.Parameters, param => param.Key == "@NotKey1_1");
        Assert.Contains(query.Parameters, param => param.Key == "@NotKey2_2");
        Assert.Contains(query.Parameters, param => param.Key == "@NotKey3_3");

        Assert.DoesNotContain(query.Parameters, param => param.Key == "Id1_1");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Id2_1");
        Assert.Contains(query.Parameters, param => param.Key == "@NotKey1_4");
        Assert.Contains(query.Parameters, param => param.Key == "@NotKey2_5");
        Assert.Contains(query.Parameters, param => param.Key == "@NotKey3_6");
    }
}