using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Used specify which column(s) should be used when generating a query based on an Id field.
/// Note: The SQL generator respects properties marked with <see cref="KeyAttribute"/>.
/// However, if any property is marked with the SQL generator's own <see cref="PrimaryKeyAttribute"/>,
/// that designation takes precedence and overrides all <see cref="KeyAttribute"/> markings
/// for SQL generation purposes.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public  class PrimaryKeyAttribute : Attribute
{
}
