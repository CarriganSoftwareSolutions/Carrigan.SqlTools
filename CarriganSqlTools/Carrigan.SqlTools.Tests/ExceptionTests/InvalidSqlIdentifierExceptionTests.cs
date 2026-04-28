using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Tests.ExceptionTests;

public class InvalidSqlIdentifierExceptionTests
{
    [Fact]
    public void SchemaName_Invalid_Exception() => Assert.Throws<InvalidSqlIdentifierException>
        (
            () => _ = new SqlGenerator<SchemaNameInvalidModel>(new SqlServerDialect())
        );

    [Identifier("ValidTable", "123InvalidSchema")]
    private sealed class SchemaNameInvalidModel
    {
        [PrimaryKey]
        public int Id { get; set; }
    }
}
