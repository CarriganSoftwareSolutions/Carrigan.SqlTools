
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SqlTools.Extensions;

public static class TypeExtensions
{
    private readonly static Dictionary<Type, string> TableNames = [];
    private readonly static Dictionary<Type, string> TableSchemas = [];
    public static string TableName(this Type type)
    {
        lock (TableNames)
        {
            if (TableNames.ContainsKey(type) is false)
            {
                TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
                TableNames[type] = tableAttribute is null ? type.Name : tableAttribute.Name;
            }
            return TableNames[type];
        }
    }


    public static string TableSchema(this Type type)
    {
        lock (TableSchemas)
        {
            if (TableSchemas.ContainsKey(type) is false)
            {
                TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
                TableSchemas[type] = tableAttribute is null || string.IsNullOrEmpty(tableAttribute.Schema) ? string.Empty : tableAttribute.Schema;
            }
            return TableSchemas[type];
        }
    }
}
