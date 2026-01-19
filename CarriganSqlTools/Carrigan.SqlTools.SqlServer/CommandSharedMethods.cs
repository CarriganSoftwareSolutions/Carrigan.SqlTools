using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;
//IGNORE SPELLING: xml

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Provides shared command functionality used by synchronous and asynchronous command executors, 
/// <see cref="Commands"/> and <see cref="CommandsAsync"/>.
/// </summary>
/// <remarks>
/// This type centralizes:
/// <list type="bullet">
///     <item><description>DbCommand construction from a <see cref="SqlQuery"/>.</description></item>
///     <item><description>Row-to-model materialization via <see cref="Invoker{T}"/>.</description></item>
///     <item><description>Post-processing such as field decryption.</description></item>
/// </list>
/// </remarks>
internal static class CommandSharedMethods
{
    /// <summary>
    /// Creates a <see cref="DbCommand"/> populated from a <see cref="SqlQuery"/>.
    /// </summary>
    /// <param name="query">The query text, command type, and parameters to apply.</param>
    /// <param name="connection">The connection used to create the command.</param>
    /// <param name="transaction">The transaction to associate with the command, if any.</param>
    /// <returns>
    /// A fully configured <see cref="DbCommand"/> with <see cref="DbCommand.CommandText"/>,
    /// <see cref="DbCommand.CommandType"/>, and parameters applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="query"/> or <paramref name="connection"/> is <see langword="null"/>.
    /// </exception>
    public static DbCommand CreateCommand(SqlQuery query, DbConnection connection, DbTransaction? transaction = null)
    {
        DbCommand command = connection.CreateCommand();
        if (transaction != null)
        {
            command.Transaction = transaction;
        }
        command.CommandText = query.QueryText;
        command.CommandType = query.CommandType;
        command.Parameters.AddRange(query.GetParameterCollection().ToArray());

        return command;
    }

    /// <summary>
    /// Reads the current record from a <see cref="DbDataReader"/> and materializes it into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type to materialize.</typeparam>
    /// <param name="dataReader">The data reader positioned on the record to read.</param>
    /// <returns>A new instance of <typeparamref name="T"/> populated with values from the current record.</returns>
    /// <remarks>
    /// Values are read column-by-column into a dictionary keyed by column name and then passed to
    /// <see cref="Invoker{T}.Invoke(Dictionary{string, object?})"/>.
    ///
    /// <para>
    /// Special handling is applied for SQL Server <c>xml</c> typed columns: the value is read as a string and converted
    /// into a <see cref="SqlXml"/> instance for downstream consumers.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataReader"/> is <see langword="null"/>.</exception>
    /// <exception cref="DataReaderFailedException">
    /// Thrown when a value cannot be read from the reader for the current record.
    /// </exception>
    /// <exception cref="RecordMaterializationException">
    /// Thrown when the row dictionary cannot be materialized into <typeparamref name="T"/> via <see cref="Invoker{T}"/>.
    /// </exception>
    internal static T ReadRecord<T>(DbDataReader dataReader) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(dataReader);

        Dictionary<string, object?> rowData = [];
        int? currentOrdinal = null;
        string? currentColumnName = null;

        try
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                currentOrdinal = i;
                currentColumnName = dataReader.GetName(i);

                string dataTypeName = dataReader.GetDataTypeName(i);

                if (dataReader.IsDBNull(i))
                    rowData.Add(currentColumnName, DBNull.Value);
                else if (string.Equals(dataTypeName, "xml", StringComparison.OrdinalIgnoreCase))
                    rowData.Add(currentColumnName, new SqlXml(XmlReader.Create(new StringReader(dataReader.GetString(i)))));
                else
                    rowData.Add(currentColumnName, dataReader.GetValue(i));
            }
        }
        catch (Exception exception) when (SqlToolsSqlServerErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsSqlServerErrorFactory.ReadFailed(typeof(T), currentOrdinal, currentColumnName, exception);
        }

        try
        {
            return Invoker<T>.Invoke(rowData);
        }
        catch (Exception exception) when (SqlToolsSqlServerErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsSqlServerErrorFactory.MaterializationFailed(typeof(T), rowData.Keys, exception);
        }
    }

    /// <summary>
    /// Decrypts encrypted properties on the provided records in-place.
    /// </summary>
    /// <typeparam name="T">The model type containing encrypted properties.</typeparam>
    /// <param name="results">The records to decrypt.</param>
    /// <param name="decrypters">The decrypter provider used to resolve decryption keys by key version.</param>
    /// <remarks>
    /// If <typeparamref name="T"/> contains properties marked as encrypted, a decrypter provider is required.
    /// The key version is read from the property identified by <see cref="ClientReflectorCache{T}.KeyVersionProperty"/>.
    /// When a matching decrypter is found, each encrypted property value is decrypted and assigned back to the record.
    /// </remarks>
    /// <exception cref="DecrypterNotProvided{T}">
    /// Thrown when encrypted properties exist on <typeparamref name="T"/>, but <paramref name="decrypters"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NoKeyVersionException{T}">
    /// Thrown when encrypted properties exist, but <typeparamref name="T"/> does not define a key-version property.
    /// </exception>
    /// <exception cref="MissingDecryptionKeyException{T}">
    /// Thrown when an encrypted property contains a value but no matching decrypter exists for the record's key version.
    /// </exception>
    /// <exception cref="DecryptionFailedException{T}">
    /// Thrown when an encrypted property fails to decrypt using the resolved decrypter.
    /// </exception>
    internal static void DecryptFields<T>(List<T> results, IDecrypters? decrypters) where T : class, new()
    {
        int? decryptionVersion = 1;
        IEncryption? decrypter = null;
        if (ClientReflectorCache<T>.EncryptedProperties.Any())
        {
            if (decrypters is null)
                throw SqlToolsSqlServerErrorFactory.DecrypterNotProvided<T>();

            _ = ClientReflectorCache<T>.KeyVersionProperty ?? throw new NoKeyVersionException<T>();
            foreach (T record in results)
            {
                decrypter = null;

                decryptionVersion = (int?)ClientReflectorCache<T>.KeyVersionProperty.GetValue(record);
                if (decryptionVersion is not null && decrypters.Keys.Contains(decryptionVersion.Value))
                {
                    decrypter = decrypters.Decrypter(decryptionVersion.Value);

                    foreach (PropertyInfo property in ClientReflectorCache<T>.EncryptedProperties)
                    {
                        string? value = property.GetValue(record)?.ToString();
                        if (value.IsNotNullOrWhiteSpace() && decrypter is not null)
                        {
                            try
                            {
                                value = decrypter.Decrypt(value);
                            }
                            catch (Exception exception) when (SqlToolsSqlServerErrorFactory.IsAlreadyWrapped(exception) is false)
                            {
                                throw SqlToolsSqlServerErrorFactory.DecryptionFailed<T>(decryptionVersion.Value, property.Name, exception);
                            }

                            property.SetValue(record, value);
                        }
                    }
                }

                if (decrypter is null)
                {
                    foreach (PropertyInfo property in ClientReflectorCache<T>.EncryptedProperties)
                    {
                        string? value = property.GetValue(record)?.ToString();
                        if (value.IsNotNullOrWhiteSpace())
                        {
                            throw SqlToolsSqlServerErrorFactory.MissingDecryptionKey<T>(decryptionVersion, property.Name);
                        }
                    }
                }
            }
        }
    }
}
