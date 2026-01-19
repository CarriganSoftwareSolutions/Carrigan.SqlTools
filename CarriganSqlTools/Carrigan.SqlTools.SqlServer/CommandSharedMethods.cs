using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Transactions;
using System.Xml;
//IGNORE SPELLING: xml

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Provides command functionality used by <see cref="Commands"/> and <see cref="CommandsAsync"/>
/// </summary>
internal static class CommandSharedMethods
{
    /// <summary>
    /// Builds an <see cref="DBCommand"/> object
    /// </summary>
    /// <param name="query">An SQL Generated from Carrigan.SqlTools</param>
    /// <param name="connection">a connection</param>
    /// <param name="transaction">a transaction</param>
    /// <returns>an <see cref="DBCommand"/> object</returns>
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
    /// Reads individual records fields by field, performs type mapping, property mapping and returns the value in a newly invoked object.
    /// </summary>
    /// <param name="dataReader">a DbDataReader</param>
    /// <returns>an instance of type <see cref="{T}"/> with the values read from the database for a given record</returns>    
    internal static T ReadRecord<T>(DbDataReader dataReader) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(dataReader);

        Dictionary<string, object?> rowData = [];
        string dataTypeName;
        for (int i = 0; i < dataReader.FieldCount; i++)
        {
            dataTypeName = dataReader.GetDataTypeName(i);
            if (dataReader.IsDBNull(i))
                rowData.Add(dataReader.GetName(i), DBNull.Value);
            else if (string.Equals(dataTypeName, "xml", StringComparison.OrdinalIgnoreCase))
                rowData.Add(dataReader.GetName(i), new SqlXml(XmlReader.Create(new StringReader(dataReader.GetString(i)))));
            else
                rowData.Add(dataReader.GetName(i), dataReader.GetValue(i));
        }
        return Invoker<T>.Invoke(rowData);
    }

    /// <summary>
    /// Performs field level decryption, if the necessary, and modifies the record.
    /// </summary>
    /// <typeparam name="T">the object type of the record</typeparam>
    /// <param name="results">the list to modify</param>
    /// <param name="decrypters">decryption interface</param>
    /// <exception cref="DecrypterNotProvided{T}">throw if a field is defined for encryption and no <see cref="IDecrypters"/> is provided</exception>
    /// <exception cref="NoKeyVersionException{T}">throw if a field is defined for encryption and no type doesn't have a keyversion field is defined for the class</exception>
    /// <exception cref="Exception"></exception>
    internal static void DecryptFields<T>(List<T> results, IDecrypters? decrypters) where T : class, new()
    {
        int? decryptionVersion = 1;
        IEncryption? decrypter = null;
        if (ClientReflectorCache<T>.EncryptedProperties.Any())
        {
            if (decrypters is null)
                throw new DecrypterNotProvided<T>();

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
                            value = decrypter.Decrypt(value);
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
                            throw new Exception($"No encryption key found for {ClientReflectorCache<T>.Type.Name}.{property.Name}");
                        }
                    }
                }
            }
        }
    }
}
