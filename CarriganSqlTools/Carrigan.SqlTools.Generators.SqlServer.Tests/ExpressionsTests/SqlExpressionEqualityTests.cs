using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class SqlExpressionEqualityTests
{
    private static void AssertEqualityContract<T>(Func<T> firstFactory, Func<T> secondFactory, Func<T> thirdFactory, Func<T> differentFactory)
        where T : SqlExpression
    {
        T first = firstFactory();
        T second = secondFactory();
        T third = thirdFactory();
        T different = differentFactory();

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(first));
        Assert.True(second.Equals(third));
        Assert.True(first.Equals(third));
        Assert.False(first.Equals(different));
        Assert.False(first.Equals((SqlExpression?)null));
        Assert.True(first.Equals((object)second));
        Assert.False(first.Equals(new object()));

        SqlExpression firstExpression = first;
        SqlExpression secondExpression = second;
        SqlExpression differentExpression = different;
        SqlExpression? nullExpression = null;

        Assert.True(firstExpression == secondExpression);
        Assert.False(firstExpression != secondExpression);
        Assert.False(firstExpression == differentExpression);
        Assert.True(firstExpression != differentExpression);
        Assert.False(firstExpression == nullExpression);
        Assert.False(nullExpression == firstExpression);
        Assert.True(firstExpression != nullExpression);
        Assert.True(nullExpression != firstExpression);
        Assert.Null(nullExpression);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Equal(second.GetHashCode(), third.GetHashCode());

        Dictionary<T, string> dictionary = new()
        {
            [first] = "first"
        };

        Assert.True(dictionary.ContainsKey(second));
        Assert.Equal("first", dictionary[second]);
        Assert.False(dictionary.ContainsKey(different));

        dictionary[second] = "second";
        Assert.Single(dictionary);
        Assert.Equal("second", dictionary[first]);

        HashSet<T> hashSet = [first];
        Assert.False(hashSet.Add(second));
        Assert.True(hashSet.Add(different));

        Dictionary<SqlExpression, string> expressionDictionary = new()
        {
            [first] = "first"
        };

        Assert.True(expressionDictionary.ContainsKey(second));
        Assert.Equal("first", expressionDictionary[second]);
    }

    [Fact]
    public void Column_EqualityContract() =>
        AssertEqualityContract(
            () => new Column<Grades>(nameof(Grades.CreditHours)),
            () => new Column<Grades>(nameof(Grades.CreditHours)),
            () => new Column<Grades>(nameof(Grades.CreditHours)),
            () => new Column<Grades>(nameof(Grades.AcademicYear)));

    [Fact]
    public void NumericColumn_EqualityContract() =>
        AssertEqualityContract(
            () => new NumericColumn<Grades>(nameof(Grades.CreditHours)),
            () => new NumericColumn<Grades>(nameof(Grades.CreditHours)),
            () => new NumericColumn<Grades>(nameof(Grades.CreditHours)),
            () => new NumericColumn<Grades>(nameof(Grades.AcademicYear)));

    [Fact]
    public void BooleanColumn_EqualityContract() =>
        AssertEqualityContract(
            () => new BooleanColumn<BooleanEqualityEntity>(nameof(BooleanEqualityEntity.First)),
            () => new BooleanColumn<BooleanEqualityEntity>(nameof(BooleanEqualityEntity.First)),
            () => new BooleanColumn<BooleanEqualityEntity>(nameof(BooleanEqualityEntity.First)),
            () => new BooleanColumn<BooleanEqualityEntity>(nameof(BooleanEqualityEntity.Second)));

    [Fact]
    public void ColumnTagExpression_EqualityContract()
    {
        Column<Grades> creditHours = new(nameof(Grades.CreditHours));
        Column<Grades> academicYear = new(nameof(Grades.AcademicYear));

        AssertEqualityContract(
            () => new ColumnTagExpression(creditHours.ColumnInfo.ColumnTag),
            () => new ColumnTagExpression(creditHours.ColumnInfo.ColumnTag),
            () => new ColumnTagExpression(creditHours.ColumnInfo.ColumnTag),
            () => new ColumnTagExpression(academicYear.ColumnInfo.ColumnTag));
    }

    [Fact]
    public void EquivalentColumnRepresentations_AreEqual()
    {
        Column<Grades> column = new(nameof(Grades.CreditHours));
        NumericColumn<Grades> numericColumn = new(nameof(Grades.CreditHours));
        ColumnTagExpression columnTagExpression = new(column.ColumnInfo.ColumnTag);

        SqlExpression columnExpression = column;
        SqlExpression numericExpression = numericColumn;
        SqlExpression tagExpression = columnTagExpression;

        Assert.True(columnExpression == numericExpression);
        Assert.True(numericExpression == tagExpression);
        Assert.True(columnExpression == tagExpression);
        Assert.Equal(columnExpression.GetHashCode(), numericExpression.GetHashCode());
        Assert.Equal(columnExpression.GetHashCode(), tagExpression.GetHashCode());
    }

    [Fact]
    public void EquivalentBooleanColumnRepresentations_AreEqual()
    {
        Column<BooleanEqualityEntity> column = new(nameof(BooleanEqualityEntity.First));
        BooleanColumn<BooleanEqualityEntity> booleanColumn = new(nameof(BooleanEqualityEntity.First));

        SqlExpression columnExpression = column;
        SqlExpression booleanExpression = booleanColumn;

        Assert.True(columnExpression == booleanExpression);
        Assert.Equal(columnExpression.GetHashCode(), booleanExpression.GetHashCode());
    }

    [Fact]
    public void Parameter_EqualityContract_UsesParameterName() =>
        AssertEqualityContract(
            () => new Parameter(1, "Value"),
            () => new Parameter(999, "Value"),
            () => new Parameter(null, "Value"),
            () => new Parameter(1, "OtherValue"));

    [Fact]
    public void NumericParameter_EqualityContract_UsesParameterName() =>
        AssertEqualityContract(
            () => new NumericParameter<int>(1, "Value"),
            () => new NumericParameter<int>(999, "Value"),
            () => new NumericParameter<int>(0, "Value"),
            () => new NumericParameter<int>(1, "OtherValue"));

    [Fact]
    public void BooleanParameter_EqualityContract_UsesParameterName() =>
        AssertEqualityContract(
            () => new BooleanParameter(true, new ParameterTag("Value")),
            () => new BooleanParameter(false, new ParameterTag("Value")),
            () => new BooleanParameter(null, new ParameterTag("Value")),
            () => new BooleanParameter(true, new ParameterTag("OtherValue")));

    [Fact]
    public void EquivalentParameterRepresentations_AreEqual()
    {
        SqlExpression parameter = new Parameter(1, "Value");
        SqlExpression numericParameter = new NumericParameter<int>(999, "Value");
        SqlExpression booleanParameter = new BooleanParameter(true, new ParameterTag("Value"));

        Assert.True(parameter == numericParameter);
        Assert.True(numericParameter == booleanParameter);
        Assert.True(parameter == booleanParameter);
        Assert.Equal(parameter.GetHashCode(), numericParameter.GetHashCode());
        Assert.Equal(parameter.GetHashCode(), booleanParameter.GetHashCode());
    }

    [Fact]
    public void ParameterIdentity_IsCaseInsensitive()
    {
        SqlExpression first = new Parameter(1, "Value");
        SqlExpression second = new Parameter(2, "value");

        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Add_EqualityContract() =>
        AssertEqualityContract(
            () => new Add(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Add(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Add(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Add(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Different")));

    [Fact]
    public void Subtract_EqualityContract() =>
        AssertEqualityContract(
            () => new Subtract(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Subtract(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Subtract(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Subtract(new NumericParameter<int>(1, "Right"), new NumericParameter<int>(2, "Left")));

    [Fact]
    public void Minus_EqualityContract() =>
        AssertEqualityContract(
            () => new Minus(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Minus(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Minus(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Minus(new NumericParameter<int>(1, "Right"), new NumericParameter<int>(2, "Left")));

    [Fact]
    public void Multiply_EqualityContract() =>
        AssertEqualityContract(
            () => new Multiply(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Multiply(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Multiply(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Multiply(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Different")));

    [Fact]
    public void Divide_EqualityContract() =>
        AssertEqualityContract(
            () => new Divide(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Divide(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Divide(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Divide(new NumericParameter<int>(1, "Right"), new NumericParameter<int>(2, "Left")));

    [Fact]
    public void Modulo_EqualityContract() =>
        AssertEqualityContract(
            () => new Modulo(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Modulo(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Modulo(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Modulo(new NumericParameter<int>(1, "Right"), new NumericParameter<int>(2, "Left")));

    [Fact]
    public void Mod_EqualityContract() =>
        AssertEqualityContract(
            () => new Mod(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")),
            () => new Mod(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right")),
            () => new Mod(new NumericParameter<int>(100, "Left"), new NumericParameter<int>(200, "Right")),
            () => new Mod(new NumericParameter<int>(1, "Right"), new NumericParameter<int>(2, "Left")));

    [Fact]
    public void ArithmeticAliases_AreEqual()
    {
        SqlExpression subtract = new Subtract(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right"));
        SqlExpression minus = new Minus(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right"));
        SqlExpression modulo = new Modulo(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right"));
        SqlExpression mod = new Mod(new NumericParameter<int>(10, "Left"), new NumericParameter<int>(20, "Right"));

        Assert.True(subtract == minus);
        Assert.Equal(subtract.GetHashCode(), minus.GetHashCode());
        Assert.True(modulo == mod);
        Assert.Equal(modulo.GetHashCode(), mod.GetHashCode());
    }

    [Fact]
    public void DifferentArithmeticOperators_AreNotEqual()
    {
        SqlExpression add = new Add(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right"));
        SqlExpression multiply = new Multiply(new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right"));

        Assert.True(add != multiply);
    }

    [Fact]
    public void Negate_EqualityContract() =>
        AssertEqualityContract(
            () => new Negate(new NumericParameter<int>(1, "Value")),
            () => new Negate(new NumericParameter<int>(2, "Value")),
            () => new Negate(new NumericParameter<int>(3, "Value")),
            () => new Negate(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void Cast_EqualityContract() =>
        AssertEqualityContract(
            () => new Cast(new Parameter(1, "Value"), CreateFieldProperties("INT")),
            () => new Cast(new Parameter(2, "Value"), CreateFieldProperties("int")),
            () => new Cast(new Parameter(3, "Value"), CreateFieldProperties("INT")),
            () => new Cast(new Parameter(1, "Value"), CreateFieldProperties("BIGINT")));

    [Fact]
    public void Avg_EqualityContract() =>
        AssertEqualityContract(
            () => new Avg(new NumericParameter<int>(1, "Value")),
            () => new Avg(new NumericParameter<int>(2, "Value")),
            () => new Avg(new NumericParameter<int>(3, "Value")),
            () => new Avg(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void Average_EqualityContract() =>
        AssertEqualityContract(
            () => new Average(new NumericParameter<int>(1, "Value")),
            () => new Average(new NumericParameter<int>(2, "Value")),
            () => new Average(new NumericParameter<int>(3, "Value")),
            () => new Average(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void AverageAlias_EqualsAvg()
    {
        SqlExpression avg = new Avg(new NumericParameter<int>(1, "Value"));
        SqlExpression average = new Average(new NumericParameter<int>(2, "Value"));

        Assert.True(avg == average);
        Assert.Equal(avg.GetHashCode(), average.GetHashCode());
    }

    [Fact]
    public void Count_EqualityContract() =>
        AssertEqualityContract(
            () => new Count(new NumericParameter<int>(1, "Value")),
            () => new Count(new NumericParameter<int>(2, "Value")),
            () => new Count(new NumericParameter<int>(3, "Value")),
            () => new Count(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void CountStar_EqualityContract()
    {
        Count first = new();
        Count second = new();
        Count different = new(new NumericParameter<int>(1, "Value"));
        SqlExpression firstExpression = first;
        SqlExpression secondExpression = second;
        SqlExpression differentExpression = different;

        Assert.True(first.Equals(second));
        Assert.True(firstExpression == secondExpression);
        Assert.True(firstExpression != differentExpression);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());

        Dictionary<Count, string> dictionary = new()
        {
            [first] = "count"
        };

        Assert.True(dictionary.ContainsKey(second));
        Assert.False(dictionary.ContainsKey(different));
    }

    [Fact]
    public void Sum_EqualityContract() =>
        AssertEqualityContract(
            () => new Sum(new NumericParameter<int>(1, "Value")),
            () => new Sum(new NumericParameter<int>(2, "Value")),
            () => new Sum(new NumericParameter<int>(3, "Value")),
            () => new Sum(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void Min_EqualityContract() =>
        AssertEqualityContract(
            () => new Min(new NumericParameter<int>(1, "Value")),
            () => new Min(new NumericParameter<int>(2, "Value")),
            () => new Min(new NumericParameter<int>(3, "Value")),
            () => new Min(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void Max_EqualityContract() =>
        AssertEqualityContract(
            () => new Max(new NumericParameter<int>(1, "Value")),
            () => new Max(new NumericParameter<int>(2, "Value")),
            () => new Max(new NumericParameter<int>(3, "Value")),
            () => new Max(new NumericParameter<int>(1, "OtherValue")));

    [Fact]
    public void DifferentAggregateFunctions_AreNotEqual()
    {
        SqlExpression sum = new Sum(new NumericParameter<int>(1, "Value"));
        SqlExpression max = new Max(new NumericParameter<int>(2, "Value"));

        Assert.True(sum != max);
    }

    [Fact]
    public void Equal_EqualityContract() =>
        AssertComparisonEqualityContract((left, right) => new Equal(left, right));

    [Fact]
    public void NotEqual_EqualityContract() =>
        AssertComparisonEqualityContract((left, right) => new NotEqual(left, right));

    [Fact]
    public void GreaterThan_EqualityContract() =>
        AssertComparisonEqualityContract((left, right) => new GreaterThan(left, right));

    [Fact]
    public void GreaterThanEqual_EqualityContract() =>
        AssertComparisonEqualityContract((left, right) => new GreaterThanEqual(left, right));

    [Fact]
    public void LessThan_EqualityContract() =>
        AssertComparisonEqualityContract((left, right) => new LessThan(left, right));

    [Fact]
    public void LessThanEqual_EqualityContract() =>
        AssertComparisonEqualityContract((left, right) => new LessThanEqual(left, right));

    [Fact]
    public void DifferentComparisonOperators_AreNotEqual()
    {
        SqlExpression left = new Parameter(1, "Left");
        SqlExpression right = new Parameter(2, "Right");
        SqlExpression equal = new Equal(left, right);
        SqlExpression notEqual = new NotEqual(new Parameter(3, "Left"), new Parameter(4, "Right"));

        Assert.True(equal != notEqual);
    }

    [Fact]
    public void And_EqualityContract() =>
        AssertLogicalEqualityContract(predicates => new And(predicates));

    [Fact]
    public void Or_EqualityContract() =>
        AssertLogicalEqualityContract(predicates => new Or(predicates));

    [Fact]
    public void DifferentLogicalOperators_AreNotEqual()
    {
        Predicates left = CreateEqual("Left", "Right");
        Predicates right = CreateEqual("OtherLeft", "OtherRight");
        SqlExpression and = new And(left, right);
        SqlExpression or = new Or(CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight"));

        Assert.True(and != or);
    }

    [Fact]
    public void Xor_EqualityContract() =>
        AssertEqualityContract(
            () => new Xor(CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight")),
            () => new Xor(CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight")),
            () => new Xor(CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight")),
            () => new Xor(CreateEqual("Left", "Different"), CreateEqual("OtherLeft", "OtherRight")));

    [Fact]
    public void Like_EqualityContract() =>
        AssertEqualityContract(
            () => new Like(new Parameter("A", "Left"), new Parameter("B", "Right"), true),
            () => new Like(new Parameter("C", "Left"), new Parameter("D", "Right"), true),
            () => new Like(new Parameter("E", "Left"), new Parameter("F", "Right"), true),
            () => new Like(new Parameter("A", "Left"), new Parameter("B", "Right"), false));

    [Fact]
    public void IsNull_EqualityContract() =>
        AssertEqualityContract(
            () => new IsNull(new Parameter(null, "Value")),
            () => new IsNull(new Parameter(1, "Value")),
            () => new IsNull(new Parameter(2, "Value")),
            () => new IsNull(new Parameter(null, "OtherValue")));

    [Fact]
    public void IsNotNull_EqualityContract() =>
        AssertEqualityContract(
            () => new IsNotNull(new Parameter(null, "Value")),
            () => new IsNotNull(new Parameter(1, "Value")),
            () => new IsNotNull(new Parameter(2, "Value")),
            () => new IsNotNull(new Parameter(null, "OtherValue")));

    [Fact]
    public void Not_EqualityContract() =>
        AssertEqualityContract(
            () => new Not(CreateEqual("Left", "Right")),
            () => new Not(CreateEqual("Left", "Right")),
            () => new Not(CreateEqual("Left", "Right")),
            () => new Not(CreateEqual("Left", "Different")));

    [Fact]
    public void ColumnValue_EqualityContract() =>
        AssertEqualityContract(
            () => new ColumnValue<Grades>(nameof(Grades.CreditHours), 1),
            () => new ColumnValue<Grades>(nameof(Grades.CreditHours), 999),
            () => new ColumnValue<Grades>(nameof(Grades.CreditHours), 0),
            () => new ColumnValue<Grades>(nameof(Grades.AcademicYear), 1));

    [Fact]
    public void EmptyPredicate_EqualityContract()
    {
        EmptyPredicate first = new();
        EmptyPredicate second = new();
        SqlExpression firstExpression = first;
        SqlExpression secondExpression = second;
        SqlExpression differentExpression = new Not(CreateEqual("Left", "Right"));

        Assert.True(first.Equals(second));
        Assert.True(firstExpression == secondExpression);
        Assert.False(firstExpression != secondExpression);
        Assert.False(firstExpression == differentExpression);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());

        Dictionary<EmptyPredicate, string> dictionary = new()
        {
            [first] = "first"
        };

        Assert.True(dictionary.ContainsKey(second));
        Assert.Equal("first", dictionary[second]);

        HashSet<EmptyPredicate> hashSet = [first];
        Assert.False(hashSet.Add(second));
    }

    [Fact]
    public void Contains_EqualityContract() =>
        AssertEqualityContract(
            () => new Contains<Grades>(new Column<Grades>(nameof(Grades.CourseCode)), new Parameter("term", "Search")),
            () => new Contains<Grades>(new Column<Grades>(nameof(Grades.CourseCode)), new Parameter("other", "Search")),
            () => new Contains<Grades>(new Column<Grades>(nameof(Grades.CourseCode)), new Parameter("third", "Search")),
            () => new Contains<Grades>(new Column<Grades>(nameof(Grades.CourseCode)), new Parameter("term", "DifferentSearch")));

    [Fact]
    public void Exists_EqualityContract_UsesSubqueryInstanceIdentity()
    {
        SqlGenerator<Order> generator = new();
        Subquery<Order> subquery = CreateSubquery(generator);
        Subquery<Order> differentSubquery = CreateSubquery(generator);

        AssertEqualityContract(
            () => new Exists(subquery),
            () => new Exists(subquery),
            () => new Exists(subquery),
            () => new Exists(differentSubquery));
    }

    [Fact]
    public void NotExists_EqualityContract_UsesSubqueryInstanceIdentity()
    {
        SqlGenerator<Order> generator = new();
        Subquery<Order> subquery = CreateSubquery(generator);
        Subquery<Order> differentSubquery = CreateSubquery(generator);

        AssertEqualityContract(
            () => new NotExists(subquery),
            () => new NotExists(subquery),
            () => new NotExists(subquery),
            () => new NotExists(differentSubquery));
    }

    private static void AssertComparisonEqualityContract<T>(Func<SqlExpression, SqlExpression, T> factory) where T : ComparisonOperator =>
        AssertEqualityContract(
            () => factory(new Parameter(1, "Left"), new Parameter(2, "Right")),
            () => factory(new Parameter(10, "Left"), new Parameter(20, "Right")),
            () => factory(new Parameter(100, "Left"), new Parameter(200, "Right")),
            () => factory(new Parameter(1, "Right"), new Parameter(2, "Left")));

    private static void AssertLogicalEqualityContract<T>(Func<IEnumerable<Predicates>, T> factory) where T : LogicalOperator =>
        AssertEqualityContract(
            () => factory([CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight")]),
            () => factory([CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight")]),
            () => factory([CreateEqual("Left", "Right"), CreateEqual("OtherLeft", "OtherRight")]),
            () => factory([CreateEqual("Left", "Different"), CreateEqual("OtherLeft", "OtherRight")]));

    private static Equal CreateEqual(string leftParameterName, string rightParameterName) =>
        new(new Parameter(1, leftParameterName), new Parameter(2, rightParameterName));

    private static FieldProperties CreateFieldProperties(string baseType) =>
        new()
        {
            BaseType = baseType,
            Length = 10,
            IsNullable = true
        };

    private static Subquery<Order> CreateSubquery(SqlGenerator<Order> generator)
    {
        Predicates predicate = new GreaterThan(new Column<Order>(nameof(Order.Total)), new Parameter(100.00m, "Total"));
        return generator.InternalSubquery(null, null, null, predicate, null, null, null, null);
    }

    private sealed class BooleanEqualityEntity
    {
        public bool First { get; set; }
        public bool Second { get; set; }
    }
}
