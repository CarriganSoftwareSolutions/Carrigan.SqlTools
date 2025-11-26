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

    //TODO: Proof read Documentation, unit tests
    /// <summary>
    /// Static method to get a Attribute that inherits from <see cref="SqlTypeAttribute"/>
    /// </summary>
    /// <param name="propertyInfo">property info</param>
    /// <returns>SqlTypeAttribute</returns>
    internal static SqlTypeAttribute? GetSqlTypeAttribute(PropertyInfo propertyInfo) =>
       propertyInfo
        .GetCustomAttributes(inherit: true)
        .OfType<SqlTypeAttribute>()
        .FirstOrDefault();
}
