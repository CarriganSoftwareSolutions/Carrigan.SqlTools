using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Xml;


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
            command.Parameters.AddRange(query.GetParameterCollection().ToArray());

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
            command.Parameters.AddRange(query.GetParameterCollection().ToArray());

            return await command.ExecuteScalarAsync();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
    public async static Task<IEnumerable<T>> ExecuteReaderAsync<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection) where T : class?, new() =>
        await ExecuteReaderAsync<T>(query, transaction, connection, null);

    public async static Task<IEnumerable<T>> ExecuteReaderAsync<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters) where T : class?, new()
    {
        Type type = typeof(T);
        List<T> results = [];
        List<Task<T>> invocationTasks = [];
        int? decryptionVersion = 1; //in later versions this will be read from a property marked by a custom annotation attribute, due time constraints, for now it will just be hard coded
        bool wasClosed = false;
        string dataTypeName;

        if (ClientReflectorCache<T>.EncryptedProperties.Any() && decrypters is null)
        {
            throw new DecrypterNotProvided<T>();
        }

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
            command.Parameters.AddRange(query.GetParameterCollection().ToArray());

            using DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (await dataReader.ReadAsync())
            {
                Dictionary<string, object?> rowData = [];
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    dataTypeName = dataReader.GetDataTypeName(i);
                    if(dataReader.IsDBNull(i))
                        rowData.Add(dataReader.GetName(i), DBNull.Value);
                    else if (string.Equals(dataTypeName, "xml", StringComparison.OrdinalIgnoreCase))
                        rowData.Add(dataReader.GetName(i), new SqlXml(XmlReader.Create(new StringReader(dataReader.GetString(i)))));
                    else
                        rowData.Add(dataReader.GetName(i), dataReader.GetValue(i));
                }

                invocationTasks.Add(Task.Run(() => Invoker<T>.Invoke(rowData)));
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
            if (decrypters is null)
                throw new DecrypterNotProvided<T>();

            _ = ClientReflectorCache<T>.KeyVersionProperty ?? throw new NoKeyVersionPropertyException<T>();
            foreach (T record in results)
            {
                decryptionVersion = (int?) ClientReflectorCache<T>.KeyVersionProperty.GetValue(record);
                if (decryptionVersion is not null && decrypters.Keys.Contains(decryptionVersion.Value))
                {
                    IEncryption? decrypter = decryptionVersion is not null ? decrypters.Decrypter(decryptionVersion.Value) : null;

                    foreach (PropertyInfo property in ClientReflectorCache<T>.EncryptedProperties)
                    {
                        string? value = property.GetValue(record)?.ToString();
                        if (value.IsNotNullOrWhiteSpace() && decrypter is not null)
                        {
                            value = decrypter.Decrypt(value);
                            property.SetValue(record, value);
                        }
                    }
                }
            }
        }
        
        return results;
    }
}
