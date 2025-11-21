using Carrigan.SqlTools.Attributes;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Documentation and unit tests, code review
public class SqlTypeMismatchException : Exception
{
    private static readonly ReadOnlyDictionary<Type, IEnumerable<Type>> AllowedAttributes;
    private static readonly ReadOnlyDictionary<Type, IEnumerable<SqlDbType>> AllowedSqlDbTypes;

    static SqlTypeMismatchException()
    {
        Dictionary<Type, IEnumerable<Type>> attributeTypes = new
        (
            [
                new (typeof(char), [typeof(SqlCharAttribute), typeof(SqlVarCharMaxAttribute), typeof(SqlTextAttribute)]),
                new (typeof(string), [typeof(SqlCharAttribute), typeof(SqlVarCharMaxAttribute), typeof(SqlTextAttribute)]),

                new (typeof(byte[]), [typeof(SqlBinaryAttribute), typeof(SqlVarBinaryMaxAttribute), typeof(SqlImageAttribute)]),

                new (typeof(decimal), [typeof(SqlFloatAttribute), typeof(SqlDecimalAttribute), typeof(SqlMoneyAttribute)]),
                new (typeof(float), [typeof(SqlFloatAttribute), typeof(SqlDecimalAttribute), typeof(SqlMoneyAttribute)]),
                new (typeof(double), [typeof(SqlFloatAttribute), typeof(SqlDecimalAttribute), typeof(SqlMoneyAttribute)]),

                new (typeof(DateTime), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute)]),
                new (typeof(DateOnly), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute)]),
                new (typeof(TimeOnly), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute), typeof(SqlTimeAttribute)]),

                new (typeof(DateTimeOffset), [typeof(SqlDateTimeOffsetAttribute)]),
            ]
        );

        Dictionary<Type, IEnumerable<SqlDbType>> sqlDbTypes = new
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

    private SqlTypeMismatchException(PropertyInfo propertyInfo, Type attributeMappingType) : base
        ($"Sql Type Attribute Mismatch: C# type, {propertyInfo.GetType().Name}, is inherently incompatible with {attributeMappingType.Name}.")
    { }
    private SqlTypeMismatchException(Type propertyType, SqlDbType sqlDbType) : base
        ($"Sql Type Attribute Mismatch: C# type, {propertyType.Name}, is inherently incompatible with {sqlDbType}.")
    { }

    public static SqlTypeMismatchException? Validate(PropertyInfo propertyInfo, SqlTypeAttribute? sqlTypeAttribute)
    {
        static bool TestTheType(Type propertyType, Type attributeMappingType)
        {
            if (AllowedAttributes.ContainsKey(propertyType) is false)
                return false;
            else
                return AllowedAttributes[propertyType].Contains(attributeMappingType);
        }
        Type? attributeMappingType = sqlTypeAttribute?.GetType();
        Type propertyType = propertyInfo.PropertyType;
        if (attributeMappingType == null)
            return null;
        else
        {
            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            attributeMappingType = Nullable.GetUnderlyingType(attributeMappingType) ?? attributeMappingType;
            if(TestTheType(propertyType, attributeMappingType))
                return null;
            else
                return new SqlTypeMismatchException(propertyInfo, attributeMappingType);
        }
    }

    public static SqlTypeMismatchException? Validate(object value, SqlDbType sqlTypeAttribute)
    {
        static bool TestTheType(Type propertyType, SqlDbType attributeMappingType)
        {
            if (AllowedSqlDbTypes.ContainsKey(propertyType) is false)
                return false;
            else
                return AllowedSqlDbTypes[propertyType].Contains(attributeMappingType);
        }

        Type propertyType;
        if (sqlTypeAttribute == SqlDbType.Variant)
            return null;
        else
        {
            propertyType = value.GetType();
            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (TestTheType(propertyType, sqlTypeAttribute))
                return null;
            else
                return new SqlTypeMismatchException(propertyType, sqlTypeAttribute);
        }
    }
}
