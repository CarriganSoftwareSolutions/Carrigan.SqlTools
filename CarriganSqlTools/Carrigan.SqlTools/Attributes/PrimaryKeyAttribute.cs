using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies which column or columns should be used when generating a query based on an Id field.
/// The SQL generator respects properties marked with <see cref="KeyAttribute"/>; however, if any
/// property is marked with the SQL generator's own <see cref="PrimaryKeyAttribute"/>, that
/// designation takes precedence and overrides all <see cref="KeyAttribute"/> markings for SQL
/// generation purposes.
///
/// This does not affect Entity Framework’s use of <see cref="KeyAttribute"/> and does not
/// override its behavior at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public  class PrimaryKeyAttribute : Attribute
{
}
