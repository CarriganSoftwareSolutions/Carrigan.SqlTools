using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ParameterBaseTests;

public abstract class SqlServerParameterBaseTest<modelT> : ParameterBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new SqlServerDialect();

    protected override string ExpectSqlFragment(ParameterTag parameterTag) =>
        $"@{parameterTag}_1";

    protected override Parameter NewParameter(string propertyName, object? value) =>
        new Parameter<modelT>(propertyName, value);

    protected override Parameter NewParameter(PropertyName propertyName, object? value) =>
        new Parameter<modelT>(propertyName, value);
}
