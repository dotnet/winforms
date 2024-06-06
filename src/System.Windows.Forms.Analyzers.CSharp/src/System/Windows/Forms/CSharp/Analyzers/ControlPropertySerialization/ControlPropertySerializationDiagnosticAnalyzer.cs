using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.ControlPropertySerialization;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ControlPropertySerializationDiagnosticAnalyzer : DiagnosticAnalyzer
{
    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization

    private const string Category = "WinForms Security";

    private static readonly DiagnosticDescriptor s_rule = new(
        id: DiagnosticIDs.ControlPropertySerialization,
        title: new LocalizableResourceString(nameof(SR.WFAC003AnalyzerTitle), SR.ResourceManager, typeof(SR)),
        messageFormat: new LocalizableResourceString(nameof(SR.WFAC003AnalyzerMessageFormat), SR.ResourceManager, typeof(SR)),
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: new LocalizableResourceString(nameof(SR.WFAC003AnalyzerDescription), SR.ResourceManager, typeof(SR)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(s_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Property);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        // We analyze only properties.
        var propertySymbol = (IPropertySymbol)context.Symbol;

        // Does the property belong to a class which derives from Component?
        if (propertySymbol.ContainingType is null
            || !propertySymbol
                .ContainingType
                .AllInterfaces
                .Any(i => i.Name == nameof(IComponent)))
        {
            return;
        }

        // Is the read/write and at least internal?
        if (propertySymbol.SetMethod is null
            || propertySymbol.DeclaredAccessibility < Accessibility.Internal)
        {
            return;
        }

        // Is the property attributed with DesignerSerializationVisibility or DefaultValue?
        if (propertySymbol.GetAttributes()
            .Any(a => a?.AttributeClass?.Name is (nameof(DesignerSerializationVisibilityAttribute))
                or (nameof(DefaultValueAttribute))))
        {
            return;
        }

        // Now, it get's a bit more tedious:
        // If the Serialization is managed via ShouldSerialize and Reset methods, we are also fine,
        // so let's check for that. First, let's get the class of the property:
        INamedTypeSymbol classSymbol = propertySymbol.ContainingType;

        // Now, let's check if the class has a method ShouldSerialize method:
        IMethodSymbol? shouldSerializeMethod = classSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.Name == $"ShouldSerialize{propertySymbol.Name}");

        // Let's make sure the method returns a bool and has no parameters:
        if (shouldSerializeMethod is null
            || shouldSerializeMethod.ReturnType.SpecialType != SpecialType.System_Boolean
            || shouldSerializeMethod.Parameters.Length > 0)
        {

            // For ALL such other symbols, produce a diagnostic.
            var diagnostic = Diagnostic.Create(s_rule, propertySymbol.Locations[0], propertySymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
