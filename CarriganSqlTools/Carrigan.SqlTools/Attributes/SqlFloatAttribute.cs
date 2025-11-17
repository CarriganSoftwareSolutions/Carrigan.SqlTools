using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
//TODO: Documentation and Unit Tests

/// <summary>
/// Specifies that a property represents a <c>FLOAT</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a SQL Server
/// <c>FLOAT</c> column on a table model.
///
/// <para>
/// SQL Server <c>FLOAT</c> allows an optional precision value (1–53), which affects  
/// its underlying storage size and numeric range. This attribute exposes that precision  
/// through constructor overloads so the data model can explicitly control the column
/// definition.
/// </para>
///
/// <para>
/// When applied to a property, this attribute overrides the default type mapping  
/// used by <see cref="Carrigan.SqlTools"/> during SQL generation and column metadata  
/// reflection.
/// </para>
///
/// <para><strong>Suggested C# Data Type:</strong><br/>
/// Properties mapped to SQL <c>FLOAT</c> columns should use the .NET <see cref="double"/> type.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlFloatAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloatAttribute"/> class and configures
    /// the associated property to represent a <c>FLOAT</c> column using SQL Server's default
    /// precision.
    /// </summary>
    /// <remarks>
    /// <para><strong>Suggested C# Data Type:</strong><br/>
    /// Properties mapped to SQL <c>FLOAT</c> columns should use the .NET <see cref="double"/> type.
    /// </para>
    /// </remarks>
    public SqlFloatAttribute() : base(SqlTypeDefinition.AsFloat())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloatAttribute"/> class and configures
    /// the associated property to represent a <c>FLOAT</c> column with an explicit precision.
    /// </summary>
    /// <remarks>
    /// <para><strong>Suggested C# Data Type:</strong><br/>
    /// Properties mapped to SQL <c>FLOAT</c> columns should use the .NET <see cref="double"/> type.
    /// </para>
    /// </remarks>
    /// <param name="precision">
    /// The SQL Server <c>FLOAT</c> precision (1–53).  
    /// This value determines the number of bits used to store the floating-point mantissa.
    /// </param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="precision"/> is outside the valid SQL Server range of 1–53.
    /// </exception>
    public SqlFloatAttribute(byte precision) : base(SqlTypeDefinition.AsFloat(precision))
    {
    }
}
