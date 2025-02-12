' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Runtime.InteropServices.ComTypes
Imports System.Windows.Forms.Analyzers
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace System.Windows.Forms.VisualBasic.Analyzers.ImplementITypedDataObjectInAdditionToIDataObject

    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public NotInheritable Class ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer
        Inherits DiagnosticAnalyzer

        Private Const NamespaceName As String = "System.Windows.Forms"

        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor) =
            ImmutableArray.Create(s_implementITypedDataObjectInAdditionToIDataObject)

        Public Overrides Sub Initialize(context As AnalysisContext)
            context.EnableConcurrentExecution()
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
            context.RegisterSyntaxNodeAction(AddressOf AnalyzeClassBlock, SyntaxKind.ClassBlock)
        End Sub

        Private Sub AnalyzeClassBlock(context As SyntaxNodeAnalysisContext)
            Dim classBlock = TryCast(context.Node, ClassBlockSyntax)
            If classBlock Is Nothing Then
                Return
            End If

            If classBlock.Implements.Count = 0 AndAlso classBlock.Inherits.Count = 0 Then
                Return
            End If

            Dim semanticModel = context.SemanticModel
            Dim compilation = semanticModel.Compilation

            ' Check if the System.Windows.Forms assembly is referenced
            If Not compilation.ReferencedAssemblyNames.Any(Function(assembly) assembly.Name = "System.Windows.Forms") Then
                Return
            End If

            Dim classSymbol = semanticModel.GetDeclaredSymbol(classBlock)
            If classSymbol Is Nothing Then
                Return
            End If

            Dim interfaces = classSymbol.AllInterfaces
            Dim implementsITypedDataObject = interfaces _
                .Any(Function(i) i.Name = "ITypedDataObject" AndAlso i.ContainingNamespace.ToDisplayString() = NamespaceName)
            If implementsITypedDataObject Then
                Return
            End If

            Dim implementsIDataObject = interfaces _
                .Any(Function(i) i.Name = "IDataObject" AndAlso i.ContainingNamespace.ToDisplayString() = NamespaceName)
            If Not implementsIDataObject Then
                Return
            End If

            ' Report if it implements IDataObject but NOT ITypedDataObject.
            Dim identifier As SyntaxToken = classBlock.ClassStatement.Identifier
            Dim diagnostic As Diagnostic = Diagnostic.Create(
                s_implementITypedDataObjectInAdditionToIDataObject,
                identifier.GetLocation(),
                identifier.Text)

            context.ReportDiagnostic(diagnostic)
        End Sub
    End Class

End Namespace
