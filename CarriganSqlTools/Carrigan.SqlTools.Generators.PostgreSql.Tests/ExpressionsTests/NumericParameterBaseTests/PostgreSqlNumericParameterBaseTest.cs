using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.NumericParameterBaseTests;

public abstract class PostgreSqlNumericParameterBaseTest<modelT> : NumericParameterBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new PostgreSqlDialect();

    protected override string ExpectSqlFragment(ParameterTag parameterTag) =>
        "$1";

    protected override NumericParameter NewNumericParameter(string propertyName, object value) =>
        value switch
        {
            byte byteValue => new NumericParameter<modelT, byte>(byteValue, propertyName),
            short shortValue => new NumericParameter<modelT, short>(shortValue, propertyName),
            int intValue => new NumericParameter<modelT, int>(intValue, propertyName),
            long longValue => new NumericParameter<modelT, long>(longValue, propertyName),
            float floatValue => new NumericParameter<modelT, float>(floatValue, propertyName),
            double doubleValue => new NumericParameter<modelT, double>(doubleValue, propertyName),
            decimal decimalValue => new NumericParameter<modelT, decimal>(decimalValue, propertyName),
            _ => throw new NotSupportedException($"Type '{value.GetType().FullName}' is not supported by the numeric parameter model tests.")
        };

    protected override NumericParameter NewNumericParameter(PropertyName propertyName, object value) =>
        value switch
        {
            byte byteValue => new NumericParameter<modelT, byte>(byteValue, propertyName),
            short shortValue => new NumericParameter<modelT, short>(shortValue, propertyName),
            int intValue => new NumericParameter<modelT, int>(intValue, propertyName),
            long longValue => new NumericParameter<modelT, long>(longValue, propertyName),
            float floatValue => new NumericParameter<modelT, float>(floatValue, propertyName),
            double doubleValue => new NumericParameter<modelT, double>(doubleValue, propertyName),
            decimal decimalValue => new NumericParameter<modelT, decimal>(decimalValue, propertyName),
            _ => throw new NotSupportedException($"Type '{value.GetType().FullName}' is not supported by the numeric parameter model tests.")
        };
}
