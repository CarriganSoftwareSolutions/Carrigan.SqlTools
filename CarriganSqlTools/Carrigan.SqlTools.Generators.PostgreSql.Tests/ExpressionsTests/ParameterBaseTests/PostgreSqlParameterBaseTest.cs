using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ParameterBaseTests;

public abstract class PostgreSqlParameterBaseTest<modelT> : ParameterBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new PostgreSqlDialect();

    protected override string ExpectSqlFragment(ParameterTag parameterTag) =>
        "$1";

    protected override Parameter NewParameter(string propertyName, object? value) =>
        new Parameter<modelT>(propertyName, value);

    protected override Parameter NewParameter(PropertyName propertyName, object? value) =>
        new Parameter<modelT>(propertyName, value);
}
