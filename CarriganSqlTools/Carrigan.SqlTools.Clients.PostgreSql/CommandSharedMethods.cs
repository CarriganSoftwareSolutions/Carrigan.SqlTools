using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Clients.Core;
using Carrigan.SqlTools.Clients.Core.Exceptions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;

namespace Carrigan.SqlTools.Clients.PostgreSql;

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
    public static NpgsqlCommand CreateCommand(SqlQuery query, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        NpgsqlCommand command = connection.CreateCommand();
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
    /// Special handling is applied for PostgreSQL <c>xml</c> typed columns: the value is read as a string and converted
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
                else if (TryReadNullableArrayValue<T>(dataReader, i, new (currentColumnName), dataTypeName, out object? arrayValue))
                    rowData.Add(currentColumnName, arrayValue);
                else if (string.Equals(dataTypeName, "xml", StringComparison.OrdinalIgnoreCase))
                {
                    using StringReader stringReader = new(dataReader.GetString(i));
                    using XmlReader xmlReader = XmlReader.Create(stringReader);
                    rowData.Add(currentColumnName, new SqlXml(xmlReader));
                }
                else
                    rowData.Add(currentColumnName, dataReader.GetValue(i));
            }
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.ReadFailed(typeof(T), currentOrdinal, currentColumnName, exception);
        }

        try
        {
            return Invoker<T>.Invoke(rowData);
        }
        catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
        {
            throw SqlToolsErrorFactory.MaterializationFailed(typeof(T), rowData.Keys, exception);
        }
    }

    /// <summary>
    /// Attempts to read a value from the data reader as a nullable array of the appropriate type based on the target property type
    /// and the data type name reported by the reader.
    /// </summary>
    /// <typeparam name="T">
    /// The model type being materialized, used to determine the target property type for the current column.
    /// </typeparam>
    /// <param name="dataReader">
    /// The data reader to read the value from, positioned on the record containing the value to read.
    /// </param>
    /// <param name="ordinal">
    /// The ordinal of the column to read, used for value retrieval from the reader.
    /// </param>
    /// <param name="columnName">
    /// The name of the column to read.
    /// </param>
    /// <param name="dataTypeName">
    /// The data type name reported by the reader.
    /// </param>
    /// <param name="value">
    /// The value read from the data reader, if successful.
    /// </param>
    /// <returns>
    /// True if the value was successfully read, false otherwise.
    /// </returns>
    private static bool TryReadNullableArrayValue<T>(DbDataReader dataReader, int ordinal, ResultColumnName columnName, string dataTypeName, out object? value) where T : class, new()
    {
        value = null;

        if (InvocationReflectorCache<T>.Exists(columnName) is false || InvocationReflectorCache<T>.IsArray(columnName) is false)
            return false;
        else
        {
            Type type = InvocationReflectorCache<T>.GetType(columnName);
            Type? elementType = type.GetElementType();
            if (type == typeof(byte[]) || elementType is null || IsNullableValueType(elementType) is false)
                return false;
            else
            {
                foreach (Type readType in GetNullableArrayReadTypes(type, dataTypeName))
                {
                    try
                    {
                        value = GetFieldValue(dataReader, ordinal, readType);
                        return true;
                    }
                    catch (InvalidCastException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                    catch (TargetInvocationException exception) when (exception.InnerException is InvalidCastException or NotSupportedException)
                    {
                    }
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Determines the appropriate nullable array types to attempt reading from the data reader based on the target array type and the data type name.
    /// </summary>
    /// <param name="targetArrayType">
    /// The target array type for which to determine appropriate read types.
    /// </param>
    /// <param name="dataTypeName">
    /// The data type name reported by the reader.
    /// </param>
    /// <returns>
    /// An enumerable of types that can be used to read the nullable array value.
    /// </returns>
    private static IEnumerable<Type> GetNullableArrayReadTypes(Type targetArrayType, string dataTypeName)
    {
        Type? providerArrayType = GetProviderNullableArrayReadType(dataTypeName);

        if (providerArrayType is not null)
            yield return providerArrayType;

        if (providerArrayType != targetArrayType)
            yield return targetArrayType;
    }

    /// <summary>
    /// Maps PostgreSQL array data type names to their corresponding nullable array CLR types for reading from the data reader.
    /// </summary>
    /// <param name="dataTypeName">
    /// The data type name reported by the reader.
    /// </param>
    /// <returns>
    /// The corresponding nullable array CLR type, or null if no match is found.
    /// </returns>
    private static Type? GetProviderNullableArrayReadType(string dataTypeName)
    {
        string normalized = NormalizePostgreSqlArrayTypeName(dataTypeName);

        return normalized switch
        {
            "uuid" or "_uuid" => typeof(Guid?[]),
            "boolean" or "bool" or "_bool" => typeof(bool?[]),
            "smallint" or "int2" or "_int2" => typeof(short?[]),
            "integer" or "int" or "int4" or "_int4" => typeof(int?[]),
            "bigint" or "int8" or "_int8" => typeof(long?[]),
            "real" or "float4" or "_float4" => typeof(float?[]),
            "double precision" or "float8" or "_float8" => typeof(double?[]),
            "numeric" or "decimal" or "money" or "_numeric" or "_money" => typeof(decimal?[]),
            "date" or "_date" => typeof(DateOnly?[]),
            "time without time zone" or "time" or "_time" => typeof(TimeOnly?[]),
            "interval" or "_interval" => typeof(TimeSpan?[]),
            "timestamp without time zone" or "timestamp" or "_timestamp" => typeof(DateTime?[]),
            "timestamp with time zone" or "timestampz" or "timestamptz" or "_timestamptz" => typeof(DateTime?[]),
            "character" or "char" or "bpchar" or "character varying" or "varchar" or "text" or "xml" or "_bpchar" or "_varchar" or "_text" or "_xml" => typeof(string[]),
            "bytea" or "_bytea" => typeof(byte[][]),
            _ => null
        };
    }

    /// <summary>
    /// Normalizes PostgreSQL array data type names by removing array indicators, trimming whitespace, and converting to lowercase for consistent comparison.
    /// </summary>
    /// <param name="dataTypeName">
    /// The data type name to normalize.
    /// </param>
    /// <returns>
    /// The normalized data type name.
    /// </returns>
    private static string NormalizePostgreSqlArrayTypeName(string dataTypeName)
    {
        string normalized = dataTypeName.Trim().ToLowerInvariant();

        if (normalized.EndsWith("[]", StringComparison.Ordinal))
            normalized = normalized[..^2].Trim();

        if (normalized.Length > 0 && normalized[0] == '_')
            return normalized;

        int openParenthesisIndex = normalized.IndexOf('(', StringComparison.Ordinal);
        if (openParenthesisIndex >= 0)
        {
            int closeParenthesisIndex = normalized.IndexOf(')', openParenthesisIndex + 1);
            if (closeParenthesisIndex > openParenthesisIndex)
                normalized = string.Concat(normalized.AsSpan(0, openParenthesisIndex), normalized.AsSpan(closeParenthesisIndex + 1)).Trim();
        }

        return normalized;
    }

    /// <summary>
    /// Uses reflection to invoke the generic <see cref="DbDataReader.GetFieldValue{T}(int)"/> method for the specified field type. 
    /// </summary>
    /// <param name="dataReader">
    /// The data reader to read the value from, positioned on the record containing the value to read.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal.
    /// </param>
    /// <param name="fieldType">
    /// The type of the field to read.
    /// </param>
    /// <returns>
    /// The value of the specified field.
    /// </returns>
    /// <exception cref="MissingMethodException"></exception>
    private static object? GetFieldValue(DbDataReader dataReader, int ordinal, Type fieldType)
    {
        MethodInfo method = typeof(DbDataReader)
            .GetMethod(nameof(DbDataReader.GetFieldValue), [typeof(int)])
            ?? throw new MissingMethodException(typeof(DbDataReader).FullName, nameof(DbDataReader.GetFieldValue));

        return method.MakeGenericMethod(fieldType).Invoke(dataReader, [ordinal]);
    }

    /// <summary>
    /// Determines if the provided type is a nullable value type, i.e. a <see cref="Nullable{T}"/> where T is a struct.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    /// <see langword="true"/> if the type is a nullable value type; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool IsNullableValueType(Type type) =>
        type.IsValueType && Nullable.GetUnderlyingType(type) is not null;

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
                throw SqlToolsErrorFactory.DecrypterNotProvided<T>();

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
                            catch (Exception exception) when (SqlToolsErrorFactory.IsAlreadyWrapped(exception) is false)
                            {
                                throw SqlToolsErrorFactory.DecryptionFailed<T>(decryptionVersion.Value, property.Name, exception);
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
                            throw SqlToolsErrorFactory.MissingDecryptionKey<T>(decryptionVersion, property.Name);
                        }
                    }
                }
            }
        }
    }
}
