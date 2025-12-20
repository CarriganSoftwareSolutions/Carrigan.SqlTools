namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Marks a method or constructor overload as intended for external consumers only.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, many APIs expose paired overloads:
/// <list type="bullet">
/// <item>
/// <description>
/// An external overload that accepts raw <see cref="string"/> values representing SQL identifiers
/// (for example, table, column, or parameter names).
/// </description>
/// </item>
/// <item>
/// <description>
/// A strongly typed overload that accepts identifier wrapper types (for example,
/// <c>PropertyName</c>, <c>ColumnName</c>, <c>TableName</c>, or <c>AliasName</c>).
/// </description>
/// </item>
/// </list>
/// Members marked with <see cref="ExternalOnlyAttribute"/> are the raw-string entry points.
/// Code within the defining assembly should call the strongly typed overloads instead.
/// <para>
/// This intent is enforced by the Carrigan.Core Roslyn analyzer <c>ExternalOnlyAnalyzer</c>
/// (diagnostic <c>CARRIGAN0001</c>), which reports a diagnostic when a member annotated with
/// this attribute is invoked from within the same assembly.
/// </para>
/// <para>
/// This attribute has no runtime behavior.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Attributes;
/// using Carrigan.SqlTools.IdentifierTypes;
///
/// public sealed class Example
/// {
///     [ExternalOnly]
///     public void DoThing(string propertyName) => DoThing(new PropertyName(propertyName));
///
///     public void DoThing(PropertyName propertyName)
///     {
///         // implementation using PropertyName class
///     }
/// }
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
public sealed class ExternalOnlyAttribute : Attribute { }