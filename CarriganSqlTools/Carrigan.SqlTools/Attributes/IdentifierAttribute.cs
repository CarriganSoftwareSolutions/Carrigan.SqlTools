using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Attribute for classes and properties that specifies the identifiers to use in generated SQL,
/// including table names, column names, and schema names.
/// 
/// Behavior:
/// - If <see cref="IdentifierAttribute"/> is not defined, the SQL generator falls back to
///   <see cref="TableAttribute"/> and <see cref="ColumnAttribute"/> annotations.
/// - If those are also absent, it uses the class or property name.
/// - If no schema is defined, no schema is applied.
/// 
/// Notes:
/// - This attribute does **not** override <see cref="TableAttribute"/> or <see cref="ColumnAttribute"/> 
///   within Entity Framework. However, it takes precedence for SQL generation performed by the SQL generator.
/// - If you are already using <see cref="TableAttribute"/> and <see cref="ColumnAttribute"/> with
///   Entity Framework, it is best to continue using those—SQL generation will honor them.
/// - If you configure tables, columns, and schemas with fluent mappings, use <see cref="IdentifierAttribute"/> 
///   to ensure consistency with your fluent configuration.
/// - If you are not using Entity Framework, you can still use <see cref="TableAttribute"/> and
///   <see cref="ColumnAttribute"/> if preferred; the SQL generator will respect them.
/// 
/// Important:
/// The SQL generator does not create, modify, or delete database objects such as tables,
/// columns, or stored procedures. It only generates SQL for SELECT, DELETE, INSERT, UPDATE,
/// and stored procedure execution.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IdentifierAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the table/column name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Public getter to indicate the schema name.
    /// </summary>
    public string? Schema { get; }

    public IdentifierAttribute(string Name, string Schema = "")
    {
        bool validName = true;
        bool validSchema = true;
        if (Name.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("SQL identity names cannot be null or empty.", nameof(Name));
        }
        else if (SqlIdentifierPattern.Fails(Name))
        {
            validName = false;
        }

        if (Schema.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(Schema))
        {
            validSchema = false;
        }

        if(validName == false && validSchema == false)
        {
            throw new SqlNamePatternException(Name, Schema);
        }
        else if(validName == false)
        {
            throw new SqlNamePatternException(Name);
        }
        else if (validSchema == false)
        {
            throw new SqlNamePatternException(Schema);
        }
        this.Name = Name;
        this.Schema = Schema;
    }
}
