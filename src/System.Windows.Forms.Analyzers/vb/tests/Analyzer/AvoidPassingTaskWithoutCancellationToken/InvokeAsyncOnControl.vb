' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.VisualBasic.Analyzers.AvoidPassingTaskWithoutCancellationToken
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.WinForms.Test
Imports Microsoft.WinForms.Utilities.Shared
Imports Xunit

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests.AnalyzerTests.AvoidPassingTaskWithoutCancellationToken

    Public Class InvokeAsyncOnControl
        Inherits RoslynAnalyzerAndCodeFixTestBase(Of AvoidPassingTaskWithoutCancellationTokenAnalyzer, DefaultVerifier)

        Public Sub New()
            MyBase.New(SourceLanguage.VisualBasic)
        End Sub

        Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
            Yield New Object() {ReferenceAssemblies.Net.Net90.WithPackages(
                ImmutableArray.Create(New PackageIdentity("Microsoft.WindowsDesktop.App.Ref", "9.0.0")))}
        End Function

        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function AvoidPassingTaskWithoutCancellationAnalyzer(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task
            ' Make sure, we can resolve the assembly we're testing against:
            ' Always pass `String.Empty` for the language here to keep it generic.
            Dim referenceAssembly = Await referenceAssemblies.ResolveAsync(
                language:=String.Empty,
                cancellationToken:=CancellationToken.None)

            Dim diagnosticId As String = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken

            Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(37, 25, 37, 84))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(40, 25, 40, 84))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(43, 25, 43, 85))

            Await context.RunAsync()
        End Function
    End Class

End Namespace
