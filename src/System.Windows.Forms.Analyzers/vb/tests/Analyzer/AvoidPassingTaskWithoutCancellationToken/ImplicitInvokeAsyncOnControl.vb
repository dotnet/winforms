' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms
Imports System.Windows.Forms.VisualBasic.Analyzers.AvoidPassingTaskWithoutCancellationToken
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.WinForms.Test
Imports Microsoft.WinForms.Utilities.Shared
Imports Xunit

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests.AnalyzerTests.AvoidPassingTaskWithoutCancellationToken

    ''' <summary>
    '''  Tests for the AvoidPassingTaskWithoutCancellationTokenAnalyzer that verify it correctly
    '''  detects InvokeAsync calls without explicit 'Me' keyword.
    ''' </summary>
    Public Class ImplicitInvokeAsyncOnControl
        Inherits RoslynAnalyzerAndCodeFixTestBase(Of AvoidPassingTaskWithoutCancellationTokenAnalyzer, DefaultVerifier)

        ''' <summary>
        '''  Initializes a new instance of the <see cref="ImplicitInvokeAsyncOnControl"/> class.
        ''' </summary>
        Public Sub New()
            MyBase.New(SourceLanguage.VisualBasic)
        End Sub

        ''' <summary>
        '''  Retrieves reference assemblies for the latest target framework versions.
        ''' </summary>
        Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
            Dim tfms As NetVersion() = {
                NetVersion.Net9_0
            }

            For Each refAssembly In ReferenceAssemblyGenerator.GetForLatestTFMs(tfms)
                Yield New Object() {refAssembly}
            Next
        End Function

        ''' <summary>
        '''  Tests that the analyzer detects InvokeAsync calls with Task return types
        '''  even when the 'Me' keyword is omitted.
        ''' </summary>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function DetectImplicitInvokeAsyncCalls(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task

            ' Make sure, we can resolve the assembly we're testing against:
            Dim referenceAssembly = Await referenceAssemblies.ResolveAsync(
                language:=String.Empty,
                cancellationToken:=CancellationToken.None)

            Dim diagnosticId As String = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken

            Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)

            ' Explicitly specify where diagnostics are expected
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(18, 19, 20, 44))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(22, 19, 24, 47))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 19, 43, 44))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(73, 19, 75, 44))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(82, 19, 84, 47))

            Await context.RunAsync()
        End Function
    End Class

End Namespace
