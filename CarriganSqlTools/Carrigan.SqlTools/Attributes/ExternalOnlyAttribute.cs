namespace Carrigan.SqlTools.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]

/// <summary>
/// Indicates that a method or constructor is intended for **external use only**.
/// </summary>
/// <remarks>
/// This attribute differentiates externally accessible overloads (e.g., those accepting
/// raw string identifiers) from their internal, strongly typed counterparts that use
/// the library’s wrapper classes (such as <c>PropertyName</c> or <c>AliasName</c>).
/// <para>
/// It must be declared <see langword="public"/> so that Roslyn analyzers in other assemblies
/// can detect and enforce its intended usage.
/// </para>
/// <para>
/// This attribute should be defined at the assembly level containing both the analyzer
/// and the target code to ensure accessibility.
/// </para>
/// </remarks>
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