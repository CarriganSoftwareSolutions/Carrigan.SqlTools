﻿using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;


//IGNORE Spelling: xml

namespace Carrigan.SqlTools.SqlServer;

public static class CommandsAsync
{
    public async static Task TestConnectionStringAsync(string connectionString, string friendlyName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(friendlyName);

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
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

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
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

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
                await connection.CloseAsync();
        }
    }
    public async static Task<IEnumerable<T>> ExecuteReaderAsync<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection) where T : class, new() =>
        await ExecuteReaderAsync<T>(query, transaction, connection, null);


    public async static Task<IEnumerable<T>> ExecuteReaderAsync<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        List<T> results = [];
        List<Task<T>> invocationTasks = [];
        bool wasClosed = false;

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
                invocationTasks.Add(Task.Run(() => Invoker<T>.Invoke(CommandSharedMethods.ReadRecord(dataReader))));
            }
            results = [.. await Task.WhenAll(invocationTasks)];
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
        CommandSharedMethods.ProcessResults(results, decrypters);
        return results;
    }
}
