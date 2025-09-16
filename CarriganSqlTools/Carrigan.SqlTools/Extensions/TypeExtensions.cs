using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Extensions;

public static class TypeExtensions
{
    private readonly static Dictionary<Type, string> TableNames = [];
    private readonly static Dictionary<Type, string> TableSchemas = [];

    /// <summary>
    /// Use reflection to get the name of the table for a given class. This is will look at the table attribution, from DataAnnotations, first.
    /// If the annotation is not present, it will then get the name of the class.
    /// Effectively the table name is considered to be the name of the class unless it has been overridden with the data annotation.
    /// In the future, I would like to add a third top priority item with my own custom attribute.
    /// </summary>
    /// <param name="type">The data Type of the class.</param>
    /// <returns>The assumed SQL name of the table this class represent.</returns>
    public static string TableName(this Type type)
    {
        lock (TableNames)
        {
            if (TableNames.ContainsKey(type) is false)
            {
                TableAttribute? tableAttribute = type.GetCustomAttribute<TableAttribute>();
                TableNames[type] = tableAttribute is null ? type.Name : tableAttribute.Name;
            }
            return TableNames[type];
        }
    }

    /// <summary>
    /// Use reflection to get the name of the schema for a given class. This is will look at the table attribution, from DataAnnotations.
    /// If the annotation is not present, then no schema will be used in the generated SQL, 
    /// and the query will run as the default schema for the user associated with the connection string used to connect to the database.
    /// In the future, I would like to add a library specific top priority attribute, so users of this library don't need to use the ms data annotations.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Either an empty string or a schema name</returns>
    public static string TableSchema(this Type type)
    {
        lock (TableSchemas)
        {
            if (TableSchemas.ContainsKey(type) is false)
            {
                TableAttribute? tableAttribute = type.GetCustomAttribute<TableAttribute>();
                TableSchemas[type] = tableAttribute is null || string.IsNullOrEmpty(tableAttribute.Schema) ? string.Empty : tableAttribute.Schema;
            }
            return TableSchemas[type];
        }
    }
}
