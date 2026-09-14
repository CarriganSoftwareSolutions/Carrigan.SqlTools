using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class UnsafeExpressionAdapterTests

{
    private sealed class TestSqlExpression : SqlExpression
    {
        private readonly string _text;
        private readonly TableTag _tableTag;
        private readonly bool _isAggregate;

        public TestSqlExpression(string text, TableTag tableTag, bool isAggregate = false)
            : base([])
        {
            _text = text;
            _tableTag = tableTag;
            _isAggregate = isAggregate;
        }

        public override IEnumerable<TableTag> LeafTables =>
            [_tableTag];

        public override bool IsAggregate() =>
            _isAggregate;

        protected override bool EqualsCore(SqlExpression other) =>
            other is TestSqlExpression testSqlExpression &&
            string.Equals(_text, testSqlExpression._text, StringComparison.Ordinal) &&
            _tableTag.Equals(testSqlExpression._tableTag) &&
            _isAggregate == testSqlExpression._isAggregate;

        protected override void AddToHashCode(ref HashCode hashCode)
        {
            hashCode.Add(_text, StringComparer.Ordinal);
            hashCode.Add(_tableTag);
            hashCode.Add(_isAggregate);
        }

        public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
        {
            yield return new SqlFragmentText(_text);
        }
    }

    [Fact]
    public void AsPredicate_PreservesExpressionBehavior()
    {
        TableTag tableTag = new("dbo", "Flags");
        TestSqlExpression expression = new("IsEnabled", tableTag);
        Predicates wrapper = expression.AsPredicate();

        Assert.IsType<PredicateWrapper>(wrapper);
        Assert.Equal("IsEnabled", wrapper.ToString());
        Assert.Equal("IsEnabled", wrapper.ToSqlFragments(NeutralDialect.Instance).ToSql(NeutralDialect.Instance));
        _ = Assert.Single(wrapper.LeafTables);
        Assert.Equal(tableTag, wrapper.LeafTables.Single());
        Assert.Contains(expression, wrapper.DescendantNodes);
    }

    [Fact]
    public void PredicateWrapper_EqualityIsTransparent()
    {
        TableTag tableTag = new("dbo", "Flags");
        SqlExpression expression = new TestSqlExpression("IsEnabled", tableTag);
        SqlExpression equivalentExpression = new TestSqlExpression("IsEnabled", new("dbo", "Flags"));
        SqlExpression differentExpression = new TestSqlExpression("IsDisabled", tableTag);
        SqlExpression wrapper = expression.AsPredicate();
        SqlExpression equivalentWrapper = equivalentExpression.AsPredicate();
        SqlExpression differentWrapper = differentExpression.AsPredicate();

        Assert.True(wrapper.Equals(expression));
        Assert.True(expression.Equals(wrapper));
        Assert.True(wrapper.Equals(equivalentExpression));
        Assert.True(equivalentExpression.Equals(wrapper));
        Assert.True(wrapper == equivalentWrapper);
        Assert.False(wrapper != equivalentWrapper);
        Assert.False(wrapper == differentWrapper);
        Assert.Equal(expression.GetHashCode(), wrapper.GetHashCode());
        Assert.Equal(equivalentExpression.GetHashCode(), equivalentWrapper.GetHashCode());

        Dictionary<SqlExpression, string> dictionary = new()
        {
            [wrapper] = "value"
        };

        Assert.Equal("value", dictionary[expression]);
        Assert.Equal("value", dictionary[equivalentExpression]);

        dictionary[equivalentExpression] = "updated";
        Assert.Single(dictionary);
        Assert.Equal("updated", dictionary[wrapper]);

        HashSet<SqlExpression> hashSet = [wrapper];
        Assert.False(hashSet.Add(expression));
        Assert.False(hashSet.Add(equivalentWrapper));
        Assert.True(hashSet.Add(differentWrapper));
    }

    [Fact]
    public void UnsafeArithmeticOverload_AcceptsSqlExpressions()
    {
        IEnumerable<SqlExpression> expressions =
        [
            new TestSqlExpression("Left", new("dbo", "Numbers")),
            new TestSqlExpression("Right", new("dbo", "Numbers"))
        ];

        Add add = new(expressions);

        Assert.Equal("(Left + Right)", add.ToString());
    }

    [Fact]
    public void UnsafeLogicalOverload_AcceptsSqlExpressions()
    {
        IEnumerable<SqlExpression> expressions =
        [
            new TestSqlExpression("Left", new("dbo", "Flags")),
            new TestSqlExpression("Right", new("dbo", "Flags"))
        ];

        And and = new(expressions);

        Assert.Equal("(Left AND Right)", and.ToString());
    }

    [Fact]
    public void PredicateWrapper_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new PredicateWrapper(null!));
}

