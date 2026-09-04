using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class NumericParameterTests
{
    [Fact]
    public void Parameter_NullValue_ImplicitConversion()
    {
        Parameter parameter = new(null, "Value");
        NumericParameter numericParameter = parameter;

        Assert.Null(numericParameter.Value);
        Assert.Equal("Value", numericParameter.ToString());
    }

    [Fact]
    public void Parameter_NonNumericValue_ImplicitConversion_Exception()
    {
        Parameter parameter = new("NotNumeric", "Value");
        Assert.Throws<NonNumericValueException>(() => (NumericParameter)parameter);
    }
}