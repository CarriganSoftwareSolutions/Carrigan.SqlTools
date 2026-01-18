using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Carrigan.SqlTools.SqlServer;

internal static class CommandSharedMethods
{
    internal static Dictionary<string, object?> ReadRecord(DbDataReader dataReader)
    {
        Dictionary<string, object?> rowData = [];
        string dataTypeName;
        for (int i = 0; i < dataReader.FieldCount; i++)
        {
            dataTypeName = dataReader.GetDataTypeName(i);
            if (dataReader.IsDBNull(i))
                rowData.Add(dataReader.GetName(i), DBNull.Value);
            else if (string.Equals(dataTypeName, "xml", StringComparison.OrdinalIgnoreCase))
            {
                object xmlValue = dataReader.GetValue(i);
                if (xmlValue is SqlXml sqlXml)
                    rowData.Add(dataReader.GetName(i), sqlXml);
                else if (xmlValue is string xmlString)
                    rowData.Add(dataReader.GetName(i), new SqlXml(XmlReader.Create(new StringReader(xmlString))));
                else
                    rowData.Add(dataReader.GetName(i), xmlValue);
            }
            else
                rowData.Add(dataReader.GetName(i), dataReader.GetValue(i));
        }
        return rowData;
    }

    internal static void ProcessResults<T>(List<T> results, IDecrypters? decrypters) where T : class, new()
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
