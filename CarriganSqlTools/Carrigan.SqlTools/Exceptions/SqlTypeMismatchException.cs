using Carrigan.SqlTools.Attributes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// This exception is thrown when the SQL  Type associated with a property uses an inherently incompatible type for the model.
/// </summary>
public sealed class SqlTypeMismatchException : Exception
{
    /// <summary>
    /// Represents the allowed attributes for a a given code type.
    /// </summary>
    private static readonly ReadOnlyDictionary<Type, ImmutableArray<Type>> AllowedAttributes;

    /// <summary>
    /// Represents the SQL types allowed to be associated with a given code type.
    /// </summary>
    private static readonly ReadOnlyDictionary<Type, ImmutableArray<SqlDbType>> AllowedSqlDbTypes;


    /// <summary>
    /// Static constructor for assigning the values to <see cref="AllowedAttributes"/> and <see cref="AllowedSqlDbTypes"/>
    /// </summary>
    static SqlTypeMismatchException()
    {
        Dictionary<Type, ImmutableArray<Type>> attributeTypes = new
        (
            [
                new (typeof(char), [typeof(SqlCharAttribute), typeof(SqlVarCharMaxAttribute), typeof(SqlTextAttribute)]),
                new (typeof(string), [typeof(SqlCharAttribute), typeof(SqlVarCharMaxAttribute), typeof(SqlTextAttribute)]),

                new (typeof(byte[]), [typeof(SqlBinaryAttribute), typeof(SqlVarBinaryMaxAttribute), typeof(SqlImageAttribute)]),

                new (typeof(decimal), [typeof(SqlFloatAttribute), typeof(SqlDecimalAttribute), typeof(SqlMoneyAttribute)]),
                new (typeof(float), [typeof(SqlFloatAttribute), typeof(SqlDecimalAttribute), typeof(SqlMoneyAttribute)]),
                new (typeof(double), [typeof(SqlFloatAttribute), typeof(SqlDecimalAttribute), typeof(SqlMoneyAttribute)]),

                new (typeof(DateTime), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute), typeof(SqlDateAttribute), typeof(SqlTimeAttribute)]),
                new (typeof(DateOnly), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute), typeof(SqlDateAttribute)]),
                new (typeof(TimeOnly), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute), typeof(SqlTimeAttribute)]),

                new (typeof(DateTimeOffset), [typeof(SqlDateTimeOffsetAttribute)]),
            ]
        );

        Dictionary<Type, ImmutableArray<SqlDbType>> sqlDbTypes = new
        (
            [   new (typeof(Guid), [SqlDbType.UniqueIdentifier]),

                new (typeof(char), [SqlDbType.Char, SqlDbType.NChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.Text, SqlDbType.NText]),
                new (typeof(string), [SqlDbType.Char, SqlDbType.NChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.Text, SqlDbType.NText]),
                new (typeof(byte[]), [SqlDbType.Binary, SqlDbType.VarBinary, SqlDbType.Image]),

                new (typeof(bool), [SqlDbType.Bit]),

                new (typeof(byte), [SqlDbType.TinyInt]),
                new (typeof(sbyte), [SqlDbType.SmallInt]),
                new (typeof(short), [SqlDbType.SmallInt]),
                new (typeof(int), [SqlDbType.Int]),
                new (typeof(long), [SqlDbType.BigInt]),

                new (typeof(float), [SqlDbType.Real, SqlDbType.Float, SqlDbType.Decimal, SqlDbType.Money, SqlDbType.SmallMoney]),
                new (typeof(double), [SqlDbType.Real, SqlDbType.Float, SqlDbType.Decimal, SqlDbType.Money, SqlDbType.SmallMoney]),
                new (typeof(decimal), [SqlDbType.Real, SqlDbType.Float, SqlDbType.Decimal, SqlDbType.Money, SqlDbType.SmallMoney]),

                new (typeof(DateTime), [SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2]),
                new (typeof(DateOnly), [SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2]),
                new (typeof(TimeOnly), [SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2]),
                new (typeof(DateTimeOffset), [SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2, SqlDbType.Time])
            ]
        );
        AllowedAttributes = new (attributeTypes);
        AllowedSqlDbTypes = new(sqlDbTypes);
    }

    /// <summary>
    /// Private constructor that sets the error message for a sql type attribute mismatch.
    /// </summary>
    /// <param name="propertyInfo">Property Info</param>
    /// <param name="attributeMappingType"> the mismatched attribute mapped to the property</param>
    private SqlTypeMismatchException(PropertyInfo propertyInfo, Type attributeMappingType) : base
        ($"Sql Type Attribute Mismatch: Property '{propertyInfo.Name}' with C# type '{propertyInfo.PropertyType.Name}' is inherently incompatible with attribute '{attributeMappingType.Name}'.")
    { }

    /// <summary>
    /// Private constructor that sets the error message for a <see cref="SqlDbType"/> mismatch.
    /// </summary>
    /// <param name="type">the code Type</param>
    /// <param name="attributeMappingType"> the mismatched <see cref="SqlDbType"/></param>
    private SqlTypeMismatchException(Type type, SqlDbType sqlDbType) : base
        ($"Sql Type Attribute Mismatch: C# type '{type.Name}' is inherently incompatible with SQL type '{sqlDbType}'.")

    { }

    /// <summary>
    /// static method uses to test a property and its associated attribute that inherits from <see cref="SqlTypeAttribute"/>
    /// </summary>
    /// <param name="propertyInfo">Property Info</param>
    /// <param name="attributeMappingType"> the attribute mapped to the property</param>
    /// <returns>returns null if there is no mismatch, returns an exception if there is a type mismatch.</returns>
    public static SqlTypeMismatchException? Validate(PropertyInfo propertyInfo, SqlTypeAttribute? sqlTypeAttribute)
    {
        static bool TestTheType(Type propertyType, Type attributeMappingType)
        {
            if (AllowedAttributes.TryGetValue(propertyType, out ImmutableArray<Type> allowedAttributes))
                return allowedAttributes.Contains(attributeMappingType);
            else
                return false;
        }
        Type? attributeMappingType = sqlTypeAttribute?.GetType();
        Type propertyType = propertyInfo.PropertyType;
        if (attributeMappingType is null)
            return null;
        else
        {
            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if(TestTheType(propertyType, attributeMappingType))
                return null;
            else
                return new SqlTypeMismatchException(propertyInfo, attributeMappingType);
        }
    }

    /// <summary>
    /// static method uses to test a a value and its associated <see cref="SqlDbType"/>.
    /// </summary>
    /// <remarks>
    ///This is used for validating parameter values with an <see cref="SqlDbType"/>.
    /// </remarks>
    /// <param name="value">a value</param>
    /// <param name="sqlDbType">a <see cref="SqlDbType"/></param>
    /// <returns></returns>
    public static SqlTypeMismatchException? Validate(object value, SqlDbType sqlDbType)
    {
        static bool TestTheType(Type propertyType, SqlDbType sqlDbType)
        {
            if (AllowedSqlDbTypes.TryGetValue(propertyType, out ImmutableArray<SqlDbType> allowedSqlDbTypes))
                return allowedSqlDbTypes.Contains(sqlDbType);
            else
                return false;
        }

        Type propertyType;
        if (sqlDbType == SqlDbType.Variant || value is null)
            return null;
        else
        {
            propertyType = value.GetType();
            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (TestTheType(propertyType, sqlDbType))
                return null;
            else
                return new SqlTypeMismatchException(propertyType, sqlDbType);
        }
    }
}
