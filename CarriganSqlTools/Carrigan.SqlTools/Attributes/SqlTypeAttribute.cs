using Carrigan.SqlTools.Types;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// A base class attribute that allows overriding the database type for the column
/// associated with the applied property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class SqlTypeAttribute : Attribute
{
    /// <summary>
    /// The SQL Server ADO.NET type, as well as the text required to declare the
    /// indicated type in SQL using the supplied sizing arguments.
    /// </summary>
    internal readonly SqlTypeDefinition SqlTypeDefinition;

    protected SqlTypeAttribute(SqlTypeDefinition sqlTypeDefinition) =>
        SqlTypeDefinition = sqlTypeDefinition;

    /// <summary>
    /// Retrieves the first attribute applied to the specified property that
    /// inherits from <see cref="SqlTypeAttribute"/>.
    /// </summary>
    /// <param name="propertyInfo">
    /// The <see cref="PropertyInfo"/> instance representing the property.
    /// </param>
    /// <returns>
    /// The first <see cref="SqlTypeAttribute"/> instance applied to the property,
    /// or <c>null</c> if no such attribute exists.
    /// </returns>
    internal static SqlTypeAttribute? GetSqlTypeAttribute(PropertyInfo propertyInfo) =>
       propertyInfo
        .GetCustomAttributes(inherit: true)
        .OfType<SqlTypeAttribute>()
        .FirstOrDefault();
}
