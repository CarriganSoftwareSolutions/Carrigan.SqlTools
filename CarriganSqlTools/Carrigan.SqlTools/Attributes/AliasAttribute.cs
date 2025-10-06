using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Attributes;
[AttributeUsage(AttributeTargets.Property)]
//TODO: Proof read documentation for entire class

/// <summary>
/// Allows for setting a default <c>AS</c> alias on a property.
/// </summary>
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
    public AliasAttribute(string aliasName)
    {
        ArgumentNullException.ThrowIfNull(aliasName, nameof(aliasName));
        if (aliasName == string.Empty)
            throw new ArgumentException("name is an empty string", nameof(aliasName));
        Name = new(aliasName);
    }
}
