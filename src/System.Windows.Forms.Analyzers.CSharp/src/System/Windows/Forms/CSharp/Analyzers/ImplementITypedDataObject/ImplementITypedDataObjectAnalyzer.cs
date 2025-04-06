// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.CSharp.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.ImplementITypedDataObject;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ImplementITypedDataObjectAnalyzer : DiagnosticAnalyzer
{
    private const string Namespace = "System.Windows.Forms";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [CSharpDiagnosticDescriptors.s_implementITypedDataObjectInAdditionToIDataObject];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration
            || classDeclaration.BaseList is null)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var compilation = semanticModel.Compilation;

        // Check if the System.Windows.Forms assembly is referenced
        if (!compilation.ReferencedAssemblyNames
            .Any(assembly => assembly.Name == "System.Windows.Forms"))
        {
            return;
        }

        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            return;
        }

        var allInterfaces = classSymbol.AllInterfaces;
        bool implementsITypedDataObject = allInterfaces
           .Any(i => i.Name == "ITypedDataObject" && i.ContainingNamespace.ToDisplayString() == Namespace);
        if (implementsITypedDataObject)
        {
            return;
        }

        bool implementsIDataObject = allInterfaces
            .Any(i => i.Name == "IDataObject" && i.ContainingNamespace.ToDisplayString() == Namespace);
        if (!implementsIDataObject)
        {
            return;
        }

        // Report if it implements IDataObject but NOT ITypedDataObject.
        var identifier = classDeclaration.Identifier;
        var diagnostic = Diagnostic.Create(
            CSharpDiagnosticDescriptors.s_implementITypedDataObjectInAdditionToIDataObject,
            identifier.GetLocation(),
            messageArgs: identifier.Text);

        context.ReportDiagnostic(diagnostic);
    }
}
