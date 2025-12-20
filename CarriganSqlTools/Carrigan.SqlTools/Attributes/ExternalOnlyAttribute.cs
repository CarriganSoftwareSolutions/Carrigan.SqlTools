namespace Carrigan.SqlTools.Attributes;


/// <summary>
/// Indicates that a method or constructor is intended for external-facing use only.
/// </summary>
/// <remarks>
/// This attribute is used to distinguish externally consumable overloads (for example,
/// those accepting raw string identifiers) from internal or strongly typed counterparts
/// that rely on the library’s identifier wrapper types (such as <c>PropertyName</c> or
/// <c>AliasName</c>).
/// <para>
/// It is primarily intended to be consumed by Roslyn analyzers, which may enforce rules
/// such as restricting call sites, discouraging internal usage, or validating API
/// surface design.
/// </para>
/// <para>
/// When this attribute is defined in a referenced assembly and applied by external
/// consumers, it must be declared <see langword="public"/> to allow those assemblies to
/// reference and apply it. Roslyn analyzers can detect the attribute regardless of its
/// accessibility within the analyzed compilation.
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
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
public sealed class ExternalOnlyAttribute : Attribute { }