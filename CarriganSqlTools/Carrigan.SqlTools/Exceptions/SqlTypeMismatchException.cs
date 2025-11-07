using Carrigan.SqlTools.Attributes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Documentation and unit tests.
public class SqlTypeMismatchException : Exception
{
    private static readonly ReadOnlyDictionary<Type, IEnumerable<Type>> Allowed;

    static SqlTypeMismatchException()
    {
        Dictionary<Type, IEnumerable<Type>> types = new
        (
            [
                new (typeof(char), [typeof(SqlCharAttribute), typeof(SqlVarCharMaxAttribute)]),
                new (typeof(string), [typeof(SqlCharAttribute), typeof(SqlVarCharMaxAttribute)]),
                new (typeof(byte[]), [typeof(SqlBinaryAttribute), typeof(SqlVarBinaryMaxAttribute)]),
                new (typeof(decimal), [typeof(SqlMoneyAttribute), typeof(SqlDecimalAttribute)]),
                new (typeof(DateTime), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute)]),
                new (typeof(DateTimeOffset), [typeof(SqlDateTimeOffsetAttribute)]),
                new (typeof(DateOnly), [typeof(SqlDateTimeAttribute), typeof(SqlDateTime2Attribute)]),
                new (typeof(TimeOnly), [typeof(SqlTimeAttribute)]),
                new (typeof(float), [typeof(SqlFloatAttribute)])
            ]
        );
        Allowed = new (types);
    }

    private SqlTypeMismatchException(PropertyInfo propertyInfo, Type attributeMappingType) : base
        ($"Sql Type Attribute Mismatch: C# typ, {propertyInfo.GetType().Name}, is inherently incompatible with {attributeMappingType.Name}.")
    { } 

    public static SqlTypeMismatchException? Validate(PropertyInfo propertyInfo, SqlTypeAttribute? sqlTypeAttribute)
    {
        static bool TestTheType(Type propertyType, Type attributeMappingType)
        {
            if (Allowed.ContainsKey(propertyType) is false)
                return false;
            else
                return Allowed[propertyType].Contains(attributeMappingType);
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
}
