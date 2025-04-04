' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.ComponentModel
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics

Namespace Global.System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration

    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public Class MissingPropertySerializationConfigurationAnalyzer
        Inherits DiagnosticAnalyzer

        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(s_missingPropertySerializationConfiguration)
            End Get
        End Property

        Public Overrides Sub Initialize(context As AnalysisContext)
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
            context.EnableConcurrentExecution()
            context.RegisterSymbolAction(AddressOf AnalyzeSymbol, SymbolKind.Property)
        End Sub

        Private Shared Sub AnalyzeSymbol(context As SymbolAnalysisContext)

            ' We analyze only properties.
            Dim propertySymbol As IPropertySymbol = TryCast(context.Symbol, IPropertySymbol)
            If propertySymbol Is Nothing Then
                Return
            End If

            ' Does the property belong to a class which derives from Component?
            If propertySymbol.ContainingType Is Nothing OrElse
               Not propertySymbol.ContainingType.AllInterfaces.Any(
                Function(i) i.Name = NameOf(IComponent)) Then

                Return
            End If

            ' Is the property read/write and at least internal?
            If propertySymbol.SetMethod Is Nothing OrElse
               propertySymbol.DeclaredAccessibility < Accessibility.Internal Then

                Return
            End If

            ' Is the property attributed with DesignerSerializationVisibility or DefaultValue?
            If propertySymbol.GetAttributes().Any(
                Function(a) a?.AttributeClass?.Name = NameOf(DesignerSerializationVisibilityAttribute) OrElse
                    a?.AttributeClass?.Name = NameOf(DefaultValueAttribute)) Then

                Return
            End If

            ' Now, it gets a bit more tedious:
            ' If the Serialization is managed via ShouldSerialize and Reset methods, we are also fine,
            ' so let's check for that. First, let's get the class of the property:
            Dim classSymbol As INamedTypeSymbol = propertySymbol.ContainingType

            ' Now, let's check if the class has a method ShouldSerialize method:
            Dim shouldSerializeMethod As IMethodSymbol = classSymbol.GetMembers().
                OfType(Of IMethodSymbol)().
                FirstOrDefault(Function(m) m.Name = $"ShouldSerialize{propertySymbol.Name}")

            ' Let's make sure the method returns a bool and has no parameters:
            If shouldSerializeMethod Is Nothing OrElse
               shouldSerializeMethod.ReturnType.SpecialType <> SpecialType.System_Boolean OrElse
               shouldSerializeMethod.Parameters.Length > 0 Then

                ' For ALL such other symbols, produce a diagnostic.
                Dim diagnostic As Diagnostic = Diagnostic.Create(
                    descriptor:=s_missingPropertySerializationConfiguration,
                    location:=propertySymbol.Locations(0),
                    propertySymbol.Name)

                context.ReportDiagnostic(diagnostic)
            End If
        End Sub
    End Class
End Namespace
