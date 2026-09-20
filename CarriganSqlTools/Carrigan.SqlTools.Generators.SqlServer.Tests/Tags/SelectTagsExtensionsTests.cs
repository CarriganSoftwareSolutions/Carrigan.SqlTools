using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.Tags;

public class SelectTagsExtensionsTests
{
    [Fact]
    public void AsSelectTags_Array_ReturnsSelectTagsContainingOriginalTagsInOrder()
    {
        SelectTag first = new Parameter(1, "First").AsSelectTag("FirstResult");
        SelectTag second = new Parameter(2, "Second").AsSelectTag("SecondResult");
        SelectTag[] selectTags = [first, second];

        SelectTags result = selectTags.AsSelectTags();
        SelectTagBase[] actual = [.. result.All()];

        Assert.Equal(2, actual.Length);
        Assert.Same(first, actual[0]);
        Assert.Same(second, actual[1]);
    }

    [Fact]
    public void AsSelectTags_EmptyArray_ReturnsEmptySelectTags()
    {
        SelectTag[] selectTags = [];

        SelectTags result = selectTags.AsSelectTags();

        Assert.Empty(result.All());
    }
}
