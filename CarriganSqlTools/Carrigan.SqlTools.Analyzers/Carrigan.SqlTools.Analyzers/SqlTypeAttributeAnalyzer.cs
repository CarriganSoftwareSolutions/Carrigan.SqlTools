using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Carrigan.SqlTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SqlTypeAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly IReadOnlyDictionary<string, string[]> AllowedTypeMetadata =
        new Dictionary<string, string[]>
        {
            ["Carrigan.SqlTools.Attributes.SqlBinaryAttribute"] =
                ["System.Byte[]"],
            ["Carrigan.SqlTools.Attributes.SqlVarBinaryMaxAttribute"] =
                ["System.Byte[]"],
            ["Carrigan.SqlTools.Attributes.SqlImageAttribute"] =
                ["System.Byte[]"],

            ["Carrigan.SqlTools.Attributes.SqlCharAttribute"] =
                ["System.Char", "System.String"],
            ["Carrigan.SqlTools.Attributes.SqlVarCharMaxAttribute"] =
                ["System.Char", "System.String"],
            ["Carrigan.SqlTools.Attributes.SqlTextAttribute"] =
                ["System.Char", "System.String"],

            ["Carrigan.SqlTools.Attributes.SqlDateTimeAttribute"] =
                ["System.DateTime", "System.DateOnly", "System.TimeOnly"],
            ["Carrigan.SqlTools.Attributes.SqlDateTime2Attribute"] =
                ["System.DateTime", "System.DateOnly", "System.TimeOnly"],

            ["Carrigan.SqlTools.Attributes.SqlDateAttribute"] =
                ["System.DateTime", "System.DateOnly"],
            ["Carrigan.SqlTools.Attributes.SqlTimeAttribute"] =
                ["System.DateTime", "System.TimeOnly"],

            ["Carrigan.SqlTools.Attributes.SqlDateTimeOffsetAttribute"] =
                ["System.DateTimeOffset"],

            ["Carrigan.SqlTools.Attributes.SqlFloatAttribute"] =
                ["System.Single", "System.Double", "System.Decimal"],
            ["Carrigan.SqlTools.Attributes.SqlMoneyAttribute"] =
                ["System.Single", "System.Double", "System.Decimal"],
            ["Carrigan.SqlTools.Attributes.SqlDecimalAttribute"] =
                ["System.Single", "System.Double", "System.Decimal"]
        };

    private static readonly IReadOnlyDictionary<string, string[]> PrecisionWarningTypeMetadata =
        new Dictionary<string, string[]>
        {
            ["Carrigan.SqlTools.Attributes.SqlFloatAttribute"] =
                ["System.Single", "System.Decimal"],
            ["Carrigan.SqlTools.Attributes.SqlDecimalAttribute"] =
                ["System.Single", "System.Double"]
        };

    private static readonly IReadOnlyDictionary<string, string[]> SemanticWarningTypeMetadata =
        new Dictionary<string, string[]>
        {
            ["Carrigan.SqlTools.Attributes.SqlDateTimeAttribute"] =
                ["System.DateOnly", "System.TimeOnly"],
            ["Carrigan.SqlTools.Attributes.SqlDateTime2Attribute"] =
                ["System.DateOnly", "System.TimeOnly"],
            ["Carrigan.SqlTools.Attributes.SqlDateAttribute"] =
                ["System.DateTime"],
            ["Carrigan.SqlTools.Attributes.SqlTimeAttribute"] =
                ["System.DateTime"]
        };

    private static readonly string[] ObsoleteAttributeMetadataNames =
    [
        "Carrigan.SqlTools.Attributes.SqlTextAttribute",
        "Carrigan.SqlTools.Attributes.SqlImageAttribute"
    ];

    private static readonly string[] LegacyAttributeMetadataNames =
    [
        "Carrigan.SqlTools.Attributes.SqlMoneyAttribute",
        "Carrigan.SqlTools.Attributes.SqlDateTimeAttribute"
    ];

    public const string TypeDiagnosticId = "CARRIGANSQL0001";
    private static readonly DiagnosticDescriptor TypeRule = new(
        id: TypeDiagnosticId,
        title: "Invalid Use Of Attribute Derived From SqlTypeAttribute",
        messageFormat: "Member '{0}' is marked with '{1}' and should not be applied to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute.");

    public const string PrecisionDiagnosticId = "CARRIGANSQL0002";
    private static readonly DiagnosticDescriptor PrecisionRule = new(
        id: PrecisionDiagnosticId,
        title: "Possible Precision Mismatch for SQL Type Attribute",
        messageFormat: "Member '{0}' is marked with '{1}' and may lose precision when applied to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Warns when a type combination may cause precision or scale mismatches between SQL and .NET representations.");

    public const string SemanticDiagnosticId = "CARRIGANSQL0003";
    private static readonly DiagnosticDescriptor SemanticRule = new(
        id: SemanticDiagnosticId,
        title: "Possible Semantic Mismatch for SQL Type Attribute",
        messageFormat: "Member '{0}' is marked with '{1}' and may represent a semantic mismatch to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Warns when a type combination may cause semantic mismatches between SQL and .NET representations.");

    public const string LegacyDiagnosticId = "CARRIGANSQL0004";
    private static readonly DiagnosticDescriptor LegacyRule = new(
        id: LegacyDiagnosticId,
        title: "Use of Attribute Associated with Legacy SQL Types DateTime and Money",
        messageFormat: "Member '{0}' is marked with '{1}', which is associated with a legacy SQL type",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks and warns when an attribute associated with legacy SQL types is used.");

    public const string ObsoleteDiagnosticId = "CARRIGANSQL0005";
    private static readonly DiagnosticDescriptor ObsoleteRule = new(
        id: ObsoleteDiagnosticId,
        title: "Use of Attribute Associated with Obsolete SQL Types",
        messageFormat: "Member '{0}' is marked with '{1}', which is associated with an obsolete SQL type",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks and warns when an attribute associated with obsolete SQL types is used.");

    public SqlTypeAttributeAnalyzer()
    {
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [TypeRule, PrecisionRule, SemanticRule, ObsoleteRule, LegacyRule];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(static compilationStartContext =>
        {
            ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> allowedTypeSymbolMappings =
                BuildTypeMappings(compilationStartContext.Compilation, AllowedTypeMetadata);

            ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> precisionWarningSymbolMappings =
                BuildTypeMappings(compilationStartContext.Compilation, PrecisionWarningTypeMetadata);

            ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> semanticWarningSymbolMappings =
                BuildTypeMappings(compilationStartContext.Compilation, SemanticWarningTypeMetadata);

            ImmutableHashSet<INamedTypeSymbol> legacyWarningSymbolMappings =
                BuildAttributeSet(compilationStartContext.Compilation, LegacyAttributeMetadataNames);

            ImmutableHashSet<INamedTypeSymbol> obsoleteAttributeSymbols =
                BuildAttributeSet(compilationStartContext.Compilation, ObsoleteAttributeMetadataNames);

            compilationStartContext.RegisterSymbolAction
            (
                symbolContext =>
                    AnalyzeProperty
                    (
                        symbolContext,
                        allowedTypeSymbolMappings,
                        precisionWarningSymbolMappings,
                        semanticWarningSymbolMappings,
                        legacyWarningSymbolMappings,
                        obsoleteAttributeSymbols
                    ),
                    SymbolKind.Property
            );
        });
    }

    private static ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> BuildTypeMappings
    (
        Compilation compilation, 
        IReadOnlyDictionary<string, string[]> metadataNameMappings
    )
    {
        ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>>.Builder builder =
            ImmutableDictionary.CreateBuilder<INamedTypeSymbol, ImmutableArray<ITypeSymbol>>(SymbolEqualityComparer.Default);

        foreach (KeyValuePair<string, string[]> mapping in metadataNameMappings)
        {
            INamedTypeSymbol? attributeSymbol = compilation.GetTypeByMetadataName(mapping.Key);
            if (attributeSymbol is not null)
            {
                IEnumerable<ITypeSymbol> resolvedTypes =
                    mapping.Value
                        .Select(typeMetadata => ResolveTypeByMetadataName(compilation, typeMetadata))
                        .OfType<ITypeSymbol>();

                ImmutableArray<ITypeSymbol> resolvedArray = [.. resolvedTypes];

                if (resolvedArray.Length > 0)
                {
                    builder[attributeSymbol] = resolvedArray;
                }
            }
        }

        return builder.ToImmutable();
    }

    private static ImmutableHashSet<INamedTypeSymbol> BuildAttributeSet(Compilation compilation, IEnumerable<string> metadataNames)
    {
        ImmutableHashSet<INamedTypeSymbol>.Builder builder =
            ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (string metadataName in metadataNames)
        {
            INamedTypeSymbol? attributeSymbol = compilation.GetTypeByMetadataName(metadataName);
            if (attributeSymbol is not null)
            {
                builder.Add(attributeSymbol);
            }
        }

        return builder.ToImmutable();
    }

    private static ITypeSymbol? ResolveTypeByMetadataName(Compilation compilation, string metadataName)
    {
        if (metadataName.EndsWith("[]", StringComparison.Ordinal))
        {
            string elementName = metadataName.Substring(0, metadataName.Length - 2);
            INamedTypeSymbol? element = compilation.GetTypeByMetadataName(elementName);
            if (element is null)
            {
                return null;
            }

            return compilation.CreateArrayTypeSymbol(element);
        }

        return compilation.GetTypeByMetadataName(metadataName);
    }

    private static void AnalyzeProperty
    (
        SymbolAnalysisContext context,
        ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> allowedTypeMappings,
        ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> precisionWarningMappings,
        ImmutableDictionary<INamedTypeSymbol, ImmutableArray<ITypeSymbol>> semanticWarningMappings,
        ImmutableHashSet<INamedTypeSymbol> legacyAttributeMappings,
        ImmutableHashSet<INamedTypeSymbol> obsoleteAttributeMappings
    )
    {
        IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;
        ITypeSymbol propertyType = GetUnderlyingPropertyType(propertySymbol.Type);

        foreach (AttributeData attribute in propertySymbol.GetAttributes())
        {
            INamedTypeSymbol? attributeClass = attribute.AttributeClass;
            if (attributeClass is not null)
            {
                Location attributeLocation = GetAttributeLocation(attribute, propertySymbol, context);

                // Enforce allowed type mappings (hard error)
                if (allowedTypeMappings.TryGetValue(attributeClass, out ImmutableArray<ITypeSymbol> allowedSymbols))
                {
                    bool isValid = allowedSymbols.Any(allowed => SymbolEqualityComparer.Default.Equals(propertyType, allowed));

                    if (isValid == false)
                    {
                        context.ReportDiagnostic
                        (
                            Diagnostic.Create
                            (
                                TypeRule,
                                attributeLocation,
                                propertySymbol.Name,
                                attributeClass.Name,
                                propertyType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                            )
                        );
                    }
                }

                // Precision warning mappings
                if (precisionWarningMappings.TryGetValue(attributeClass, out ImmutableArray<ITypeSymbol> warningSymbols))
                {
                    bool isWarning = warningSymbols.Any(allowed => SymbolEqualityComparer.Default.Equals(propertyType, allowed));

                    if (isWarning)
                    {
                        context.ReportDiagnostic
                        (Diagnostic.Create
                            (
                                PrecisionRule,
                                attributeLocation,
                                propertySymbol.Name,
                                attributeClass.Name,
                                propertyType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                            )
                        );
                    }
                }

                // semantic warning mappings
                if (semanticWarningMappings.TryGetValue(attributeClass, out ImmutableArray<ITypeSymbol> semanticSymbols))
                {
                    bool isWarning = semanticSymbols.Any(allowed => SymbolEqualityComparer.Default.Equals(propertyType, allowed));

                    if (isWarning)
                    {
                        context.ReportDiagnostic
                        (Diagnostic.Create
                            (
                                SemanticRule,
                                attributeLocation,
                                propertySymbol.Name,
                                attributeClass.Name,
                                propertyType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                            )
                        );
                    }
                }

                // Obsolete attribute usage
                if (legacyAttributeMappings.Contains(attributeClass))
                {
                    context.ReportDiagnostic
                    (
                        Diagnostic.Create
                        (
                            LegacyRule,
                            attributeLocation,
                            propertySymbol.Name,
                            attributeClass.Name
                        )
                    );
                }

                // Obsolete attribute usage
                if (obsoleteAttributeMappings.Contains(attributeClass))
                {
                    context.ReportDiagnostic
                    (
                        Diagnostic.Create
                        (
                            ObsoleteRule,
                            attributeLocation,
                            propertySymbol.Name,
                            attributeClass.Name
                        )
                    );
                }
            }
        }
    }

    private static ITypeSymbol GetUnderlyingPropertyType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol &&
            namedTypeSymbol.IsGenericType &&
            namedTypeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return namedTypeSymbol.TypeArguments[0];
        }

        return typeSymbol;
    }

    private static Location GetAttributeLocation(
        AttributeData attribute,
        IPropertySymbol propertySymbol,
        SymbolAnalysisContext context)
    {
        SyntaxNode? attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);
        if (attributeSyntax is not null)
        {
            return attributeSyntax.GetLocation();
        }

        if (propertySymbol.Locations.Length > 0)
        {
            return propertySymbol.Locations[0];
        }

        return Location.None;
    }
}
