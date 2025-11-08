using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Carrigan.SqlTools.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SqlTypeAttributeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CARRIGANSQL0001";
    private const string MessageFormat = "Member '{0}' is marked with 'SqlBinaryAttribute' and should not be applied to type '{1}'";
    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Invalid SqlBinaryAttribute Application",
        messageFormat: MessageFormat,
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
    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        IPropertySymbol property = (IPropertySymbol)context.Symbol;
        IArrayTypeSymbol byteArrayType = context.Compilation.CreateArrayTypeSymbol(
                context.Compilation.GetSpecialType(SpecialType.System_Byte));
        INamedTypeSymbol? sqlBinaryAttrSymbol =
            context.Compilation.GetTypeByMetadataName("Carrigan.SqlTools.Attributes.SqlBinaryAttribute");

        Location attributeLocation;

        if (sqlBinaryAttrSymbol is not null)
        {
            foreach (AttributeData attr in property.GetAttributes())
            {
                INamedTypeSymbol? attributeClass = attr.AttributeClass;
                if (attributeClass is not null)
                {
                    if (SymbolEqualityComparer.Default.Equals(attributeClass, sqlBinaryAttrSymbol))
                    {
                        if (SymbolEqualityComparer.Default.Equals(property.Type, byteArrayType) is false)
                        {
                            attributeLocation = attr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                                   ?? property.Locations.FirstOrDefault()
                                   ?? Location.None;
                            context.ReportDiagnostic(Diagnostic.Create(
                                Rule,
                                attributeLocation,
                                property.Name, property.Type.Name));
                        }
                    }
                }
            }
        }
    }
}
