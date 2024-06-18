' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.ComponentModel
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.Analyzers.VisualBasic.Resources
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics

' This is not really a VB-specific diagnostic, since they are not language agnostic.
' We want to stay consistent with the C# version, though, so we rename the class to keep using the variable name.
' (VB is not case-sensitive, so we can't have the same names in a code file, even if the casing is different.)
Imports VBDiagnostic = Microsoft.CodeAnalysis.Diagnostic

Namespace System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration

    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public Class MissingPropertySerializationConfigurationDiagnosticAnalyzer
        Inherits DiagnosticAnalyzer

        Private Shared ReadOnly s_rule As New DiagnosticDescriptor(
            id:=DiagnosticIDs.MissingPropertySerializationConfiguration,
            title:=New LocalizableResourceString(NameOf(SR.WFCA100AnalyzerTitle), SR.ResourceManager, GetType(SR)),
            messageFormat:=New LocalizableResourceString(NameOf(SR.WFCA100AnalyzerMessageFormat), SR.ResourceManager, GetType(SR)),
            category:=DiagnosticCategories.WinFormsSecurity,
            defaultSeverity:=DiagnosticSeverity.Error,
            isEnabledByDefault:=True,
            description:=New LocalizableResourceString(NameOf(SR.WFCA100AnalyzerDescription), SR.ResourceManager, GetType(SR))
        )

        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(s_rule)
            End Get
        End Property

        Public Overrides Sub Initialize(context As AnalysisContext)
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
            context.EnableConcurrentExecution()
            context.RegisterSymbolAction(AddressOf AnalyzeSymbol, SymbolKind.Property)
        End Sub

        Private Shared Sub AnalyzeSymbol(context As SymbolAnalysisContext)

            ' We analyze only properties.
            Dim propertySymbol = DirectCast(context.Symbol, IPropertySymbol)

            ' Does the property belong to a class which derives from Component?
            If propertySymbol.ContainingType Is Nothing _
                OrElse Not propertySymbol _
                    .ContainingType _
                    .AllInterfaces _
                    .Any(Function(i) i.Name = NameOf(IComponent)) Then

                Return
            End If

            ' Is the read/write and at least internal?
            If propertySymbol.SetMethod Is Nothing _
                OrElse propertySymbol.DeclaredAccessibility < Accessibility.Internal Then

                Return
            End If

            ' Is the property attributed with DesignerSerializationVisibility or DefaultValue?
            If propertySymbol _
                    .GetAttributes() _
                    .Any(Function(a) a?.AttributeClass?.Name = NameOf(DesignerSerializationVisibilityAttribute) _
                OrElse a?.AttributeClass?.Name = NameOf(DefaultValueAttribute)) Then

                Return
            End If

            ' Now, it gets a bit more tedious:
            ' If the Serialization is managed via ShouldSerialize and Reset methods, we are also fine,
            ' so let's check for that. First, let's get the class of the property:
            Dim classSymbol As INamedTypeSymbol = propertySymbol.ContainingType

            ' Now, let's check if the class has a method ShouldSerialize method:
            Dim shouldSerializeMethod As IMethodSymbol = classSymbol _
                .GetMembers() _
                .OfType(Of IMethodSymbol)() _
                .FirstOrDefault(Function(m) m.Name = $"ShouldSerialize{propertySymbol.Name}")

            ' Let's make sure the method returns a bool and has no parameters:
            If shouldSerializeMethod Is Nothing _
                OrElse shouldSerializeMethod.ReturnType.SpecialType <> SpecialType.System_Boolean _
                OrElse shouldSerializeMethod.Parameters.Length > 0 Then

                ' For ALL such other symbols, produce a diagnostic.
                Dim diagnostic = VBDiagnostic.Create(s_rule, propertySymbol.Locations(0), propertySymbol.Name)
                context.ReportDiagnostic(diagnostic)
            End If
        End Sub
    End Class
End Namespace
