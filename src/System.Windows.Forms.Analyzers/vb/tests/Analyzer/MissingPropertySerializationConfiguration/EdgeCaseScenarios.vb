' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms
Imports System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.WinForms.Test
Imports Microsoft.WinForms.Utilities.Shared
Imports Xunit

Namespace Global.System.Windows.Forms.Analyzers.VisualBasic.Tests.AnalyzerTests.MissingPropertySerializationConfiguration

    ''' <summary>
    '''  Tests specific edge cases for the MissingPropertySerializationConfigurationAnalyzer:
    '''  - Static properties which should not get flagged
    '''  - Properties in classes implementing non-System.ComponentModel.IComponent interfaces
    '''  - Properties with private setters
    '''  - Inherited properties that are already attributed correctly
    ''' </summary>
    Public Class EdgeCaseScenarios
        Inherits RoslynAnalyzerAndCodeFixTestBase(Of MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier)

        ''' <summary>
        '''  Initializes a new instance of the <see cref="EdgeCaseScenarios"/> class.
        ''' </summary>
        Public Sub New()
            MyBase.New(SourceLanguage.VisualBasic)
        End Sub

        ''' <summary>
        '''  Retrieves reference assemblies for the latest target framework versions.
        ''' </summary>
        Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
            Dim tfms As NetVersion() = {
                NetVersion.Net6_0,
                NetVersion.Net7_0,
                NetVersion.Net8_0,
                NetVersion.WinFormsBuild ' Build from artifacts folder
            }

            For Each refAssembly In ReferenceAssemblyGenerator.GetForLatestTFMs(tfms)
                Yield New Object() {refAssembly}
            Next
        End Function

        ''' <summary>
        '''  Tests that the analyzer correctly handles edge cases:
        '''  - Not flagging static properties
        '''  - Not flagging properties in classes that implement non-System.ComponentModel.IComponent
        '''  - Not flagging properties with private setters
        '''  - Not flagging overridden properties when the base is properly attributed
        ''' </summary>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function TestAnalyzerDiagnostics(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task

            Dim diagnosticId = DiagnosticIDs.MissingPropertySerializationConfiguration

            Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)

            ' We expect no diagnostics for most edge cases
            context.ExpectedDiagnostics.Clear()

            ' Only expect diagnostic on the one property that should be flagged
            context.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerError(diagnosticId).WithSpan(110, 25, 110, 40))

            Await context.RunAsync()
        End Function
    End Class

End Namespace
