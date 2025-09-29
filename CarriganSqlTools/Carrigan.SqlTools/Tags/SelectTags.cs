using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tags;
//Document and unit test.
public class SelectTags : ISelectTags
{
    private readonly IEnumerable<SelectTag> _selectTags;
    public SelectTags(params IEnumerable<SelectTag> selectTags) =>
        _selectTags = selectTags;

    public bool Any() =>
        _selectTags.Any();
    public string GetSelects() =>
        string.Join(", ", _selectTags);

    public IEnumerable<TableTag> GetTableTags() =>
        _selectTags
            .SelectMany(select => select.GetTableTags());



    //TODO: Documentation, unit testing
    internal SelectTags Append<T>(PropertyName properties, AliasName? aliasName = null) =>
        new
        (
            _selectTags.Append
            (
                new SelectTag
                (
                    SqlToolsReflectorCache<T>
                        .GetColumnsFromProperties(properties)
                        .FirstOrDefault() ?? throw new InvalidPropertyException<T>(properties),
                    aliasName is not null ? new AliasTag(aliasName.Value) : null
                )
            )
        );

    //TODO: Documentation, unit testing
    [ExternalOnly]
    public SelectTags Append<T>(string property, string? aliasName = null) =>
        Append<T>(new PropertyName(property), aliasName is not null ? new AliasName(aliasName) : null);

    //TODO: Documentation, unit testing
    public SelectTags Concat<T>(params IEnumerable<PropertyName> properties) =>
        new
        (
            _selectTags.Concat
            (
                SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)   //TODO: It would be nice to get select directly from a property
                    .Select(column => column._selectTag)    //TODO: It would be nice to skip this step.
            )            
        );

    //TODO: Documentation, unit testing
    [ExternalOnly]
    public SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));



    //TODO: Documentation, unit testing
    internal static SelectTags Get<T>(PropertyName properties, AliasName? aliasName = null) =>
        new
        (
            new SelectTag
            (
                SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)
                    .FirstOrDefault() ?? throw new InvalidPropertyException<T>(properties),
                aliasName is not null ? new AliasTag(aliasName.Value) : null
            )
        );

    //TODO: Documentation, unit testing
    [ExternalOnly]
    public static SelectTags Get<T>(string property, string? aliasName = null) =>
        Get<T>(new PropertyName(property), aliasName is not null ? new AliasName(aliasName) : null);

    //TODO: Documentation, unit testing
    public static SelectTags GetMany<T>(params IEnumerable<PropertyName> properties) =>
        new
        (
            SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)   //TODO: It would be nice to get select directly from a property
                    .Select(column => column._selectTag)   //TODO: It would be nice to skip this step.
        );

    //TODO: Documentation, unit testing
    [ExternalOnly]
    public static SelectTags GetMany<T>(params IEnumerable<string> properties) =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));
}
