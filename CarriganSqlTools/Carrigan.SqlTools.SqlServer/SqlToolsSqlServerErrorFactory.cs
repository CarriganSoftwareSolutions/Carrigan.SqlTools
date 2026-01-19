using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.Exceptions;
using System.Data.Common;

namespace Carrigan.SqlTools.SqlServer;

internal static class SqlToolsSqlServerErrorFactory
{
    internal static ConnectionFailedException ConnectionFailed(string friendlyName, Exception exception) =>
        new(friendlyName, exception);

    internal static CommandExecutionFailedException ExecutionFailed(string operation, SqlQuery query, DbConnection connection, DbTransaction? transaction, Exception exception) =>
        new(operation, query, connection, transaction, exception);

    internal static DataReaderFailedException ReadFailed(Type modelType, int? ordinal, string? columnName, Exception exception) =>
        new(modelType, ordinal, columnName, exception);

    internal static RecordMaterializationException MaterializationFailed(Type modelType, IEnumerable<string> columnNames, Exception exception) =>
        new(modelType, columnNames, exception);

    internal static DecrypterNotProvided<T> DecrypterNotProvided<T>() =>
        new();

    internal static MissingDecryptionKeyException<T> MissingDecryptionKey<T>(int? keyVersion, string propertyName) =>
        new(keyVersion, propertyName);

    internal static DecryptionFailedException<T> DecryptionFailed<T>(int keyVersion, string propertyName, Exception exception) =>
        new(keyVersion, propertyName, exception);

    internal static bool IsAlreadyWrapped(Exception exception) =>
        exception is SqlToolsSqlServerException;
}
