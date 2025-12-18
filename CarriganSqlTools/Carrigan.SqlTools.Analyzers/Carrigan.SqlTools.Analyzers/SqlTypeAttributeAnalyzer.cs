using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Carrigan.SqlTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]

/// <summary>
/// Analyzes the usage of <c>SqlTypeAttribute</c>-derived attributes and enforces type compatibility.
///
/// <para><strong>Philosophy:</strong></para>
/// <list type="bullet">
///   <item>
///     <description>
///     Hard errors <see cref="SqlTypeAttributeAnalyzer.TypeRule"/> are raised only when a SQL type attribute is
///     applied to a property type that is fundamentally incompatible with it. These rules
///     mirror those enforced by <c>SqlTypeMismatchException</c>.
///     </description>
///   </item>
///   <item>
///     <description>
///     Precision warnings <see cref="SqlTypeAttributeAnalyzer.PrecisionRule"/> occur when a numeric SQL type may
///     lose precision or scale when mapped to a property type, or vice versa.
///     This includes binary floating-point vs. exact decimal mismatches.
///     </description>
///   </item>
///   <item>
///     <description>
///     Semantic warnings <see cref="SqlTypeAttributeAnalyzer.SemanticRule"/> highlight cases where semantics of
///     the property (date-only, time-only, full timestamp) are inconsistent with the semantics
///     of the SQL type, even though the combination is technically allowed.
///     </description>
///   </item>
///   <item>
///     <description>
///     Legacy warnings <see cref="SqlTypeAttributeAnalyzer.LegacyRule"/> are emitted for SQL types that are
///     technically supported but discouraged due to limited precision or range.
///     This includes <c>DATETIME</c> and <c>MONEY</c> where <c>DATETIME2</c> or <c>DECIMAL</c> are preferred.
///     </description>
///   </item>
///   <item>
///     <description>
///     Obsolete warnings <see cref="SqlTypeAttributeAnalyzer.ObsoleteRule"/> are issued for SQL types that are
///     obsolete and should no longer appear in modern databases, such as <c>TEXT</c> and <c>IMAGE</c>.
///     </description>
///   </item>
/// </list>
/// </summary>
/// <remarks>
/// This analyzer ensures that SQL mappings remain correct, predictable, and aligned with
/// best practices across both modern and legacy SQL Server data types.
/// The rules enforced here intentionally mirror, and expand upon,
/// <c>SqlTypeMismatchException</c> so that issues can be surfaced at compile time rather
/// than failing at runtime.
/// </remarks>
public sealed class SqlTypeAttributeAnalyzer : DiagnosticAnalyzer
{
    #region Type Mappings
    /// <summary>
    /// Defines all type combinations that are considered <strong>valid</strong>. 
    /// Any combination not appearing in this table produces
    /// a <see cref="TypeRule"/> error.
    ///
    /// <para>
    /// Mappings in this table correspond directly to the logic in
    /// <c>SqlTypeMismatchException.AllowedAttributes</c>. The analyzer uses metadata names
    /// instead of <see cref="Type"/> objects in order to support analysis of <c>DateOnly</c> and <c>TimeOnly</c>,
    /// even though analyzers doesn't support those types.
    /// </para>
    /// </summary>
    /// 
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

    /// <summary>
    /// Defines precision-risk combinations, e.g. binary floating-point CLR types mapped to
    /// exact decimal SQL types, or exact decimal CLR types mapped to floating-point SQL types.
    ///
    /// <para>
    /// These do <strong>not</strong> represent invalid combinations—only ones where
    /// precision loss is plausible. These are allowed, but use at your own risk.
    /// </para>
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string[]> PrecisionWarningTypeMetadata =
        new Dictionary<string, string[]>
        {
            ["Carrigan.SqlTools.Attributes.SqlFloatAttribute"] =
                ["System.Single", "System.Decimal"],
            ["Carrigan.SqlTools.Attributes.SqlDecimalAttribute"] =
                ["System.Single", "System.Double"]
        };

    /// <summary>
    /// Defines mappings that are technically valid but semantically suspicious.
    /// Examples include:
    /// <list type="bullet">
    ///   <item><description>
    ///   Applying a full date/time SQL type to a date-only type.
    ///   </description></item>
    ///   <item><description>
    ///   Applying a full date/time SQL type to a date-only or time-only type.
    ///   </description></item>
    /// </list>
    /// </summary>
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
                ["System.DateTime"],

