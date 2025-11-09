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
    private static readonly Dictionary<string, Type[]> attributeTypes =
        new KeyValuePair<string, Type[]>[]
        {
            new ("SqlBinaryAttribute", [typeof(byte[])]),
            new ("SqlCharAttribute", [typeof(char), typeof(string)]),
            new ("SqlDateTime2Attribute", [typeof(DateTime), typeof(DateOnly)]),
            new ("SqlDateTimeAttribute", [typeof(DateTime), typeof(DateOnly)]),
            new ("SqlDateTimeOffsetAttribute", [typeof(DateTimeOffset)]),
            new ("SqlDecimalAttribute", [typeof(decimal)]),
            new ("SqlFloatAttribute", [typeof(float)]),
            new ("SqlMoneyAttribute", [typeof(decimal)]),
            new ("SqlTimeAttribute", [typeof(TimeOnly)]),
            new ("SqlVarBinaryMaxAttribute", [typeof(byte[])]),
            new ("SqlVarCharMaxAttribute", [typeof(char), typeof(string)])
        }.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
    public const string DiagnosticId = "CARRIGANSQL0001";
    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: $"Invalid Use Of Attribute Derived From SqlTypeAttribute",
        messageFormat: "Member '{0}' is marked with '{1}' and should not be applied to type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Enforces type safety with the use of the various attributes that inherit from SqlTypeAttribute.");

    public SqlTypeAttributeAnalyzer()
    {
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        [Rule];

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
    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        IPropertySymbol property = (IPropertySymbol)context.Symbol;
        INamedTypeSymbol? attributeClass;
        Location attributeLocation;
        bool isValid;

        foreach (AttributeData attribute in property.GetAttributes())
        {
            attributeClass = attribute.AttributeClass;
            if (attributeClass is not null && attributeTypes.TryGetValue(attributeClass.Name, out Type[]? value))
            {
                isValid = value.Select(type => GetTypeSymbol(context.Compilation, type))
                    .OfType<ITypeSymbol>()
                    .Any(validSymbol => SymbolEqualityComparer.Default.Equals(property.Type, validSymbol));
                if (isValid == false)
                {
                    attributeLocation = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                            ?? property.Locations.FirstOrDefault()
                            ?? Location.None;
                    context.ReportDiagnostic(Diagnostic.Create(
                        Rule,
                        attributeLocation,
                        property.Name, 
                        attributeClass.Name, 
                        property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                }                   
            }
        }
    }
}
