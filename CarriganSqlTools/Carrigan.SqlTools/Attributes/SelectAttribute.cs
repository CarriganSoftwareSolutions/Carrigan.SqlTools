using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple =false)]
internal class SelectAttribute<T> : Attribute
{
    internal SelectTag SelectTag { get; init; }

    [ExternalOnly]
    public SelectAttribute(string propertyName, string? aliasName = null) : 
        this (new (propertyName), aliasName is null ? null : new AliasName(aliasName))
    {

    }
    internal SelectAttribute(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTag = new
        (
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(propertyName)
                .SingleOrDefault()?.ColumnTag ?? throw new InvalidPropertyException<T>(propertyName),
            AliasTag.New(aliasName)
        );

}
