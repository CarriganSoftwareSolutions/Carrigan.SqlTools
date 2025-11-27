using Carrigan.SqlTools.Attributes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Unit Tests
/// <summary>
/// Represents an error that occurs when the SQL type associated with a property/value
/// is inherently incompatible with the corresponding CLR type.
/// </summary>
/// <remarks>
/// This exception is used to enforce SQL type validation with typed 
/// compatibility rules between CLR model types and their associated SQL metadata.
/// It is thrown only in cases of fundamental incompatibility.
/// These Property type attributes are also enforced by a code analyzer provided with the library.
/// </remarks>
public sealed class SqlTypeMismatchException : Exception
{
    /// <summary>
    /// Contains the allowed <see cref="SqlTypeAttribute"/> types for a given CLR type.
    /// </summary>
    private static readonly ReadOnlyDictionary<Type, ImmutableArray<Type>> AllowedAttributes;

    /// <summary>
    /// Contains the allowed <see cref="SqlDbType"/> values for a given CLR type.
    /// </summary>
    private static readonly ReadOnlyDictionary<Type, ImmutableArray<SqlDbType>> AllowedSqlDbTypes;


    /// <summary>
    /// Initializes the static mapping tables used to validate SQL type compatibility.
    /// </summary>
    /// <remarks>
    /// These mappings define which SQL type attributes and SQL type enumerations are
    /// considered valid for each CLR type. They are used by the
    /// <see cref="Validate(PropertyInfo, SqlTypeAttribute?)"/> and
    /// <see cref="Validate(object, SqlDbType)"/> methods.
    /// </remarks>
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
    /// Initializes a new instance of the <see cref="SqlTypeMismatchException"/> class
    /// for a mismatch between a CLR property type and a <see cref="SqlTypeAttribute"/>.
    /// </summary>
    /// <param name="propertyInfo">The property being validated.</param>
    /// <param name="attributeMappingType">The attribute type that is incompatible with the property.</param>
    private SqlTypeMismatchException(PropertyInfo propertyInfo, Type attributeMappingType) : base
        ($"Sql Type Attribute Mismatch: Property '{propertyInfo.Name}' with C# type '{propertyInfo.PropertyType.Name}' is inherently incompatible with attribute '{attributeMappingType.Name}'.")
    { }


    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTypeMismatchException"/> class
    /// for a mismatch between a CLR value type and a <see cref="SqlDbType"/>.
    /// </summary>
    /// <param name="type">The CLR type that is incompatible.</param>
    /// <param name="sqlDbType">The corresponding SQL type that caused the mismatch.</param>
    private SqlTypeMismatchException(Type type, SqlDbType sqlDbType) : base
        ($"Sql Type Attribute Mismatch: C# type '{type.Name}' is inherently incompatible with SQL type '{sqlDbType}'.")

    { }


    /// <summary>
    /// Validates whether a CLR property and its associated <see cref="SqlTypeAttribute"/>
    /// represent a compatible type pairing.
    /// </summary>
    /// <param name="propertyInfo">The property to validate.</param>
    /// <param name="sqlTypeAttribute">The SQL type attribute applied to the property.</param>
    /// <returns>
    /// <c>null</c> if the types are compatible, or an instance of
    /// <see cref="SqlTypeMismatchException"/> if the types are inherently incompatible.
    /// </returns>
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
    /// Validates whether a CLR value is compatible with a specific <see cref="SqlDbType"/>.
    /// </summary>
    /// <remarks>
    /// This method is typically used when validating parameter values assigned to
    /// ADO.NET commands.
    /// </remarks>
    /// <param name="value">The CLR value to validate.</param>
    /// <param name="sqlDbType">The SQL type that the value is being assigned to.</param>
    /// <returns>
    /// <c>null</c> if the value is compatible with the specified SQL type, or an instance of
    /// <see cref="SqlTypeMismatchException"/> if a mismatch is detected.
    /// </returns>
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
