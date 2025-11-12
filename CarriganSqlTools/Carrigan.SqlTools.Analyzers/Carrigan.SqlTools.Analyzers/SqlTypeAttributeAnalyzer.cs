using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Carrigan.SqlTools.Analyzers;

#pragma warning disable RS1041 // Compiler extensions should be implemented in assemblies targeting netstandard2.0
[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning restore RS1041 // Compiler extensions should be implemented in assemblies targeting netstandard2.0
public sealed class SqlTypeAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly Dictionary<string, string[]> attributesMetaData =
        new()
        {
            ["SqlBinaryAttribute"] = ["System.Byte[]"],
            ["SqlCharAttribute"] = ["System.Char", "System.String"],
            ["SqlDateTime2Attribute"] = ["System.DateTime", "System.DateOnly"],
            ["SqlDateTimeAttribute"] = ["System.DateTime", "System.DateOnly"],
            ["SqlDateTimeOffsetAttribute"] = ["System.DateTimeOffset"],
            ["SqlDecimalAttribute"] = ["System.Decimal"],
            ["SqlFloatAttribute"] = ["System.Single"],
            ["SqlImageAttribute"] = ["System.Byte[]"],
            ["SqlMoneyAttribute"] = ["System.Decimal"],
            ["SqlTextAttribute"] = ["System.Char", "System.String"],
            ["SqlTimeAttribute"] = ["System.TimeOnly"],
            ["SqlVarBinaryMaxAttribute"] = ["System.Byte[]"],
            ["SqlVarCharMaxAttribute"] = ["System.Char", "System.String"]
        };

    private static readonly HashSet<string> obsoletes =
        new(["SqlTextAttribute", "SqlImageAttribute"]);

    public const string TypeDiagnosticId = "CARRIGANSQL0001";
    private static readonly DiagnosticDescriptor TypeRule = new(
        id: TypeDiagnosticId,
        title: "Invalid Use Of Attribute Derived From SqlTypeAttribute",
        messageFormat: "Member '{0}' is marked with '{1}' and should not be applied to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute.");

    //public const string PrecisionDiagnosticId = "CARRIGANSQL0002";
    //private static readonly DiagnosticDescriptor PrecisionRule = new(
    //    id: PrecisionDiagnosticId,
    //    title: "Invalid Use Of Attribute Derived From SqlTypeAttribute",
    //    messageFormat: "Member '{0}' is marked with '{1}' and should not be applied to type '{2}'",
    //    category: "Usage",
    //    defaultSeverity: DiagnosticSeverity.Warning,
    //    isEnabledByDefault: true,
    //    description: "Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute.");

    public const string ObsoleteDiagnosticId = "CARRIGANSQL0003";
    private static readonly DiagnosticDescriptor ObsoleteRule = new(
        id: ObsoleteDiagnosticId,
        title: "Use Of Attribute Associated With Obsolete SQL Types",
        messageFormat: "Member '{0}' is marked with '{1}' with is associated with an obsolete SQL Types",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks and warns for the use of an Attribute associate with obsolete SQL types.");

    //public const string NotRecommendedDiagnosticId = "CARRIGANSQL0004";
    //private static readonly DiagnosticDescriptor NotRecommendedRule = new(
    //    id: NotRecommendedDiagnosticId,
    //    title: "Invalid Use Of Attribute Derived From SqlTypeAttribute",
    //    messageFormat: "Member '{0}' is marked with '{1}' and should not be applied to type '{2}'",
    //    category: "Usage",
    //    defaultSeverity: DiagnosticSeverity.Warning,
    //    isEnabledByDefault: true,
    //    description: "Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute.");

    public SqlTypeAttributeAnalyzer()
    {
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [TypeRule, ObsoleteRule];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }
    public static ITypeSymbol? GetTypeSymbol(Compilation compilation, Type runtimeType)
    {
        if (runtimeType.IsArray)
        {
            ITypeSymbol? elementTypeSymbol = GetTypeSymbol(compilation, runtimeType.GetElementType()!);
            return elementTypeSymbol is null
                ? null
                : compilation.CreateArrayTypeSymbol(elementTypeSymbol);
        }
        else if (runtimeType.IsGenericType && runtimeType.IsGenericTypeDefinition is false)
        {
            Type genericDefinition = runtimeType.GetGenericTypeDefinition();
            INamedTypeSymbol? genericDefinitionSymbol = compilation.GetTypeByMetadataName(genericDefinition.FullName!);

            if (genericDefinitionSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                ITypeSymbol[] typeArguments =
                [.. runtimeType
                        .GetGenericArguments()
                        .Select(genericArgument => GetTypeSymbol(compilation, genericArgument))
                        .OfType<ITypeSymbol>()
                ];

                return namedTypeSymbol.Construct(typeArguments);
            }

            return null;
        }
        else
        {
            string metadataName = runtimeType.FullName ??
                                  (runtimeType.Namespace is null
                                      ? runtimeType.Name
                                      : $"{runtimeType.Namespace}.{runtimeType.Name}");

            return compilation.GetTypeByMetadataName(metadataName);
        }
    }
    private static ITypeSymbol? ResolveTypeByMetadataName(Compilation compilation, string metadataName)
    {
        if (metadataName.EndsWith("[]", StringComparison.Ordinal))
        {
            string elementName = metadataName.Substring(0, metadataName.Length - 2);
            INamedTypeSymbol? element = compilation.GetTypeByMetadataName(elementName);
            return element is null ? null : compilation.CreateArrayTypeSymbol(element);
        }

        return compilation.GetTypeByMetadataName(metadataName);
    }
    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;
        INamedTypeSymbol? attributeClass;
        Location attributeLocation;
        bool isValid;

        foreach (AttributeData attribute in propertySymbol.GetAttributes())
        {
            attributeClass = attribute.AttributeClass;
            if (attributeClass is not null)
            {
                if (attributesMetaData.TryGetValue(attributeClass.Name, out string[]? allowedTypeMetaData))
                {
                    ITypeSymbol[] allowedSymbols =
                    [
                        .. allowedTypeMetaData
                            .Select(name => ResolveTypeByMetadataName(context.Compilation, name))
                            .OfType<ITypeSymbol>()
                    ];

                    //Check for type mismatches
                    isValid = allowedSymbols.Any(
                        allowed => SymbolEqualityComparer.Default.Equals(propertySymbol.Type, allowed));

                    if (isValid == false)
                    {
                        attributeLocation = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                                ?? propertySymbol.Locations.FirstOrDefault()
                                ?? Location.None;
                        context.ReportDiagnostic(Diagnostic.Create(
                            TypeRule,
                            attributeLocation,
                            propertySymbol.Name,
                            attributeClass.Name,
                            propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                    }
                }

                //check for obsolete SQL Types
                if (obsoletes.Contains(attributeClass.Name))
                {
                    attributeLocation = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                            ?? propertySymbol.Locations.FirstOrDefault()
                            ?? Location.None;
                    context.ReportDiagnostic(Diagnostic.Create(
                        ObsoleteRule,
                        attributeLocation,
                        propertySymbol.Name,
                        attributeClass.Name));
                }
            }
        }
    }
}
