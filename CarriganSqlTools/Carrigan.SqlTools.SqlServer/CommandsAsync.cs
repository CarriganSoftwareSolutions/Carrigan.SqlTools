using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Reflection;


namespace Carrigan.SqlTools.SqlServer;

public static class CommandsAsync
{
    public async static Task TestConnectionStringAsync(string connectionString, string friendlyName)
    {
        try
        {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"{friendlyName}: Connection could not be established.", ex);
        }
    }

    public async static Task<int> ExecuteNonQueryAsync(SqlQuery query, DbTransaction? transaction, DbConnection connection)
    {
        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            await connection.OpenAsync();
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
            command.Parameters.AddRange(query.Parameters.AsEnumerable().Select(parameter => new SqlParameter(parameter.Key, parameter.Value)).ToArray());

            return await command.ExecuteNonQueryAsync();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
    }
    public async static Task<object?> ExecuteScalarAsync(SqlQuery query, DbTransaction? transaction, DbConnection connection)
    {
        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            await connection.OpenAsync();
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
            command.Parameters.AddRange(query.Parameters.AsEnumerable().Select(parameter => new SqlParameter(parameter.Key, parameter.Value)).ToArray());

            return await command.ExecuteScalarAsync();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public async static Task<IEnumerable<T>> ExecuteReaderAsync<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecryptors decryptors) where T : class?, new()
    {
        Type type = typeof(T);
        List<T> results = [];
        Invoker<T> invocator = new();
        List<Task<T>> invocationTasks = [];
        int? decryptionVersion = 1; //in later versions this will be read from a field marked by a custom annotation attribute, due time constraints, for now it will just be hard coded
        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            await connection.OpenAsync();
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
            command.Parameters.AddRange(query.Parameters.AsEnumerable().Select(parameter => new SqlParameter(parameter.Key, parameter.Value)).ToArray());

            using DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                Dictionary<string, object?> rowData = [];
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    rowData.Add(dataReader.GetName(i), dataReader.GetValue(i));
                }

                invocationTasks.Add(Task.Run(() => invocator.Invoke(rowData)));
            }
            results = [.. await Task.WhenAll(invocationTasks)];
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        if (ClientReflectorCache<T>.EncryptedProperties.Any())
        {
            _ = ClientReflectorCache<T>.KeyVersionProperty ?? throw new NullReferenceException($"The class, {type.Name}, has encrypted properties, but no key version field.");
            foreach (T record in results)
            {
                decryptionVersion = (int?) ClientReflectorCache<T>.KeyVersionProperty.GetValue(record);
                if (decryptionVersion is not null && decryptors.Keys.Contains(decryptionVersion.Value))
                {
                    IEncryption? decryptor = decryptionVersion is not null ? decryptors.Decryptor(decryptionVersion.Value) : null;

                    foreach (PropertyInfo property in ClientReflectorCache<T>.EncryptedProperties)
                    {
                        string? value = property.GetValue(record)?.ToString();
                        if (value.IsNotNullOrWhiteSpace() && decryptor is not null)
                        {
                            value = decryptor.Decrypt(value);
                            property.SetValue(record, value);
                        }
                    }
                }
            }
        }
        
        return results;
    }
}
