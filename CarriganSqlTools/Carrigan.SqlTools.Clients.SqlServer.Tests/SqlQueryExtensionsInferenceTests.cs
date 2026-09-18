using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Carrigan.SqlTools.Clients.SqlServer.Tests;

public class SqlQueryExtensionsInferenceTests
{
    [Fact]
    public void GetParameterCollection_InferredString_UsesValueLength()
    {
        SqlParameter actual = GetSingleParameter("Characters", "#~");

        Assert.Equal(SqlDbType.NVarChar, actual.SqlDbType);
        Assert.Equal(2, actual.Size);
    }

    [Fact]
    public void GetParameterCollection_InferredEmptyString_UsesMinimumLengthOne()
    {
        SqlParameter actual = GetSingleParameter("Value", string.Empty);

        Assert.Equal(SqlDbType.NVarChar, actual.SqlDbType);
        Assert.Equal(1, actual.Size);
    }

    [Fact]
    public void GetParameterCollection_InferredStringAboveBoundedLimit_UsesMax()
    {
        SqlParameter actual = GetSingleParameter("Value", new string('x', 4001));

        Assert.Equal(SqlDbType.NVarChar, actual.SqlDbType);
        Assert.Equal(-1, actual.Size);
    }

    [Fact]
    public void GetParameterCollection_InferredByteArray_UsesValueLength()
    {
        SqlParameter actual = GetSingleParameter("Value", new byte[2]);

        Assert.Equal(SqlDbType.VarBinary, actual.SqlDbType);
        Assert.Equal(2, actual.Size);
    }

    [Fact]
    public void GetParameterCollection_InferredEmptyByteArray_UsesMinimumLengthOne()
    {
        SqlParameter actual = GetSingleParameter("Value", Array.Empty<byte>());

        Assert.Equal(SqlDbType.VarBinary, actual.SqlDbType);
        Assert.Equal(1, actual.Size);
    }

    [Fact]
    public void GetParameterCollection_InferredByteArrayAboveBoundedLimit_UsesMax()
    {
        SqlParameter actual = GetSingleParameter("Value", new byte[8001]);

        Assert.Equal(SqlDbType.VarBinary, actual.SqlDbType);
        Assert.Equal(-1, actual.Size);
    }

    private static SqlParameter GetSingleParameter(string parameterName, object value)
    {
        SqlFragmentParameter parameter = new(new ParameterTag(parameterName), null, value);
        SqlQuery query = new(new SqlServerDialect(), CommandType.Text, [parameter]);

        return query.GetParameterCollection().Single();
    }
}
