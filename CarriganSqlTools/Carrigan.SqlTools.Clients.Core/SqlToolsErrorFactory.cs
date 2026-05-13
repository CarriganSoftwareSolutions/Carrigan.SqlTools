using Carrigan.SqlTools.Clients.Core.Exceptions;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using System.Data.Common;

namespace Carrigan.SqlTools.Clients.Core;

internal static class SqlToolsErrorFactory
{
    internal static ConnectionFailedException ConnectionFailed(string friendlyName, Exception exception) =>
        new(friendlyName, exception);

    internal static CommandExecutionFailedException ExecutionFailed(string operation, SqlQuery query, Exception exception) =>
        new(operation, query, exception);

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
        exception is SqlToolsQueryException;
}
