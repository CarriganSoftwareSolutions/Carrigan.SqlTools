using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;

//IGNORE Spelling: xml

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// provides methods to execute various ado commands utilizing <see cref="SqlQuery"/>s synchronously
/// </summary>
public static class Commands
{
    /// <summary>
    /// Help method to test a connect, arguably useful for ensuring your connection strings work...
    /// This is just a fail safe to make sure the connection string for an application is set up correctly. 
    /// This provides a fail early error, in case things aren't set up correctly.
    /// I don't necessarily recommend using this, but I needed somewhere to put it for my own use.
    /// </summary>
    /// <param name="connectionString">a connection string</param>
    /// <param name="friendlyName">used for generating exceptions</param>
    /// <returns></returns>
    /// <exception cref="Exception">Thrown if a connection can't be established</exception>
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
        catch (Exception ex)
        {
            throw new Exception($"{friendlyName}: Connection could not be established.", ex);
        }
    }

    /// <summary>
    /// Provides a convenient way to execute an ADO.net NonQuery utilizing <see cref="SqlQuery"/>
    /// </summary>
    /// <param name="query">the query</param>
    /// <param name="transaction">the transaction, optionally null</param>
    /// <param name="connection">the connection</param>
    /// <returns>returns what the underlying ExecuteNonQuery returns, if successful</returns>
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
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
    /// <summary>
    /// Provides a convenient way to execute an ADO.net Scalar utilizing <see cref="SqlQuery"/>
    /// </summary>
    /// <param name="query">the query</param>
    /// <param name="transaction">the transaction, optionally null</param>
    /// <param name="connection">the connection</param>
    /// <returns>returns what the underlying ExecuteScalar returns, if successful</returns>
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
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    /// <summary>
    /// Provides a convenient way to execute an ADO.net Reader utilizing <see cref="SqlQuery"/>
    /// </summary>
    /// <param name="query">the query</param>
    /// <param name="transaction">the transaction, optionally null</param>
    /// <param name="connection">the connection</param>
    /// <returns>returns an IEnumerable<T> of records read using ADO.Net</returns>
    public static IEnumerable<T> ExecuteReader<T>(SqlQuery query, DbTransaction? transaction, DbConnection connection, IDecrypters? decrypters = null) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);

        List<T> results = [];
        PropertyInfo? keyVersionProperty = ClientReflectorCache<T>.KeyVersionProperty;
        IEnumerable<PropertyInfo> encryptedProperties = ClientReflectorCache<T>.EncryptedProperties;
        bool wasClosed = false;

        if (ClientReflectorCache<T>.EncryptedProperties.Any() && decrypters is null)
        {
            throw new DecrypterNotProvided<T>();
        }

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
        finally
        {
            if (wasClosed && connection.State == ConnectionState.Open)
                connection.Close();
        }
        CommandSharedMethods.DecryptFields(results, decrypters);
        return results;
    }
}
