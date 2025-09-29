using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Attributes;
[AttributeUsage(AttributeTargets.Property)]
//TODO: Proof read documentation for entire class
public class AliasAttribute : Attribute
{
    /// <summary>
    /// Public getter to get the <c>Alias</c> for the <c>AS</c> clause for a given property.
    /// </summary>
    internal AliasName Name
    { get; set; }

    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="aliasName">name of alias</param>
    public AliasAttribute(string aliasName) => 
        Name = new(aliasName);
}
