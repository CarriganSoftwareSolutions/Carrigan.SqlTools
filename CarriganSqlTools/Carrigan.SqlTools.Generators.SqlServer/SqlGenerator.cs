using Carrigan.Core.Interfaces;
using Carrigan.SqlTools;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
namespace Carrigan.SqlTools.Generators.SqlServer;

public class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    protected override ISqlDialects Dialect { get; init; } = new SqlServerDialect();

    public SqlGenerator()
    {
    }

    public SqlGenerator(IEncryption encryption) : base(encryption)
    {
    }
}
