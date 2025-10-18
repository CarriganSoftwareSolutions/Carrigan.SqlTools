namespace Carrigan.SqlTools.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]

//TODO: Proof Read
/// <summary>
/// Flag a method or constructor as being external only.
/// The reason for this being necessary is that methods that require a property name
/// are exposed for external users to provide the name as a string,
/// and not require going through the property name class.
/// However, internally, I want to enforce only using the string wrapper classes
/// for internal type safety.
/// THIS MUST BE PUBLIC, to be exposed to the Roslyn analyzer.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// [ExternalOnly]
/// public void SomeExternalOnlyMethod()
/// {
/// }
/// ]]></code>
/// </example>
//Note: THIS MUST BE PUBLIC, to be exposed to the Roslyn analyzer.
public sealed class ExternalOnlyAttribute : Attribute { }