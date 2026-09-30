using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;
//IGNORE SPELLING: abcdef

public abstract class SqlExpressionsBaseTests
{
    public abstract IEnumerable<Func<SqlExpression>> AttemptNullConstructions();
    protected static Parameter First => new(1, "First");
    protected static Parameter Second => new(2, "Second");
    protected static Parameter Third => new(3, "Third");
    protected static readonly Parameter Default = new(Default);
    protected static Parameter Value => new(1, "Value");
    protected static Parameter ParameterValue => new(1);
    protected static Parameter ParameterTrue => new(1, "True");
    protected static Parameter ParameterFalse => new(1, "False");
    protected static Parameter ParameterDifferentValue => new(10);
    protected static Parameter DifferentParameter => new(20, "Different");
    protected static Count Aggregate => new(ParameterValue);
    protected static Add LeftRight => new(Left, Right);
    protected static Parameter Left => new(1, "Left");
    protected static Parameter Right => new(2, "Right");
    protected static Parameter NullParameter => new NullParameter<int>(new Dialects.NeutralDialect());
    protected static Parameter DateValue => new(new DateTime(2026, 9, 23, 6, 30, 15), "Value");
    protected static Parameter Amount => new(2, "Amount");
    protected static Parameter Find => new("p", "Find");
    protected static Parameter AbcValue => new("abcdef", "Value");
    protected static Parameter Start => new(2, "Start");
    protected static Parameter Length => new(3, "Length");
    protected static Parameter Replacement => new("X", "Replacement");
    protected static Parameter PiValue => new(3.141m, "PiValue");
    protected static Parameter Precision => new(2, "Precision");




    [Fact]
    public void Constructor_ThrowsArgumentNullExceptions()
    {
        foreach(Func<SqlExpression> foo in AttemptNullConstructions())
        {
            _ = Assert.Throws<ArgumentNullException>(() => foo());
        }
    }
}
