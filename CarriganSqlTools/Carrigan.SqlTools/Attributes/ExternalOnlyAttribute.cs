namespace Carrigan.SqlTools.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public sealed class ExternalOnlyAttribute : Attribute { }