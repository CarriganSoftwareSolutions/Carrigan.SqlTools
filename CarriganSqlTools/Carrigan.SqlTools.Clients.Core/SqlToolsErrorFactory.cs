using Carrigan.SqlTools.Clients.Core.Exceptions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Clients.Core;

/// <summary>
/// Provides factory methods for creating SQL Tools client exception types.
/// </summary>
internal static class SqlToolsErrorFactory
{
    /// <summary>
    /// Creates an exception for a failed database connection.
    /// </summary>
    /// <param name="friendlyName">The friendly name of the connection that failed.</param>
    /// <param name="exception">The exception that caused the connection failure.</param>
    /// <returns>The wrapped connection failure exception.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="friendlyName"/> or <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    internal static ConnectionFailedException ConnectionFailed(string friendlyName, Exception exception) =>
        new(friendlyName, exception);

    /// <summary>
    /// Creates an exception for a failed SQL command execution.
    /// </summary>
    /// <param name="operation">The operation being performed when command execution failed.</param>
    /// <param name="query">The SQL query that failed to execute.</param>
    /// <param name="exception">The exception that caused command execution to fail.</param>
    /// <returns>The wrapped command execution failure exception.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="operation"/>, <paramref name="query"/>, or <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    internal static CommandExecutionFailedException ExecutionFailed(string operation, SqlQuery query, Exception exception) =>
        new(operation, query, exception);

    /// <summary>
    /// Creates an exception for a failed data reader operation.
    /// </summary>
    /// <param name="modelType">The model type being read.</param>
    /// <param name="ordinal">The column ordinal being read, or <see langword="null"/> when unavailable.</param>
    /// <param name="columnName">The column name being read, or <see langword="null"/> when unavailable.</param>
    /// <param name="exception">The exception that caused the read operation to fail.</param>
    /// <returns>The wrapped data reader failure exception.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="modelType"/> or <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    internal static DataReaderFailedException ReadFailed(Type modelType, int? ordinal, string? columnName, Exception exception) =>
        new(modelType, ordinal, columnName, exception);

    /// <summary>
    /// Creates an exception for a failed record materialization operation.
    /// </summary>
    /// <param name="modelType">The model type being materialized.</param>
    /// <param name="columnNames">The column names available during materialization.</param>
    /// <param name="exception">The exception that caused materialization to fail.</param>
    /// <returns>The wrapped record materialization failure exception.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="modelType"/>, <paramref name="columnNames"/>, or <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    internal static RecordMaterializationException MaterializationFailed(Type modelType, IEnumerable<string> columnNames, Exception exception) =>
        new(modelType, columnNames, exception);

    /// <summary>
    /// Creates an exception for a missing decrypter dependency.
    /// </summary>
    /// <typeparam name="T">The model type that requires decryption.</typeparam>
    /// <returns>The missing decrypter exception.</returns>
    internal static DecrypterNotProvided<T> DecrypterNotProvided<T>() =>
        new();

    /// <summary>
    /// Creates an exception for a missing decryption key.
    /// </summary>
    /// <typeparam name="T">The model type that requires decryption.</typeparam>
    /// <param name="keyVersion">The key version that was required, or <see langword="null"/> when unavailable.</param>
    /// <param name="propertyName">The property that required the missing key.</param>
    /// <returns>The missing decryption key exception.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyName"/> is <see langword="null"/>.
    /// </exception>
    internal static MissingDecryptionKeyException<T> MissingDecryptionKey<T>(int? keyVersion, string propertyName) =>
        new(keyVersion, propertyName);

    /// <summary>
    /// Creates an exception for a failed decryption operation.
    /// </summary>
    /// <typeparam name="T">The model type that requires decryption.</typeparam>
    /// <param name="keyVersion">The key version used during the failed decryption operation.</param>
    /// <param name="propertyName">The property being decrypted.</param>
    /// <param name="exception">The exception that caused decryption to fail.</param>
    /// <returns>The wrapped decryption failure exception.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertyName"/> or <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    internal static DecryptionFailedException<T> DecryptionFailed<T>(int keyVersion, string propertyName, Exception exception) =>
        new(keyVersion, propertyName, exception);

    /// <summary>
    /// Determines whether an exception has already been wrapped by a SQL Tools query exception.
    /// </summary>
    /// <param name="exception">The exception to inspect.</param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="exception"/> is a <see cref="SqlToolsQueryException"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool IsAlreadyWrapped(Exception exception) =>
        exception is SqlToolsQueryException;
}