// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Forms.CSharp.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MissingPropertySerializationConfigurationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [CSharpDiagnosticDescriptors.s_missingPropertySerializationConfiguration];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(context =>
        {
            var iSiteSymbol = context.Compilation.GetTypeByMetadataName(typeof(ISite).FullName);
            var iComponentSymbol = context.Compilation.GetTypeByMetadataName(typeof(IComponent).FullName);
            var designerSerializationVisibilitySymbol = context.Compilation.GetTypeByMetadataName(typeof(DesignerSerializationVisibilityAttribute).FullName);
            var defaultValueSymbol = context.Compilation.GetTypeByMetadataName(typeof(DefaultValueAttribute).FullName);
            context.RegisterSymbolAction(context => AnalyzeSymbol(context, iSiteSymbol, iComponentSymbol, designerSerializationVisibilitySymbol, defaultValueSymbol), SymbolKind.Property);
        });
    }

    private static void AnalyzeSymbol(
        SymbolAnalysisContext context,
        INamedTypeSymbol? iSiteSymbol,
        INamedTypeSymbol? iComponentSymbol,
        INamedTypeSymbol? designerSerializationVisibilitySymbol,
        INamedTypeSymbol? defaultValueSymbol)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;
        if (propertySymbol.IsStatic)
        {
            return;
        }

        // A property of System.ComponentModel.ISite we never flag.
        if (propertySymbol.Type.Equals(iSiteSymbol, SymbolEqualityComparer.Default))
        {
            return;
        }

        // If the property is part of any interface named IComponent, we're out.
        if (propertySymbol.ContainingType.Name == nameof(IComponent))
        {
            return;
        }

        // Does the property belong to a class which implements the System.ComponentModel.IComponent interface?
        if (iComponentSymbol is not null &&
            !propertySymbol.ContainingType.AllInterfaces.Contains(iComponentSymbol, SymbolEqualityComparer.Default))
        {
            return;
        }

        // Is the property read/write and at least internal and doesn't have a private setter?
        if (propertySymbol.SetMethod is not IMethodSymbol propertySetter
            || propertySetter.DeclaredAccessibility == Accessibility.Private
            || propertySymbol.DeclaredAccessibility < Accessibility.Internal)
        {
            return;
        }

        // Skip overridden properties since the base property should already have the appropriate serialization configuration
        if (propertySymbol.IsOverride)
        {
            return;
        }

        // Is the property attributed with DesignerSerializationVisibility or DefaultValue?
        if (propertySymbol.GetAttributes()
            .Any(a => a.AttributeClass is not null &&
                    (a.AttributeClass.Equals(designerSerializationVisibilitySymbol, SymbolEqualityComparer.Default) ||
                    a.AttributeClass.Equals(defaultValueSymbol, SymbolEqualityComparer.Default))))
        {
            return;
        }

        // Now, it get's a bit more tedious:
        // If the Serialization is managed via ShouldSerialize and Reset methods, we are also fine,
        // so let's check for that. First, let's get the class of the property:
        INamedTypeSymbol classSymbol = propertySymbol.ContainingType;

        // Now, let's check if the class has a method ShouldSerialize method:
        // Let's make sure the method returns a bool and has no parameters:
        IMethodSymbol? shouldSerializeMethod = classSymbol
            .GetMembers($"ShouldSerialize{propertySymbol.Name}")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.ReturnType.SpecialType == SpecialType.System_Boolean && m.Parameters.IsEmpty);

        if (shouldSerializeMethod is null)
        {
            // For ALL such other symbols, produce a diagnostic.
            var diagnostic = Diagnostic.Create(
                descriptor: CSharpDiagnosticDescriptors.s_missingPropertySerializationConfiguration,
                location: propertySymbol.Locations[0],
                messageArgs: propertySymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
