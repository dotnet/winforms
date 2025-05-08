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

        Private Const SystemComponentModelName As String = "System.ComponentModel"

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

            ' A property of System.ComponentModel.ISite we never flag.
            If propertySymbol.Type.Name = NameOf(ISite) AndAlso
               propertySymbol.Type.ContainingNamespace.ToString() = SystemComponentModelName Then
                Return
            End If

            ' If the property is part of any interface named IComponent, we're out.
            If propertySymbol.ContainingType.Name = NameOf(IComponent) Then
                Return
            End If

            ' Skip static properties since they are not serialized by the designer
            If propertySymbol.IsStatic Then
                Return
            End If

            ' Is the property read/write, at least internal, and doesn't have a private setter?
            If propertySymbol.SetMethod Is Nothing OrElse
               propertySymbol.SetMethod.DeclaredAccessibility = Accessibility.Private OrElse
               propertySymbol.DeclaredAccessibility < Accessibility.Internal Then
                Return
            End If

            ' Skip overridden properties since the base property should already
            ' have the appropriate serialization configuration
            If propertySymbol.IsOverride Then
                Return
            End If

            ' Does the property belong to a class which implements the System.ComponentModel.IComponent interface?
            If propertySymbol.ContainingType Is Nothing OrElse
               Not propertySymbol.ContainingType.AllInterfaces.Any(
                Function(i) i.Name = NameOf(IComponent) AndAlso
                          i.ContainingNamespace IsNot Nothing AndAlso
                          i.ContainingNamespace.ToString() = SystemComponentModelName) Then
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
