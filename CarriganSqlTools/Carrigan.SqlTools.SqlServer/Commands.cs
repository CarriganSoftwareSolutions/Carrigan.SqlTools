using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
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

public static class Commands
{
    public static void TestConnectionString(string connectionString, string friendlyName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(friendlyName);

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
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

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
            command.Parameters.AddRange(query.GetParameterCollection().ToArray());

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
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

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
            command.Parameters.AddRange(query.GetParameterCollection().ToArray());

            return command.ExecuteScalar();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection) where T : class, new() =>
        ExecuteReader<T>(query, transaction, connection, null);

    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        List<T> results = [];
        PropertyInfo? keyVersionProperty = ClientReflectorCache<T>.KeyVersionProperty;
        IEnumerable<PropertyInfo> encryptedProperties = ClientReflectorCache<T>.EncryptedProperties;
        bool wasClosed = false;

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
            command.Parameters.AddRange(query.GetParameterCollection().ToArray());

            using DbDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                results.Add(Invoker<T>.Invoke(CommandSharedMethods.ReadRecord(dataReader)));
            }
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
        CommandSharedMethods.ProcessResults(results, decrypters);
        return results;
    }
}
