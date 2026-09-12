using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Base.Tests.DialectTests;

public class NeutralDialectTests
{
    [Fact]
    public void ColumnExpression_ToString()
    {
        TableTag tableTag = new(new SchemaName("Schema"), new TableName("Table"));
        ColumnTagExpression columnTagExpression = new(new ColumnTag(tableTag, new ColumnName("Column")));

        Assert.Equal("Schema.Table.Column", columnTagExpression.ToString());
    }

    [Fact]
    public void Parameter_ToString() =>
        Assert.Equal("Value", new Parameter(1, "Value").ToString());

    [Fact]
    public void Comparison_ToString() =>
        Assert.Equal("(Left = Right)", new Equal(new Parameter(1, "Left"), new Parameter(2, "Right")).ToString());

    [Fact]
    public void Aggregate_ToString() =>
        Assert.Equal("COUNT(Value)", new Count(new Parameter(1, "Value")).ToString());

    [Fact]
    public void Aggregate_All_ToString() =>
        Assert.Equal("COUNT(*)", new Count().ToString());

    [Fact]
    public void Cast_ToString()
    {
        Cast cast = new(new Parameter(1, "Value"), new FieldProperties { BaseType = "INTEGER" });
        Assert.Equal("CAST(Value AS INTEGER)", cast.ToString());
    }

    [Theory]
    [InlineData(null, "(Left LIKE Right)")]
    [InlineData(true, "(Left CASE SENSITIVE LIKE Right)")]
    [InlineData(false, "(Left CASE INSENSITIVE LIKE Right)")]
    public void Like_ToString(bool? isCaseSensitive, string expected) =>
        Assert.Equal(expected, new Like(new Parameter("A", "Left"), new Parameter("B", "Right"), isCaseSensitive).ToString());

    [Fact]
    public void Xor_ToString()
    {
        IsNull left = new(new Parameter(1, "Left"));
        IsNotNull right = new(new Parameter(2, "Right"));
        Assert.Equal("((Left IS NULL) XOR (Right IS NOT NULL))", new Xor(left, right).ToString());
    }

    [Fact]
    public void EmptyPredicate_ToString() =>
        Assert.Equal(string.Empty, new EmptyPredicate().ToString());

    [Fact]
    public void RenderFinalParameterName_DoesNotFinalize() =>
        Assert.Equal("Value", NeutralDialect.Instance.RenderFinalParameterName("Value", 12));

    [Fact]
    public void QuoteIdentifier_DoesNotQuote() =>
        Assert.Equal("Identifier", NeutralDialect.Instance.QuoteIdentifier("Identifier"));
}
