using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
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
    private readonly SqlGenerator<CompositePrimaryKeyTable> _sqlGeneratorCompositeKeyTable;
    private readonly ColumnCollection<CompositePrimaryKeyTable> _leftCompositeKeyTable = new("NotKey1", "NotKey2");
    private readonly SqlGenerator<Address> _sqlGeneratorAddress;

    public SqlGenerator_UpdateByIdTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncrypter);
        _sqlGeneratorForSqlTypeEntity = new SqlGenerator<SqlTypeEntity>(_mockEncrypter);
        _sqlGeneratorForNullablesTestEntity = new SqlGenerator<NullableTestEntity>(_mockEncrypter);
        _sqlGeneratorForEntityWithEncryption = new SqlGenerator<EntityWithEncryption>(_mockEncrypter);
        _sqlGeneratorCompositeKeyTable = new SqlGenerator<CompositePrimaryKeyTable>(_mockEncrypter);
        _sqlGeneratorAddress = new();
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

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2, [When] = @When_3 WHERE [Id] = @Id_4;";
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

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2, [When] = @When_3 WHERE [Id] = @Id_4;";
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
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@Id_4").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_1").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_2").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_3").Single().Value); // When
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
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@Id_4").Single().Value); // Id
        Assert.Equal("Test Name", query.Parameters.Where(param => param.Key == "@Name_1").Single().Value); // Name
        Assert.Equal(new DateTime(2023, 10, 1), query.Parameters.Where(param => param.Key == "@DateOf_2").Single().Value); // DateOf
        Assert.Equal("Now", query.Parameters.Where(param => param.Key == "@When_3").Single().Value); // When

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


        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@Name_1").Single().Value); // Name
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@When_3").Single().Value); // When
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

        string expectedSql = "UPDATE [EntityWithoutTableAttribute] SET [Description] = @Description_1 WHERE [Id] = @Id_2;";
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

        string expectedSql = "UPDATE [myschema].[EntityWithSchema] SET [Description] = @Description_1 WHERE [Id] = @Id_2;";
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

        string expectedSql = "UPDATE [Test] SET [Name] = @Name_1, [DateOf] = @DateOf_2, [When] = @When_3 WHERE [Id] = @Id_4;";
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


        SqlQuery query = _sqlGeneratorForSqlTypeEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [TestSqlTypes] SET [LongValue] = @LongValue_1, [ShortValue] = @ShortValue_2, [ByteValue] = @ByteValue_3, [BoolValue] = @BoolValue_4, [DecimalValue] = @DecimalValue_5, [FloatValue] = @FloatValue_6, [DoubleValue] = @DoubleValue_7, [StringValue] = @StringValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [ByteArrayValue] = @ByteArrayValue_11, [CharValue] = @CharValue_12, [TimeOnlyValue] = @TimeOnlyValue_13, [DateOnlyValue] = @DateOnlyValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [IntValue] = @IntValue_16;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(16, query.Parameters.Count);

        Assert.Equal(42, query.Parameters.Where(param => param.Key == "@IntValue_16").Single().Value);                                                // IntValue
        Assert.Equal(1234567890L, query.Parameters.Where(param => param.Key == "@LongValue_1").Single().Value);                                      // LongValue
        Assert.Equal((short)32000, query.Parameters.Where(param => param.Key == "@ShortValue_2").Single().Value);                                    // ShortValue
        Assert.Equal((byte)255, query.Parameters.Where(param => param.Key == "@ByteValue_3").Single().Value);                                        // ByteValue
        Assert.Equal(true, query.Parameters.Where(param => param.Key == "@BoolValue_4").Single().Value);                                             // BoolValue
        Assert.Equal(99.99m, query.Parameters.Where(param => param.Key == "@DecimalValue_5").Single().Value);                                        // DecimalValue
        Assert.Equal(3.14f, query.Parameters.Where(param => param.Key == "@FloatValue_6").Single().Value);                                           // FloatValue
        Assert.Equal(123.456, query.Parameters.Where(param => param.Key == "@DoubleValue_7").Single().Value);                                        // DoubleValue
        Assert.Equal("Test String", query.Parameters.Where(param => param.Key == "@StringValue_8").Single().Value);                                  // StringValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "@DateTimeValue_9").Single().Value);    // DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@GuidValue_10").Single().Value); // GuidValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "@ByteArrayValue_11").Single().Value);             // ByteArrayValue
        Assert.Equal('A', query.Parameters.Where(param => param.Key == "@CharValue_12").Single().Value);                                              // CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "@TimeOnlyValue_13").Single().Value);                        // TimeOnlyValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_15").Single().Value);                // DateTimeOffsetValue
    }

    [Fact]
    public void TestSqlUpdateStringForNullableTypes()
    {
        DateTimeOffset dateTimeOffsetTestValue = NullableTestEntity.DateTimeOffsetTestValue;
        NullableTestEntity entity = NullableTestEntity.GetStandardTestSet();


        SqlQuery query
            = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [NullableTestEntity] SET [IntValue] = @IntValue_1, [LongValue] = @LongValue_2, [ShortValue] = @ShortValue_3, [ByteValue] = @ByteValue_4, [BoolValue] = @BoolValue_5, [DecimalValue] = @DecimalValue_6, [FloatValue] = @FloatValue_7, [DoubleValue] = @DoubleValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [CharValue] = @CharValue_11, [TimeOnlyValue] = @TimeOnlyValue_12, [DateOnlyValue] = @DateOnlyValue_13, [ByteArrayValue] = @ByteArrayValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [Key] = @Key_16;";
        Assert.Equal(expectedSql, query.QueryText);

        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "@Key_16").Single().Value);                                             // Key
        Assert.Equal((int?)1, query.Parameters.Where(param => param.Key == "@IntValue_1").Single().Value);                                           // Nullable IntValue
        Assert.Equal((long?)123456789L, query.Parameters.Where(param => param.Key == "@LongValue_2").Single().Value);                                // Nullable LongValue
        Assert.Equal((short?)123, query.Parameters.Where(param => param.Key == "@ShortValue_3").Single().Value);                                     // Nullable ShortValue
        Assert.Equal((byte?)255, query.Parameters.Where(param => param.Key == "@ByteValue_4").Single().Value);                                       // Nullable ByteValue
        Assert.Equal((bool?)true, query.Parameters.Where(param => param.Key == "@BoolValue_5").Single().Value);                                      // Nullable BoolValue
        Assert.Equal((decimal?)99.99m, query.Parameters.Where(param => param.Key == "@DecimalValue_6").Single().Value);                              // Nullable DecimalValue
        Assert.Equal((float?)3.14f, query.Parameters.Where(param => param.Key == "@FloatValue_7").Single().Value);                                   // Nullable FloatValue
        Assert.Equal((double?)1.618, query.Parameters.Where(param => param.Key == "@DoubleValue_8").Single().Value);                                 // Nullable DoubleValue
        Assert.Equal(new DateTime(2024, 11, 6, 1, 14, 1, 2, 3), query.Parameters.Where(param => param.Key == "@DateTimeValue_9").Single().Value);    // Nullable DateTimeValue
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.Where(param => param.Key == "@GuidValue_10").Single().Value); // Nullable GuidValue
        Assert.Equal((char?)'A', query.Parameters.Where(param => param.Key == "@CharValue_11").Single().Value);                                       // Nullable CharValue
        Assert.Equal(new TimeOnly(1, 2, 0), query.Parameters.Where(param => param.Key == "@TimeOnlyValue_12").Single().Value);                        // Nullable TimeOnlyValue
        Assert.Equal(new DateOnly(1, 12, 25), query.Parameters.Where(param => param.Key == "@DateOnlyValue_13").Single().Value);                      // Nullable DateOnlyValue
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, query.Parameters.Where(param => param.Key == "@ByteArrayValue_14").Single().Value);             // Nullable ByteArrayValue
        Assert.Equal(dateTimeOffsetTestValue, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_15").Single().Value);                // Nullable DateTimeOffsetValue
    }


    [Fact]
    public void TestSqlUpdateStringForNullableTypes_WithNullValues()
    {
        NullableTestEntity entity = NullableTestEntity.GetNullTestSet();


        SqlQuery query = _sqlGeneratorForNullablesTestEntity.UpdateById(entity, null);

        string expectedSql = "UPDATE [NullableTestEntity] SET [IntValue] = @IntValue_1, [LongValue] = @LongValue_2, [ShortValue] = @ShortValue_3, [ByteValue] = @ByteValue_4, [BoolValue] = @BoolValue_5, [DecimalValue] = @DecimalValue_6, [FloatValue] = @FloatValue_7, [DoubleValue] = @DoubleValue_8, [DateTimeValue] = @DateTimeValue_9, [GuidValue] = @GuidValue_10, [CharValue] = @CharValue_11, [TimeOnlyValue] = @TimeOnlyValue_12, [DateOnlyValue] = @DateOnlyValue_13, [ByteArrayValue] = @ByteArrayValue_14, [DateTimeOffsetValue] = @DateTimeOffsetValue_15 WHERE [Key] = @Key_16;";
        Assert.Equal(expectedSql, query.QueryText);


        // Assert that parameters have the correct values and are correctly mapped
        Assert.Equal(Guid.Empty, query.Parameters.Where(param => param.Key == "@Key_16").Single().Value);             // Key
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@IntValue_1").Single().Value);      // Nullable IntValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@LongValue_2").Single().Value);     // Nullable LongValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ShortValue_3").Single().Value);    // Nullable ShortValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ByteValue_4").Single().Value);     // Nullable ByteValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@BoolValue_5").Single().Value);     // Nullable BoolValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DecimalValue_6").Single().Value);  // Nullable DecimalValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@FloatValue_7").Single().Value);    // Nullable FloatValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DoubleValue_8").Single().Value);   // Nullable DoubleValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateTimeValue_9").Single().Value); // Nullable DateTimeValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@GuidValue_10").Single().Value);     // Nullable GuidValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@CharValue_11").Single().Value);     // Nullable CharValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@TimeOnlyValue_12").Single().Value); // Nullable TimeOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateOnlyValue_13").Single().Value); // Nullable DateOnlyValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@ByteArrayValue_14").Single().Value);    // Nullable ByteArrayValue
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@DateTimeOffsetValue_15").Single().Value); // Nullable DateTimeOffsetValue
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

        string expectedSql = "UPDATE [Test] SET [NotSensitiveData] = @NotSensitiveData_1, [SensitiveData] = @SensitiveData_2, [KeyVersion] = @KeyVersion_3 WHERE [Id] = @Id_4;";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(1337, query.Parameters.Where(param => param.Key == "@Id_4").Single().Value); // Id
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "@NotSensitiveData_1").Single().Value);
        Assert.NotEqual("Shhh...", query.Parameters.Where(param => param.Key == "@SensitiveData_2").Single().Value);
        Assert.Equal("Shhh...", _mockEncrypter.Decrypt(query.Parameters.Where(param => param.Key == "@SensitiveData_2").Single().Value.ToString()));
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "@KeyVersion_3").Single().Value);
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

        string expectedSql = "UPDATE [Test] SET [NotSensitiveData] = @NotSensitiveData_1, [SensitiveData] = @SensitiveData_2, [KeyVersion] = @KeyVersion_3 WHERE [Id] = @Id_4;";

        Assert.Equal(expectedSql, query.QueryText);

        Assert.Equal(4, query.Parameters.Count);
        Assert.Equal(1337, query.Parameters.Where(param => param.Key == "@Id_4").Single().Value); // Id
        Assert.Equal("SHOUT. SHOUT IT OUT LOUD. THESE ARE THE THINGS...", query.Parameters.Where(param => param.Key == "@NotSensitiveData_1").Single().Value);
        Assert.Equal(DBNull.Value, query.Parameters.Where(param => param.Key == "@SensitiveData_2").Single().Value);
        Assert.Equal(1, query.Parameters.Where(param => param.Key == "@KeyVersion_3").Single().Value);
    }

    [Fact]
    public void SqlUpdate_WithInnerJoin_WithJoinsAndPredicates()
    {

        CompositePrimaryKeyTable entity3 = new()
        {
            Id1 = 11,
            Id2 = 12,

            NotKey1 = 13,
            NotKey2 = 14
        };

        CompositePrimaryKeyTable entity1 = new()
        {
            Id1 = 1,
            Id2 = 2,

            NotKey1 = 3,
            NotKey2 = 4
        };

        CompositePrimaryKeyTable entity2 = new()
        {
            Id1 = 5,
            Id2 = 6,

            NotKey1 = 7,
            NotKey2 = 8
        };

        PredicatesLogic.Predicates predicateId = new Equal(new Column<JoinLeftTable>("Id"), new Parameter("Id", 3));
        SqlQuery query = _sqlGeneratorCompositeKeyTable.UpdateByIds(entity3, _leftCompositeKeyTable, entity1, entity2);

        string expectedSql = "UPDATE [Ck] SET [Ck].[NotKey1] = @NotKey1_1, [Ck].[NotKey2] = @NotKey2_2 FROM [Ck] WHERE " +
            "((([Ck].[Id1] = @Id1_3) AND ([Ck].[Id2] = @Id2_4)) OR (([Ck].[Id1] = @Id1_5) AND ([Ck].[Id2] = @Id2_6)))";
        Assert.Equal(expectedSql, query.QueryText);

        int expectedCount = 6;
        int actualCount = query.Parameters.Count;

        Assert.Equal(expectedCount, actualCount);

        int expectedValue = 13;
        int actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@NotKey1_1").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 14;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@NotKey2_2").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 1;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Id1_3").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 2;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Id2_4").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 5;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Id1_5").Single().Value;

        Assert.Equal(expectedValue, actualValue);

        expectedValue = 6;
        actualValue = (int)query.Parameters.AsEnumerable().Where(parameter => parameter.Key == "@Id2_6").Single().Value;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Throws_NoPrimaryKeyException()
    {
        Address value = new() { City = "Clarksville", PostalCode = "37043", Street = "Madison" };
        IEnumerable<Address> entities =
        [
            new() { City = "Clarksville", PostalCode = "37043", Street = "Madison" }
        ];
        Assert.Throws<NoPrimaryKeyPropertyException<Address>>(() => _sqlGeneratorAddress.UpdateById(entities.Single()));
        Assert.Throws<NoPrimaryKeyPropertyException<Address>>(() => _sqlGeneratorAddress.UpdateByIds(value, null, entities));
    }

    [Fact]
    public void SqlUpdateString_GeneratesCorrectSql_WithCompositePrimaryKey_UsesAndInWhereClause()
    {
        CompositePrimaryKeyTable entity = new()
        {
            Id1 = 1,
            Id2 = 2,
            NotKey1 = 10,
            NotKey2 = 20,
            NotKey3 = 30
        };

        SqlQuery query = _sqlGeneratorCompositeKeyTable.UpdateById(entity);

        string expectedSql =
            "UPDATE [Ck] SET [NotKey1] = @NotKey1_1, [NotKey2] = @NotKey2_2, [NotKey3] = @NotKey3_3 WHERE [Id1] = @Id1_4 AND [Id2] = @Id2_5;";

        Assert.Equal(expectedSql, query.QueryText);
    }
}