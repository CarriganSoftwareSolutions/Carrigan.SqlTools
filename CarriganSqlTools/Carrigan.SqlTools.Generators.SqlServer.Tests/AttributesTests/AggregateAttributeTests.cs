using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class AggregateAttributeTests
{
    [Fact]
    public void GetAll_RendersAggregateAttributes()
    {
        SelectTags selects = SelectTagGenerator.GetAll<AggregateProjection>();

        IEnumerable<string> actual = selects.Select(static select => select.ToString());

        Assert.Equal(
        [
            "AVG(AggregateSource.Amount) AS AverageAmount",
            "COUNT(AggregateSource.Id) AS CountedIds",
            "MAX(AggregateSource.Amount) AS MaximumAmount",
            "MIN(AggregateSource.Amount) AS MinimumAmount",
            "SUM(AggregateSource.Amount) AS ExplicitSum"
        ],
        actual);
    }

    [Fact]
    public void Select_RendersAggregateAttributesWithSqlServerIdentifiers()
    {
        SqlGenerator<AggregateSource> generator = new();
        SelectTags selects = SelectTagGenerator.GetAll<AggregateProjection>();

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal(
            "SELECT AVG([AggregateSource].[Amount]) AS [AverageAmount], " +
            "COUNT([AggregateSource].[Id]) AS [CountedIds], " +
            "MAX([AggregateSource].[Amount]) AS [MaximumAmount], " +
            "MIN([AggregateSource].[Amount]) AS [MinimumAmount], " +
            "SUM([AggregateSource].[Amount]) AS [ExplicitSum] " +
            "FROM [AggregateSource]",
            query.QueryText);
    }

    [Fact]
    public void Select_CountAttributeWithoutGenericType_RendersCountStar()
    {
        SqlGenerator<AggregateSource> generator = new();
        SelectTags selects = SelectTagGenerator.GetAll<CountStarProjection>();

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS [TotalCount] FROM [AggregateSource]", query.QueryText);
        Assert.Empty(selects.GetTableTags());
    }

    [Fact]
    public void AggregateAttribute_DefaultAlias_UsesDecoratedPropertyMapping()
    {
        SelectTagBase select = Assert.Single(SelectTagGenerator.GetAll<MappedProjection>());

        Assert.Equal("SUM([AggregateSource].[Amount]) AS [MappedTotal]", select.ToSql(new SqlServerDialect()));
    }

    private sealed class AggregateSource
    {
        public int Id { get; set; }

        [Alias("SourceAmountAlias")]
        public decimal Amount { get; set; }
    }

    private sealed class AggregateProjection
    {
        [Average<AggregateSource>(nameof(AggregateSource.Amount))]
        public decimal AverageAmount { get; set; }

        [Count<AggregateSource>(nameof(AggregateSource.Id))]
        public long CountedIds { get; set; }

        [Max<AggregateSource>(nameof(AggregateSource.Amount))]
        public decimal MaximumAmount { get; set; }

        [Min<AggregateSource>(nameof(AggregateSource.Amount))]
        public decimal MinimumAmount { get; set; }

        [Sum<AggregateSource>(nameof(AggregateSource.Amount), "ExplicitSum")]
        public decimal TotalAmount { get; set; }
    }

    private sealed class CountStarProjection
    {
        [Count]
        public long TotalCount { get; set; }
    }

    private sealed class MappedProjection
    {
        [Identifier("MappedTotal")]
        [Sum<AggregateSource>(nameof(AggregateSource.Amount))]
        public decimal Total { get; set; }
    }
}
