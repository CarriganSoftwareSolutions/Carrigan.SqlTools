using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Query;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;


namespace Carrigan.SqlTools.Tests.GeneratorsTests;


//IGNORE SPELLING: SHHH myschema

public class SqlGenerator_UpdateByIdTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;
    private readonly SqlGenerator<SqlTypeEntity> _sqlGeneratorForSqlTypeEntity;
    private readonly SqlGenerator<NullableTestEntity> _sqlGeneratorForNullablesTestEntity;
    private readonly SqlGenerator<EntityWithEncryption> _sqlGeneratorForEntityWithEncryption;
    private readonly SqlGenerator<CompositeKeyTable> _sqlGeneratorCompositeKeyTable;
    private readonly SetColumns<CompositeKeyTable> _leftCompositeKeyTable = new("NotKey1", "NotKey2");

    public SqlGenerator_UpdateByIdTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncrypter);
        _sqlGeneratorForSqlTypeEntity = new SqlGenerator<SqlTypeEntity>(_mockEncrypter);
        _sqlGeneratorForNullablesTestEntity = new SqlGenerator<NullableTestEntity>(_mockEncrypter);
        _sqlGeneratorForEntityWithEncryption = new SqlGenerator<EntityWithEncryption>(_mockEncrypter);
        _sqlGeneratorCompositeKeyTable = new SqlGenerator<CompositeKeyTable>(_mockEncrypter);
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectSql_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name, [DateOf] = @DateOf, [When] = @When WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectSql_WithTableAttribute_No_Optional_Columns_Param()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name, [DateOf] = @DateOf, [When] = @When WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectParameters()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "DateOf").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "When").Single().Value); // When
    }

    [Fact]
    public void SqlUpdateString_ExcludesNotMappedProperties()
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

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

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
    public void SqlUpdateString_HandlesNullValues()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = null!, // Nullable property
            When = null, // Nullable property
            DateOf = DateTime.UtcNow
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);


        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "Name").Single().Value); // Name
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "When").Single().Value); // When
    }

    [Fact]
    public void SqlUpdateString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.UpdateById(entityWithoutTableAttribute, null);

        string expectedSql = "UPDATE [EntityWithoutTableAttribute] SET [Description] = @Description WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlUpdateString_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithSchema.UpdateById(entityWithSchema, null);

        string expectedSql = "UPDATE [myschema].[EntityWithSchema] SET [Description] = @Description WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);
    }


    [Fact]
    public void SqlUpdateString_IgnoresClassTypeProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1),
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };


        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.UpdateById(testEntity, null);

        string expectedSql = "UPDATE [Test] SET [Name] = @Name, [DateOf] = @DateOf, [When] = @When WHERE [Id] = @Id;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.DoesNotContain("Address", query.QueryText);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
    }

    [Fact]
    public void SqlUpdatesString_GeneratesCorrectSqlForAllSupportedTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = SqlTypeEntity.DateTimeOffsetTestValue;
        SqlTypeEntity entity = SqlTypeEntity.GetStandardTestSet();


        SqlQuery query  = _sqlGeneratorForSqlTypeEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [TestSqlTypes] SET [LongValue] = @LongValue, [ShortValue] = @ShortValue, [ByteValue] = @ByteValue, [BoolValue] = @BoolValue, [DecimalValue] = @DecimalValue, [FloatValue] = @FloatValue, [DoubleValue] = @DoubleValue, [StringValue] = @StringValue, [DateTimeValue] = @DateTimeValue, [GuidValue] = @GuidValue, [ByteArrayValue] = @ByteArrayValue, [CharValue] = @CharValue, [TimeOnlyValue] = @TimeOnlyValue, [DateOnlyValue] = @DateOnlyValue, [DateTimeOffsetValue] = @DateTimeOffsetValue WHERE [IntValue] = @IntValue;";
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
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value);                // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlUpdateStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();


        SqlQuery query
            = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [NullableTestEntity] SET [IntValue] = @IntValue, [LongValue] = @LongValue, [ShortValue] = @ShortValue, [ByteValue] = @ByteValue, [BoolValue] = @BoolValue, [DecimalValue] = @DecimalValue, [FloatValue] = @FloatValue, [DoubleValue] = @DoubleValue, [DateTimeValue] = @DateTimeValue, [GuidValue] = @GuidValue, [CharValue] = @CharValue, [TimeOnlyValue] = @TimeOnlyValue, [DateOnlyValue] = @DateOnlyValue, [ByteArrayValue] = @ByteArrayValue, [DateTimeOffsetValue] = @DateTimeOffsetValue WHERE [Key] = @Key;";
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
    public void TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();


        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [NullableTestEntity] SET [IntValue] = @IntValue, [LongValue] = @LongValue, [ShortValue] = @ShortValue, [ByteValue] = @ByteValue, [BoolValue] = @BoolValue, [DecimalValue] = @DecimalValue, [FloatValue] = @FloatValue, [DoubleValue] = @DoubleValue, [DateTimeValue] = @DateTimeValue, [GuidValue] = @GuidValue, [CharValue] = @CharValue, [TimeOnlyValue] = @TimeOnlyValue, [DateOnlyValue] = @DateOnlyValue, [ByteArrayValue] = @ByteArrayValue, [DateTimeOffsetValue] = @DateTimeOffsetValue WHERE [Key] = @Key;";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "Key").Single().Value);             // Key
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "IntValue").Single().Value);      // Nullable IntValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "LongValue").Single().Value);     // Nullable LongValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ShortValue").Single().Value);    // Nullable ShortValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ByteValue").Single().Value);     // Nullable ByteValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "BoolValue").Single().Value);     // Nullable BoolValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DecimalValue").Single().Value);  // Nullable DecimalValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "FloatValue").Single().Value);    // Nullable FloatValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DoubleValue").Single().Value);   // Nullable DoubleValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateTimeValue").Single().Value); // Nullable DateTimeValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "GuidValue").Single().Value);     // Nullable GuidValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "CharValue").Single().Value);     // Nullable CharValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "TimeOnlyValue").Single().Value); // Nullable TimeOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateOnlyValue").Single().Value); // Nullable DateOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "ByteArrayValue").Single().Value);    // Nullable ByteArrayValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "DateTimeOffsetValue").Single().Value); // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void SqlUpdateString_Encrypted()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = "Shhh..."
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.UpdateById(entity, null);

        string expectedSql = "UPDATE [Test] SET [NotSensitiveData] = @NotSensitiveData, [SensitiveData] = @SensitiveData, [KeyVersion] = @KeyVersion WHERE [Id] = @Id;";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(1337, query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "NotSensitiveData").Single().Value);
        Assert.NotEqual("Shhh...", query.Parameters.Where(param => param.Key == "SensitiveData").Single().Value);
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(query.Parameters.Where(param => param.Key == "SensitiveData").Single().Value.ToString()));
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "KeyVersion").Single().Value);
    }

    [Fact]
    public void SqlUpdateString_Encrypted_Null()
    {
        EntityWithEncryption entity = new()
        {
            Id = 1337,
            NotSensitiveData = "SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...",
            SensitiveData = null
        };

        SqlQuery query = _sqlGeneratorForEntityWithEncryption.UpdateById(entity, null);

        string expectedSql = "UPDATE [Test] SET [NotSensitiveData] = @NotSensitiveData, [SensitiveData] = @SensitiveData, [KeyVersion] = @KeyVersion WHERE [Id] = @Id;";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(1337, query.Parameters.Where(param => param.Key == "Id").Single().Value); // Id
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "NotSensitiveData").Single().Value);
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "SensitiveData").Single().Value);
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "KeyVersion").Single().Value);
    }

    [Fact]
    public void SqlUpdate_WithInnerJoin_WithJoinsAndPredicates()
    {

        CompositeKeyTable entity3 = new()
        {
            Id1 = 11,
            Id2 = 12,

            NotKey1 = 13,
            NotKey2 = 14
        };

        CompositeKeyTable entity1 = new()
        {
            Id1 = 1,
            Id2 = 2,

            NotKey1 = 3,
            NotKey2 = 4
        };

        CompositeKeyTable entity2 = new()
        {
            Id1 = 5,
            Id2 = 6,

            NotKey1 = 7,
            NotKey2 = 8
        };

        PredicatesBase predicateId = new Equal(new Columns<JoinLeftTable>("Id"), new Parameters("Id", 3));
        SqlQuery query = _sqlGeneratorCompositeKeyTable.UpdateByIds(entity3, _leftCompositeKeyTable, entity1, entity2);

        string expectedSql = "UPDATE [Ck] SET [Ck].[NotKey1] = @ParameterSet_NotKey1, [Ck].[NotKey2] = @ParameterSet_NotKey2 FROM [Ck] WHERE " +
            "((([Ck].[Id1] = @Parameter_0_0_R_Id1) AND ([Ck].[Id2] = @Parameter_0_1_R_Id2)) OR (([Ck].[Id1] = @Parameter_1_0_R_Id1) AND ([Ck].[Id2] = @Parameter_1_1_R_Id2)))";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 6;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        int expectedValue = 13;
        int actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@ParameterSet_NotKey1").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 14;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@ParameterSet_NotKey2").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 1;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Parameter_0_0_R_Id1").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 2;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Parameter_0_1_R_Id2").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 5;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Parameter_1_0_R_Id1").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 6;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Parameter_1_1_R_Id2").Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }
}
