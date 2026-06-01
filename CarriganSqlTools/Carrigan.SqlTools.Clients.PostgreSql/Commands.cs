using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Clients.Core;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using System.Data;

namespace Carrigan.SqlTools.Clients.PostgreSql;

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
            using NpgsqlConnection connection = new(connectionString);
            connection.Open();
            connection.Close();
        }
        catch (Exception exception)
        {
            throw SqlToolsErrorFactory.ConnectionFailed(friendlyName, exception);
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
    public static int ExecuteNonQuery(IQueryBuilder query, NpgsqlTransaction? transaction, NpgsqlConnection connection) =>
        ExecuteNonQuery(query.AsSqlQuery(), transaction, connection);

    /// <summary>
    /// Executes an ADO.NET non-query command (for example, INSERT/UPDATE/DELETE) using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The number of rows affected.</returns>
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
    public static int ExecuteNonQuery(SqlQuery query, NpgsqlTransaction? transaction, NpgsqlConnection connection)
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
            using NpgsqlCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            return command.ExecuteNonQuery();
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ExecutionFailed(nameof(ExecuteNonQuery), query, exception);
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
    public static object? ExecuteScalar(IQueryBuilder query, NpgsqlTransaction? transaction, NpgsqlConnection connection) =>
        ExecuteScalar(query.AsSqlQuery(), transaction, connection);

    /// <summary>
    /// Executes an ADO.NET scalar command using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The first column of the first row in the result set, or <see langword="null"/> if the result set is empty.</returns>
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
    public static object? ExecuteScalar(SqlQuery query, NpgsqlTransaction? transaction, NpgsqlConnection connection)
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
            using NpgsqlCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            return command.ExecuteScalar();
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ExecutionFailed(nameof(ExecuteScalar), query, exception);
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
    public static IEnumerable<T> ExecuteReader<T>(IQueryBuilder query, NpgsqlTransaction? transaction, NpgsqlConnection connection, IDecrypters? decrypters = null) where T : class, new() =>
        ExecuteReader<T>(query.AsSqlQuery(), transaction, connection, decrypters);

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
    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, NpgsqlTransaction? transaction, NpgsqlConnection connection, IDecrypters? decrypters = null) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        if (ClientReflectorCache<T>.EncryptedProperties.Any() && decrypters is null)
        {
            throw SqlToolsErrorFactory.DecrypterNotProvided<T>();
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
            using NpgsqlCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);
            using NpgsqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                results.Add(CommandSharedMethods.ReadRecord<T>(dataReader));
            }
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ExecutionFailed(nameof(ExecuteReader), query, exception);
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
