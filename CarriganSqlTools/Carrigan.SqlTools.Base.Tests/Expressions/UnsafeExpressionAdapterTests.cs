using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CARRIGAN0002 // These tests intentionally exercise APIs marked TypeSafetyLoss.

public class UnsafeExpressionAdapterTests
#pragma warning restore IDE0079 // Remove unnecessary suppression
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
    public void AsNumericExpression_PreservesExpressionBehavior()
    {
        TableTag tableTag = new("dbo", "Numbers");
        TestSqlExpression expression = new("Value", tableTag, true);
        NumericExpression wrapper = expression.AsNumericExpression();

        Assert.IsType<NumericExpressionWrapper>(wrapper);
        Assert.Equal("Value", wrapper.ToString());
        Assert.Equal("Value", wrapper.ToSqlFragments(NeutralDialect.Instance).ToSql(NeutralDialect.Instance));
        _ = Assert.Single(wrapper.LeafTables);
        Assert.Equal(tableTag, wrapper.LeafTables.Single());
        Assert.Contains(expression, wrapper.DescendantNodes);
        Assert.True(wrapper.ContainsAggregate());
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
    public void Adapter_PreservesDescendantParameterTraversal()
    {
        Parameter parameter = new(42, "Value");
        NumericExpression wrapper = parameter.AsNumericExpression();

        Assert.Contains(parameter, wrapper.DescendantNodes);
        Assert.Contains(parameter, wrapper.DescendantParameters);
    }

    [Fact]
    public void Adapter_RemainsTransparentInsideParentEquality()
    {
        SqlExpression expression = new TestSqlExpression("Value", new("dbo", "Numbers"));
        NumericExpression wrapper = expression.AsNumericExpression();
        Add adaptedAdd = new(wrapper);
        IEnumerable<SqlExpression> unsafeExpressions = [expression];
        Add unsafeAdd = new(unsafeExpressions);

        Assert.True(adaptedAdd == unsafeAdd);
        Assert.Equal(adaptedAdd.GetHashCode(), unsafeAdd.GetHashCode());
    }

    [Fact]
    public void NumericExpressionWrapper_EqualityIsTransparent()
    {
        TableTag tableTag = new("dbo", "Numbers");
        SqlExpression expression = new TestSqlExpression("Value", tableTag);
        SqlExpression equivalentExpression = new TestSqlExpression("Value", new("dbo", "Numbers"));
        SqlExpression differentExpression = new TestSqlExpression("OtherValue", tableTag);
        SqlExpression wrapper = expression.AsNumericExpression();
        SqlExpression equivalentWrapper = equivalentExpression.AsNumericExpression();
        SqlExpression differentWrapper = differentExpression.AsNumericExpression();

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
            [expression] = "value"
        };

        Assert.Equal("value", dictionary[wrapper]);
        Assert.Equal("value", dictionary[equivalentWrapper]);

        dictionary[equivalentWrapper] = "updated";
        Assert.Single(dictionary);
        Assert.Equal("updated", dictionary[expression]);

        HashSet<SqlExpression> hashSet = [expression];
        Assert.False(hashSet.Add(wrapper));
        Assert.False(hashSet.Add(equivalentWrapper));
        Assert.True(hashSet.Add(differentWrapper));
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
    public void NestedAdapters_ResolveToOriginalExpressionIdentity()
    {
        SqlExpression expression = new TestSqlExpression("Value", new("dbo", "Numbers"));
        SqlExpression numericWrapper = expression.AsNumericExpression();
        SqlExpression predicateWrapper = numericWrapper.AsPredicate();
        SqlExpression nestedNumericWrapper = predicateWrapper.AsNumericExpression();

        Assert.True(expression == numericWrapper);
        Assert.True(expression == predicateWrapper);
        Assert.True(expression == nestedNumericWrapper);
        Assert.True(nestedNumericWrapper == expression);
        Assert.Equal(expression.GetHashCode(), numericWrapper.GetHashCode());
        Assert.Equal(expression.GetHashCode(), predicateWrapper.GetHashCode());
        Assert.Equal(expression.GetHashCode(), nestedNumericWrapper.GetHashCode());
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
    public void NumericExpressionWrapper_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new NumericExpressionWrapper(null!));

    [Fact]
    public void PredicateWrapper_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new PredicateWrapper(null!));
}

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning restore CARRIGAN0002

#pragma warning restore IDE0079 // Remove unnecessary suppression