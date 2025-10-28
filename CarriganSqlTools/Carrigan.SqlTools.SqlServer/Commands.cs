using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Reflection;


namespace Carrigan.SqlTools.SqlServer;

public static class Commands
{
    private static IEnumerable<SqlParameter> GetParameterCollection(SqlQuery query)
    {
        static SqlParameter GetSqlParameter(ParameterTag parameter, object value)
        {
            SqlParameter sqlParameter = new(parameter, value)
            {
                SqlDbType = parameter.SqlType?.Type ?? SqlTypeCache.GetSqlDbTypeFromValue(value)
            };

            if (parameter.SqlType?.UseMax is not null)
            {
                if (parameter.SqlType.UseMax)
                    sqlParameter.Size = -1;
            }
            else
            {
                if (parameter.SqlType?.Size is not null)
                    sqlParameter.Size = parameter.SqlType.Size.Value;
                if (parameter.SqlType?.Precision is not null)
                    sqlParameter.Precision = parameter.SqlType.Precision.Value;
                if (parameter.SqlType?.Scale is not null)
                    sqlParameter.Scale = parameter.SqlType.Scale.Value;
            }

            return sqlParameter;
        }

        return query
                    .Parameters
                    .AsEnumerable()
                    .Select(parameter => GetSqlParameter(parameter.Key, parameter.Value));
    }

    public static void TestConnectionString(string connectionString, string friendlyName)
    {
        try
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            connection.Close();
        }
        catch (Exception ex)
        {
            throw new Exception($"{friendlyName}: Connection could not be established.", ex);
        }
    }

    public static int ExecuteNonQuery(SqlQuery query, DbTransaction? transaction, DbConnection connection)
    {
        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            connection.Open();
        }
        try
        {
            using DbCommand command = connection.CreateCommand();
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.CommandText = query.QueryText;
            command.CommandType = query.CommandType;
            command.Parameters.AddRange(GetParameterCollection(query).ToArray());

            return command.ExecuteNonQuery();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
    public static object? ExecuteScalar(SqlQuery query, DbTransaction? transaction, DbConnection connection)
    {
        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            connection.Open();
        }

        try
        {
            using DbCommand command = connection.CreateCommand();
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandText = query.QueryText;
            command.CommandType = query.CommandType;
            command.Parameters.AddRange(GetParameterCollection(query).ToArray());

            return command.ExecuteScalar();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection) where T : class?, new() => 
        ExecuteReader<T>(query, transaction, connection, null);

    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters) where T : class?, new()
    {
        List<T> results = [];
        PropertyInfo? keyVersionProperty = ClientReflectorCache<T>.KeyVersionProperty;
        IEnumerable<PropertyInfo> encryptedProperties = ClientReflectorCache<T>.EncryptedProperties;
        int? decryptionVersion = 1; //in later versions this will be read from a property marked by a custom annotation attribute, due time constraints, for now it will just be hard coded
        bool wasClosed = false;
        IEncryption? decrypter;

        if (ClientReflectorCache<T>.EncryptedProperties.Any() && decrypters is null)
        {
            throw new DecrypterNotProvided<T>();
        }

        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            connection.Open();
        }

        try
        {
            using DbCommand command = connection.CreateCommand();
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandText = query.QueryText;
            command.CommandType = query.CommandType;
            command.Parameters.AddRange(GetParameterCollection(query).ToArray());

            using DbDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Dictionary<string, object?> rowData = [];
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    rowData.Add(dataReader.GetName(i), dataReader.GetValue(i));
                    string columnName = dataReader.GetName(i);
                }
                results.Add(Invoker<T>.Invoke(rowData));
            }
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
        if (encryptedProperties.Any())
        {
            if (decrypters is null)
                throw new DecrypterNotProvided<T>();

            if (keyVersionProperty is null)
                throw new NullReferenceException($"KeyVersion attribute not set on data model, {ClientReflectorCache<T>.Type.Name}, with encrypted properties.");
            else if((Nullable.GetUnderlyingType(keyVersionProperty.PropertyType) ?? keyVersionProperty.PropertyType) != typeof(int))
                throw new NullReferenceException($"The KeyVersion, {keyVersionProperty.Name}, attribute is not a int for data model, {ClientReflectorCache<T>.Type.Name}");

            decrypter = null;
            foreach (T record in results)
            {
                decryptionVersion = (int?)keyVersionProperty.GetValue(record);
                if (decryptionVersion is not null && decrypters.Keys.Contains(decryptionVersion.Value))
                {
                    decrypter = decryptionVersion is not null ? decrypters.Decrypter(decryptionVersion.Value) : null;

                    if (decrypter is not null)
                    {
                        foreach (PropertyInfo property in encryptedProperties)
                        {
                            string? value = property.GetValue(record)?.ToString();
                            if (value.IsNotNullOrWhiteSpace())
                            {
                                value = decrypter.Decrypt(value);
                                property.SetValue(record, value);
                            }
                        }
                    }
                }
                if(decrypter is null)
                {
                    foreach (PropertyInfo property in encryptedProperties)
                    {
                        string? value = property.GetValue(record)?.ToString();
                        if (value.IsNullOrWhiteSpace())
                        {
                            throw new Exception($"No encryption key found for {ClientReflectorCache<T>.Type.Name}.{property.Name}");
                        }
                    }
                }
            }
        }

        return results;
    }
}
