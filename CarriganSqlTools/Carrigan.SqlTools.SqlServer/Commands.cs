using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.Exceptions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

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
    /// <exception cref="ConnectionFailedException">Thrown if a connection cannot be established.</exception>
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
        catch (Exception exception)
        {
            throw SqlToolsSqlServerErrorFactory.ConnectionFailed(friendlyName, exception);
        }
    }

    /// <summary>
    /// Executes an ADO.NET non-query command (for example, INSERT/UPDATE/DELETE) using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The number of rows affected.</returns>
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
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
        catch (Exception exception) when (SqlToolsSqlServerErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsSqlServerErrorFactory.ExecutionFailed(nameof(ExecuteNonQuery), query, connection, transaction, exception);
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
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
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
        catch (Exception exception) when (SqlToolsSqlServerErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsSqlServerErrorFactory.ExecutionFailed(nameof(ExecuteScalar), query, connection, transaction, exception);
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
    /// <exception cref="NoKeyVersionException{T}">Thrown when encrypted properties exist, but the type does not define a key-version property.</exception>
    /// <exception cref="MissingDecryptionKeyException{T}">Thrown when encrypted properties contain values, but no matching decryption key can be found.</exception>
    /// <exception cref="DecryptionFailedException{T}">Thrown when decryption fails for an encrypted property.</exception>
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
    /// <exception cref="DataReaderFailedException">Thrown when reading the data reader fails.</exception>
    /// <exception cref="RecordMaterializationException">Thrown when materializing a record into <typeparamref name="T"/> fails.</exception>
    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters = null) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        if (ClientReflectorCache<T>.EncryptedProperties.Any() && decrypters is null)
        {
            throw SqlToolsSqlServerErrorFactory.DecrypterNotProvided<T>();
        }

        List<T> results = [];
        bool wasClosed = false;

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
        catch (Exception exception) when (SqlToolsSqlServerErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsSqlServerErrorFactory.ExecutionFailed(nameof(ExecuteReader), query, connection, transaction, exception);
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
