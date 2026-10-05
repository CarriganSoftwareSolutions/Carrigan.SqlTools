using Carrigan.Core.Extensions;
using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

namespace Carrigan.SqlTools.Base.Tests.Expressions;
//IGNORE SPELLING: abcdef

public abstract class SqlExpressionsBaseTests
{
    public abstract IEnumerable<Func<SqlExpression>>? AttemptNullConstructions();
    //Note: These should attempt to create an expression that contains a column and an aggregate.
    public abstract IEnumerable<Func<SqlExpression>>? AttemptMixedAggregateConstructions();

    public abstract IEnumerable<SqlExpression>? ExpressionsThatAreNotEqual { get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatAreEqual { get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatHaveAggregates { get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatHaveNoAggregates { get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatHaveColumns { get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatHaveNoColumns{ get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatHaveParameters { get; }
    public abstract IEnumerable<SqlExpression>? ExpressionsThatHaveNoParameters { get; }


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
    protected static Count AggregateWithParameterNoColumn => new(ParameterValue);
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
    protected static SqlExpression ColumnA => NewColumnExpression("ColA");
    protected static SqlExpression ColumnB => NewColumnExpression("ColB");
    protected static SqlExpression Column1 => NewColumnExpression("Col1");
    protected static SqlExpression Column2 => NewColumnExpression("Col2");
    protected static Count AggregateColumn => new(ColumnA);

    private static Column NewColumnExpression(string columnName)
    {
        PropertyInfo propertyInfo = typeof(ColumnTable).GetProperty(columnName)!;
        ColumnInfo columnInfo = new(null, new TableName(nameof(ColumnTable)), propertyInfo, []);
        return new Column(columnInfo);
    }


    [Fact]
    public void Constructor_ThrowsArgumentNullExceptions()
    {
        IEnumerable<Func<SqlExpression>>? functions = AttemptNullConstructions();
        if (functions.IsNotNullOrEmpty())
        {
            foreach (Func<SqlExpression> foo in functions)
            {
                _ = Assert.Throws<ArgumentNullException>(() => foo());
            }
        }
    }

    [Fact]
    public void Constructor_AggregateInconsistency_DoesNotThrowException()
    {
        IEnumerable<Func<SqlExpression>>? functions = AttemptMixedAggregateConstructions();
        if (functions.IsNotNullOrEmpty())
        {
            foreach (Func<SqlExpression> foo in functions)
            {
                foo();
            }
        }
    }

    [Fact]
    public void Expressions_Equal()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatAreEqual;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                foreach (SqlExpression other in expressions)
                {
                    Assert.Equal(expression, other);
                }
            }
        }
    }

    [Fact]
    public void Expressions_Equal_HaveMatchingHashCodes()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatAreEqual;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                foreach (SqlExpression other in expressions)
                    Assert.Equal(expression.GetHashCode(), other.GetHashCode());
            }
        }
    }

    [Fact]
    public void Expressions_NotEqual()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatAreNotEqual;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.Single(expressions, other => other == expression);
            }
        }
    }

    [Fact]
    public void Expressions_HasAggregates()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatHaveAggregates;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.True(expression.HasAggregates());
            }
        }
    }


    [Fact]
    public void Expressions_HasNoAggregates()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatHaveNoAggregates;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.False(expression.HasAggregates());
            }
        }
    }

    [Fact]
    public void Expressions_HasColumns()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatHaveColumns;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.True(expression.HasColumns());
            }
        }
    }


    [Fact]
    public void Expressions_HasNoColumns()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatHaveNoColumns;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.False(expression.HasColumns());
            }
        }
    }

    [Fact]
    public void Expressions_HasParameters()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatHaveParameters;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.True(expression.HasParameters());
            }
        }
    }


    [Fact]
    public void Expressions_HasNoParameters()
    {
        IEnumerable<SqlExpression>? expressions = ExpressionsThatHaveNoParameters;
        if (expressions.IsNotNullOrEmpty())
        {
            foreach (SqlExpression expression in expressions)
            {
                Assert.False(expression.HasParameters());
            }
        }
    }
}
