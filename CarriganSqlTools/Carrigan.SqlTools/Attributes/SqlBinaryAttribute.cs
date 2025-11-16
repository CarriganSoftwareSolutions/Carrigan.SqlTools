using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Proof read Documentation and Unit Tests

/// <summary>
/// An attribute that allows for overriding default data type mapping for a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlBinaryAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="SqlBinaryAttribute"/> for a fixed or variable binary column.
    /// </summary>
    /// <param name="storageTypeEnum">Determines if the instance will represent a variable storage type of a fixed storage type.</param>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum) : base (GetSqlTypeDefinition(storageTypeEnum))
    {

    }
    /// <summary>
    /// Initializes a new instance of <see cref="SqlBinaryAttribute"/> with a specific size.
    /// </summary>
    /// <param name="storageTypeEnum">Determines if the instance will represent a variable storage type of a fixed storage type.</param>
    /// <param name="size">Determines the storage size that instance will represent.</param>
    public SqlBinaryAttribute(StorageTypeEnum storageTypeEnum, int size) : base(GetSqlTypeDefinition(storageTypeEnum, size))
    {

    }

    /// <summary>
    /// Creates an instance of the attribute that allows for overriding default data type mapping for a property.
    /// </summary>
    /// <param name="storageTypeEnum">Determines if the instance will represent a variable storage type of a fixed storage type.</param>
    /// <param name="size">Determines the storage size that instance will represent.</param>
    /// <returns>An <see cref="SqlTypeDefinition"/> reflecting the supplied arguments.</returns>
    /// <exception cref="NotSupportedException"></exception>
    private static SqlTypeDefinition GetSqlTypeDefinition(StorageTypeEnum storageTypeEnum, int? size = null) => 
        storageTypeEnum switch
        {
            StorageTypeEnum.Fixed => SqlTypeDefinition.AsBinary(size),
            StorageTypeEnum.Var => SqlTypeDefinition.AsVarBinary(size),
            _ => throw new NotSupportedException()
        };
}
