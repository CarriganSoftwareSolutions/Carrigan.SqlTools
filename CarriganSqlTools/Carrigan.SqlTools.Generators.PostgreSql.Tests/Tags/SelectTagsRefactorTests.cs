using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Tags;

public class SelectTagsRefactorTests
{
    private static readonly PostgreSqlDialect Dialect = new();

    [Fact]
    public void GenericSelectTag_UsesReflectedSelectProjectionMetadata()
    {
        SelectTag selectTag = new SelectTag<SelectProjection>(nameof(SelectProjection.LeftId));

        Assert.Equal("\"SelectLeft\".\"Id\" AS \"LeftId\"", selectTag.ToSql(Dialect));
        Assert.Equal("SelectLeft", Assert.Single(selectTag.TableTags).ToString());
    }

    [Fact]
    public void GenericSelectTag_InvalidAlias_ThrowsInvalidSqlIdentifierException() => Assert.Throws<InvalidSqlIdentifierException>
        (
            () => new SelectTag<SelectLeft>(nameof(SelectLeft.Id), "123Invalid")
        );

    [Fact]
    public void Concat_UsesReflectedSelectProjectionMetadata()
    {
        SelectTags result = new SelectTags().Concat<SelectProjection>
        (
            new PropertyName(nameof(SelectProjection.LeftId)),
            new PropertyName(nameof(SelectProjection.RightName))
        );

        Assert.Equal("\"SelectLeft\".\"Id\" AS \"LeftId\", \"SelectRight\".\"Name\" AS \"RightName\"", result.ToSql(Dialect));
        Assert.Equal(["SelectLeft", "SelectRight"], result.GetTableTags().Select(static table => table.ToString()));
    }

    [Fact]
    public void GenericSelectTags_UsesReflectedSelectProjectionMetadata()
    {
        SelectTags result = new SelectTags<SelectProjection>
        (
            [
                new(nameof(SelectProjection.LeftId)),
                new(nameof(SelectProjection.RightName))
            ]
        );

        Assert.Equal("\"SelectLeft\".\"Id\" AS \"LeftId\", \"SelectRight\".\"Name\" AS \"RightName\"", result.ToSql(Dialect));
        Assert.Equal(["SelectLeft", "SelectRight"], result.GetTableTags().Select(static table => table.ToString()));
    }

    [Fact]
    public void SelectTagConcat_UsesReflectedSelectProjectionMetadata()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));

        SelectTags result = first.Concat<SelectProjection>(nameof(SelectProjection.RightName));

        Assert.Equal("\"SelectLeft\".\"Id\", \"SelectRight\".\"Name\" AS \"RightName\"", result.ToSql(Dialect));
        Assert.Equal(["SelectLeft", "SelectRight"], result.GetTableTags().Select(static table => table.ToString()));
    }

    [Fact]
    public void SelectTagAppend_SelectTag_PreservesOrder()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));
        SelectTag second = SelectTagGenerator.Get<SelectRight>(nameof(SelectRight.Name));

        SelectTags result = first.Append(second);

        Assert.Equal("\"SelectLeft\".\"Id\", \"SelectRight\".\"Name\"", result.ToSql(Dialect));
    }

    [Fact]
    public void SelectTagConcat_SelectTags_PreservesOrder()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));
        SelectTag second = SelectTagGenerator.Get<SelectRight>(nameof(SelectRight.Name));
        SelectTag third = SelectTagGenerator.Get<SelectRight>(nameof(SelectRight.Name), "SecondName");

        SelectTags result = first.Concat(new SelectTags(second, third));

        Assert.Equal("\"SelectLeft\".\"Id\", \"SelectRight\".\"Name\", \"SelectRight\".\"Name\" AS \"SecondName\"", result.ToSql(Dialect));
    }

    [Fact]
    public void SelectTagImplicitConversionToSelectTags_PreservesInstance()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));

        SelectTags result = first;

        Assert.Same(first, Assert.Single(result.All()));
    }

    [Fact]
    public void SelectTagAppend_UsesGeneratorAndPreservesExistingTag()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));

        SelectTags result = first.Append<SelectRight>(nameof(SelectRight.Name), "NameOverride");

        Assert.Equal("\"SelectLeft\".\"Id\", \"SelectRight\".\"Name\" AS \"NameOverride\"", result.ToSql(Dialect));
    }

    [Fact]
    public void SelectTagAppend_InvalidAliasString_ThrowsInvalidSqlIdentifierException()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));

        Assert.Throws<InvalidSqlIdentifierException>
        (
            () => first.Append<SelectRight>(nameof(SelectRight.Name), "123Invalid")
        );
    }

    [Fact]
    public void SelectTagAppend_InvalidAliasName_ThrowsInvalidSqlIdentifierException()
    {
        SelectTag first = SelectTagGenerator.Get<SelectLeft>(nameof(SelectLeft.Id));

        Assert.Throws<InvalidSqlIdentifierException>
        (
            () => first.Append<SelectRight>
            (
                new PropertyName(nameof(SelectRight.Name)),
                new AliasName("123Invalid")
            )
        );
    }

    private sealed class SelectLeft
    {
        public int Id { get; set; }
    }

    private sealed class SelectRight
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class SelectProjection
    {
        [SelectTag<SelectLeft>(nameof(SelectLeft.Id), nameof(LeftId))]
        public int LeftId { get; set; }

        [SelectTag<SelectRight>(nameof(SelectRight.Name), nameof(RightName))]
        public string RightName { get; set; } = string.Empty;
    }
}
