using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Clients.Core;
using Carrigan.SqlTools.Clients.Core.Exceptions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using System.Data;

namespace Carrigan.SqlTools.Clients.PostgreSql;

/// <summary>
/// Provides methods to execute various ADO.NET commands using <see cref="SqlQuery"/> asynchronously.
/// </summary>
public static class CommandsAsync
{
    /// <summary>
    /// Attempts to open and close a SQL connection to validate the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="friendlyName">A friendly name included in the exception message when a connection cannot be established.</param>
    /// <exception cref="ConnectionFailedException">Thrown if a connection cannot be established.</exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static async Task TestConnectionStringAsync(string connectionString, string friendlyName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(friendlyName);

        try
        {
            using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            await connection.CloseAsync().ConfigureAwait(false);
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
    public static async Task<int> ExecuteNonQueryAsync(IQueryBuilder query, NpgsqlTransaction? transaction, NpgsqlConnection connection) =>
        await ExecuteNonQueryAsync(query.AsSqlQuery(), transaction, connection);

    /// <summary>
    /// Executes an ADO.NET non-query command (for example, INSERT/UPDATE/DELETE) using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The number of rows affected.</returns>
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static async Task<int> ExecuteNonQueryAsync(SqlQuery query, NpgsqlTransaction? transaction, NpgsqlConnection connection)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            await connection.OpenAsync().ConfigureAwait(false);
        }

        try
        {
            using NpgsqlCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ExecutionFailed(nameof(ExecuteNonQueryAsync), query, exception);
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                await connection.CloseAsync().ConfigureAwait(false);
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
    public static async Task<object?> ExecuteScalarAsync(IQueryBuilder query, NpgsqlTransaction? transaction, NpgsqlConnection connection) =>
        await ExecuteScalarAsync(query.AsSqlQuery(), transaction, connection);

    /// <summary>
    /// Executes an ADO.NET scalar command using a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <param name="transaction">The transaction (optional).</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The first column of the first row in the result set, or <see langword="null"/> if the result set is empty.</returns>
    /// <exception cref="CommandExecutionFailedException">Thrown when command execution fails.</exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static async Task<object?> ExecuteScalarAsync(SqlQuery query, NpgsqlTransaction? transaction, NpgsqlConnection connection)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        bool wasClosed = false;
        if (connection.State != ConnectionState.Open)
        {
            wasClosed = true;
            await connection.OpenAsync().ConfigureAwait(false);
        }

        try
        {
            using NpgsqlCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);

            return await command.ExecuteScalarAsync().ConfigureAwait(false);
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ExecutionFailed(nameof(ExecuteScalarAsync), query, exception);
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                await connection.CloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Executes an ADO.NET reader command using a <see cref="SqlQuery"/> and materializes all records.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
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
    public static async Task<IEnumerable<T>> ExecuteReaderAsync<T>(IQueryBuilder query, NpgsqlTransaction? transaction, NpgsqlConnection connection, IDecrypters? decrypters = null) where T : class, new() =>
        await ExecuteReaderAsync<T>(query.AsSqlQuery(), transaction, connection, decrypters);

    /// <summary>
    /// Executes an ADO.NET reader command using a <see cref="SqlQuery"/> and materializes all records.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
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
    public static async Task<IEnumerable<T>> ExecuteReaderAsync<T>(SqlQuery query, NpgsqlTransaction? transaction, NpgsqlConnection connection, IDecrypters? decrypters = null) where T : class, new()
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
            await connection.OpenAsync().ConfigureAwait(false);
        }

        try
        {
            using NpgsqlCommand command = CommandSharedMethods.CreateCommand(query, connection, transaction);
            using NpgsqlDataReader dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await dataReader.ReadAsync().ConfigureAwait(false))
            {
                results.Add(CommandSharedMethods.ReadRecord<T>(dataReader));
            }
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ExecutionFailed(nameof(ExecuteReaderAsync), query, exception);
        }
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                await connection.CloseAsync().ConfigureAwait(false);
        }

        CommandSharedMethods.DecryptFields(results, decrypters);
        return results;
    }
}
