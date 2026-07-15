using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class AggregateAttributeTests
{
    [Fact]
    public void Select_RendersAggregateAttributesWithPostgreSqlIdentifiers()
    {
        SqlGenerator<AggregateSource> generator = new();
        SelectTags selects = SelectTagGenerator.GetAll<AggregateProjection>();

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal(
            "SELECT AVG(\"AggregateSource\".\"Amount\") AS \"AverageAmount\", " +
            "COUNT(\"AggregateSource\".\"Id\") AS \"CountedIds\", " +
            "MAX(\"AggregateSource\".\"Amount\") AS \"MaximumAmount\", " +
            "MIN(\"AggregateSource\".\"Amount\") AS \"MinimumAmount\", " +
            "SUM(\"AggregateSource\".\"Amount\") AS \"ExplicitSum\" " +
            "FROM \"AggregateSource\"",
            query.QueryText);
    }

    [Fact]
    public void Select_CountAttributeWithoutGenericType_RendersCountStar()
    {
        SqlGenerator<AggregateSource> generator = new();
        SelectTags selects = SelectTagGenerator.GetAll<CountStarProjection>();

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS \"TotalCount\" FROM \"AggregateSource\"", query.QueryText);
        Assert.Empty(selects.GetTableTags());
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
}
