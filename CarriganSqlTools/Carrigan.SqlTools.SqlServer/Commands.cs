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

/// <summary>
/// Provides methods to execute various ADO.NET commands using <see cref="SqlQuery"/> synchronously.
/// </summary>
public static class Commands
{
    /// <summary>
    /// Attempts to open and close a SQL connection to validate the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="friendlyName">A friendly name included in the exception message when a connection cannot be established.</param>
    /// <exception cref="Exception">Thrown if a connection cannot be established.</exception>
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

    /// <summary>
    /// Executes an ADO.NET non-query command (for example, INSERT/UPDATE/DELETE) using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The number of rows affected.</returns>
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
            using DbCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            return command.ExecuteNonQuery();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
    /// <summary>
    /// Executes an ADO.NET scalar command using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The first column of the first row in the result set, or <see langword="null"/> if the result set is empty.</returns>
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
            using DbCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            return command.ExecuteScalar();
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    /// <summary>
    /// Executes an ADO.NET reader command using a <see cref="SqlQuery"/> and materializes all records.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <param name="decrypters">Optional decrypter provider used to decrypt properties marked as encrypted.</param>
    /// <returns>A sequence of records read from the database.</returns>
    /// <exception cref="DecrypterNotProvided{T}">Thrown when one or more encrypted properties exist, but no decrypter provider is supplied.</exception>
    /// <exception cref="NoKeyVersionException{T}">Thrown when encrypted properties exist, but the type does not define a key-version property (for example, a property marked with <c>[KeyVersion]</c>).</exception>
    /// <exception cref="Exception">Thrown when encrypted properties contain values, but no matching decryption key can be found.</exception>
    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters = null) where T : class, new()
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
            using DbCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            using DbDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                results.Add(CommandSharedMethods.ReadRecord<T>(dataReader));
            }
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
        CommandSharedMethods.DecryptFields(results, decrypters);
        return results;
    }
}