            ["Carrigan.SqlTools.Attributes.SqlVarCharMaxAttribute"] = ["System.Char"],
            ["Carrigan.SqlTools.Attributes.SqlTextAttribute"] = ["System.Char"]
        };

    /// <summary>
    /// Attributes associated with SQL types that are marked obsolete by Microsoft.
    /// SQL Server no longer recommends using <c>TEXT</c> and <c>IMAGE</c>.
    /// </summary>
    private static readonly string[] ObsoleteAttributeMetadataNames =
    [
        "Carrigan.SqlTools.Attributes.SqlTextAttribute",
        "Carrigan.SqlTools.Attributes.SqlImageAttribute"
    ];

    /// <summary>
    /// Attributes associated with legacy SQL types with known structural limitations.
    /// <para>
    /// These types are still supported by SQL Server but have:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///   <c>MONEY</c> — fixed and limited scale; may truncate when applied to <c>decimal</c>.
    ///   </description></item>
    ///   <item><description>
    ///   <c>DATETIME</c> — limited range (1753–9999) and coarse resolution (3.33ms).
    ///   </description></item>
    /// </list>
    /// </summary>
    private static readonly string[] LegacyAttributeMetadataNames =
    [
        "Carrigan.SqlTools.Attributes.SqlMoneyAttribute",
        "Carrigan.SqlTools.Attributes.SqlDateTimeAttribute"
    ];

    #endregion

    #region Diagnostic Descriptors
    /// <summary>
    /// Reports an error when a SQL type attribute is applied to a type that is not
    /// allowed by the Carrigan.SqlTools mapping rules.
    /// </summary>
    public const string TypeDiagnosticId = "CARRIGANSQL0001";
    /// <summary>
    /// Reports an error when a SQL type attribute is applied to a type that is not
    /// allowed by the Carrigan.SqlTools mapping rules.
    /// </summary>
    private static readonly DiagnosticDescriptor TypeRule = new(
        id: TypeDiagnosticId,
        title: "Invalid Use Of Attribute Derived From SqlTypeAttribute",
        messageFormat: "Member '{0}' is marked with '{1}' and should not be applied to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute.");

    /// <summary>
    /// Reports a warning when a SQL type attribute may cause precision or scale loss
    /// when applied to a numeric type.
    /// </summary>
    public const string PrecisionDiagnosticId = "CARRIGANSQL0002";
    /// <summary>
    /// Reports a warning when a SQL type attribute may cause precision or scale loss
    /// when applied to a numeric type.
    /// </summary>
    private static readonly DiagnosticDescriptor PrecisionRule = new(
        id: PrecisionDiagnosticId,
        title: "Possible Precision Mismatch for SQL Type Attribute",
        messageFormat: "Member '{0}' is marked with '{1}' and may lose precision when applied to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Warns when a type combination may cause precision or scale mismatches between SQL and .NET representations.");

    /// <summary>
    /// Reports a warning when a SQL type attribute is applied to a type whose
    /// semantics (date-only, time-only, full date time) does not match the SQL type.
    /// </summary>
    public const string SemanticDiagnosticId = "CARRIGANSQL0003";
    /// <summary>
    /// Reports a warning when a SQL type attribute is applied to a type whose
    /// semantics (date-only, time-only, full date time) does not match the SQL type.
    /// </summary>
    private static readonly DiagnosticDescriptor SemanticRule = new(
        id: SemanticDiagnosticId,
        title: "Possible Semantic Mismatch for SQL Type Attribute",
        messageFormat: "Member '{0}' is marked with '{1}' and may represent a semantic mismatch to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Warns when a type combination may cause semantic mismatches between SQL and .NET representations.");

    /// <summary>
    /// Reports a warning when an attribute corresponding to a legacy SQL type is used.
    /// These types are supported but discouraged for new development.
    /// </summary>
    public const string LegacyDiagnosticId = "CARRIGANSQL0004";
    /// <summary>
    /// Reports a warning when an attribute corresponding to a legacy SQL type is used.
    /// These types are supported but discouraged for new development.
    /// </summary>
    private static readonly DiagnosticDescriptor LegacyRule = new(
        id: LegacyDiagnosticId,
        title: "Use of Attribute Associated with Legacy SQL Types DateTime and Money",
        messageFormat: "Member '{0}' is marked with '{1}', which is associated with a legacy SQL type",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks and warns when an attribute associated with legacy SQL types is used.");

    /// <summary>
    /// Reports a warning when an attribute corresponding to an obsolete SQL type is used.
    /// These types should not be used in modern database schemas.
    /// </summary>
    public const string ObsoleteDiagnosticId = "CARRIGANSQL0005";
    /// <summary>
    /// Reports a warning when an attribute corresponding to an obsolete SQL type is used.
    /// These types should not be used in modern database schemas.
    /// </summary>
    private static readonly DiagnosticDescriptor ObsoleteRule = new(
        id: ObsoleteDiagnosticId,
        title: "Use of Attribute Associated with Obsolete SQL Types",
        messageFormat: "Member '{0}' is marked with '{1}', which is associated with an obsolete SQL type",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Checks and warns when an attribute associated with obsolete SQL types is used.");

    #endregion

    public SqlTypeAttributeAnalyzer()
    {
    }

    /// <summary>
    /// Initializes the analyzer and registers the callbacks used to evaluate
    /// SQL type attribute usage across the compilation.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [TypeRule, PrecisionRule, SemanticRule, ObsoleteRule, LegacyRule];

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">context</param>
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

    /// <summary>
    /// Resolves a dictionary of metadata-name for types into Roslyn symbols for comparison.
    /// This allows the analyzer to reference types such as <c>DateOnly</c> and <c>TimeOnly</c>
    /// </summary>
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

    /// <summary>
    /// Builds a set of attribute symbols corresponding to legacy or obsolete SQL types.
    /// </summary>
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

    /// <summary>
    /// Resolves a type by metadata name, including array support.
    /// </summary>
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

    /// <summary>
    /// Performs the main analysis for SQL type attribute usage on a property.
    /// Multiple diagnostics may be produced for a single property if it violates
    /// more than one rule category (precision, semantic, legacy, etc.).
    /// </summary>
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

    /// <summary>
    /// Extracts the underlying CLR type of <c>Nullable&lt;T&gt;</c> so comparisons use
    /// the non-nullable T.
    /// </summary>
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

    /// <summary>
    /// Gets the appropriate <see cref="Location"/> for a diagnostic tied to an attribute.
    /// </summary>
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
