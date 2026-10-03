using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.PredicatesLogicTests;

//IGNORE SPELLING: ilsabasbdyas

public class ColumnValueTests : PredicateLogicBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
        [];

    private static readonly PostgreSqlDialect Dialect = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1"),
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col2), "1"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1"),
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "2"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        [];

    [Fact]
    public void ByColumnValue_ConstructorSimple_InValid_BadCol() =>
        Assert.Throws<InvalidPropertyException<ColumnTable>>((Func<object?>)(() => new PredicatesLogic.ColumnValue<ColumnTable>("ilsabasbdyas", (object)"1")));

    [Fact]
    public void ByColumnValue_ConstructorSimple_Valid() =>
        _ = new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1");

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterCount()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColumnValues.AllParticipatingParameters.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterValidate()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");

        string expectedValue;
        object actualValue;
        string expectedString;
        string actualString;

        IParameter parameter;

        parameter = byColumnValues.AllParticipatingParameters.Where(param => param.Name.ToString() == "Col1").First();
        expectedValue = "1";
        expectedString = "Col1";
        actualValue = parameter.Value ?? string.Empty;
        actualString = parameter.Name.ToString();
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ColumnCount()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColumnValues.AllParticipatingColumns.OfType<IColumnBase>().Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_Validate()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString;
        string actualString;

        IColumnBase column;

        column = byColumnValues.AllParticipatingColumns.OfType<IColumnBase>().Where(col => col.ColumnInfo.ToString() == "ColumnTable.Col1").First();
        expectedString = "\"ColumnTable\".\"Col1\"";
        actualString = column.ColumnInfo.ColumnTag.ToSql(Dialect);
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ToSql()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString = "(\"ColumnTable\".\"Col1\" = $1)";
        string actualString = byColumnValues.ToSqlFragments(Dialect).ToSql(Dialect);
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_PropertyNameNull_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>((Func<object?>)(() => new PredicatesLogic.ColumnValue<ColumnTable>((PropertyName)null!, (object)"1")));

}