using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanParameterBaseTests;

public abstract class PostgreSqlBooleanParameterBaseTest<modelT> : BooleanParameterBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new PostgreSqlDialect();

    protected override string ExpectSqlFragment(ParameterTag parameterTag) =>
        "$1";

    protected override BooleanParameter NewBooleanParameter(string propertyName, bool value) =>
        new BooleanParameter<modelT>(value, propertyName);

    protected override BooleanParameter NewBooleanParameter(PropertyName propertyName, bool value) =>
        new BooleanParameter<modelT>(value, propertyName);
}
