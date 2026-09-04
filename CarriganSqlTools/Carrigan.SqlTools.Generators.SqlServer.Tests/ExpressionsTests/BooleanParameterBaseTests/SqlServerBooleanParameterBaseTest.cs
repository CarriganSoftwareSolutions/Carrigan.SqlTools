using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.BooleanParameterBaseTests;

public abstract class SqlServerBooleanParameterBaseTest<modelT> : BooleanParameterBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new SqlServerDialect();

    protected override string ExpectSqlFragment(ParameterTag parameterTag) =>
        $"@{parameterTag}_1";

    protected override BooleanParameter NewBooleanParameter(string propertyName, bool value) =>
        new BooleanParameter<modelT>(value, propertyName);

    protected override BooleanParameter NewBooleanParameter(PropertyName propertyName, bool value) =>
        new BooleanParameter<modelT>(value, propertyName);
}
