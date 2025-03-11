' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms
Imports System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration
Imports System.Windows.Forms.VisualBasic.CodeFixes.AddDesignerSerializationVisibility
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.WinForms.Test
Imports Microsoft.WinForms.Utilities.Shared
Imports Xunit

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests.AnalyzerTests.MissingPropertySerializationConfiguration

    ''' <summary>
    '''  Represents a set of test scenarios for custom controls to verify
    '''  property serialization behavior.
    ''' </summary>
    ''' <remarks>
    '''  <para>
    '''   This class is derived from <see cref="RoslynAnalyzerAndCodeFixTestBase(Of TAnalyzer, TVerifier)"/>
    '''   and is intended to validate how properties are serialized in custom controls during
    '''   analyzer and code-fix operations.
    '''  </para>
    '''  <para>
    '''   Use the provided methods to test scenarios such as missing property serialization and to
    '''   confirm that the code fix provider can correct those issues.
    '''  </para>
    ''' </remarks>
    Public Class CustomControlScenarios
        Inherits RoslynAnalyzerAndCodeFixTestBase(Of MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier)

        ''' <summary>
        '''  Initializes a new instance of the <see cref="CustomControlScenarios"/> class.
        ''' </summary>
        ''' <remarks>
        '''  <para>
        '''   Calls the base constructor with the Visual Basic source language to set up the
        '''   environment for analyzer tests.
        '''  </para>
        ''' </remarks>
        Public Sub New()
            MyBase.New(SourceLanguage.VisualBasic)
        End Sub

        ''' <summary>
        '''  Retrieves reference assemblies for the latest target framework versions.
        ''' </summary>
        ''' <remarks>
        '''  <para>
        '''   This method generates an enumerable of object arrays, each containing
        '''   reference assemblies for different .NET target framework versions.
        '''  </para>
        '''  <para>
        '''   It uses the <see cref="ReferenceAssemblyGenerator.GetForLatestTFMs"/> method
        '''   to fetch the reference assemblies for the specified target framework versions.
        '''   Using this approach guarantees the latest reference assembly versions for each
        '''   respective TFM.
        '''  </para>
        ''' </remarks>
        ''' <returns>
        '''  An enumerable of object arrays, each containing a set of reference assemblies.
        ''' </returns>
        Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
            Dim tfms As NetVersion() = {
                NetVersion.Net6_0,
                NetVersion.Net7_0,
                NetVersion.Net8_0,
                NetVersion.Net9_0
            }

            For Each refAssembly In ReferenceAssemblyGenerator.GetForLatestTFMs(tfms)
                Yield New Object() {refAssembly}
            Next
        End Function

        ''' <summary>
        '''  Tests the diagnostics produced by
        '''  <see cref="MissingPropertySerializationConfigurationAnalyzer"/>.
        ''' </summary>
        ''' <remarks>
        '''  <para>
        '''   First, it verifies that the initial diagnostics match the
        '''   expected results. Then, it re-runs the test context to confirm
        '''   that any corrections remain consistent.
        '''  </para>
        '''  <para>
        '''   This method depends on the provided file set and reference
        '''   assemblies to generate the appropriate environment for analysis.
        '''  </para>
        ''' </remarks>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function TestDiagnostics(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task
            Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)
            Await context.RunAsync()

            context = GetVisualBasicFixedTestContext(fileSet, referenceAssemblies)
            Await context.RunAsync()
        End Function

        ''' <summary>
        '''  Tests the code-fix provider to ensure it correctly applies designer serialization attributes.
        ''' </summary>
        ''' <remarks>
        '''  <para>
        '''   This uses the <see cref="AddDesignerSerializationVisibilityCodeFixProvider"/>
        '''   to verify that the fix properly addresses missing or invalid property serialization attributes.
        '''  </para>
        '''  <para>
        '''   It also configures the code-fix test behavior to skip fix-all
        '''   checks at the project and solution level, focusing on the
        '''   document-level fix itself.
        '''  </para>
        ''' </remarks>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function TestCodeFix(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task
            Dim context = GetVisualBasicCodeFixTestContext(Of AddDesignerSerializationVisibilityCodeFixProvider)(
                fileSet,
                referenceAssemblies,
                numberOfFixAllIterations:=-2)

            context.CodeFixTestBehaviors =
                CodeFixTestBehaviors.SkipFixAllInProjectCheck Or
                CodeFixTestBehaviors.SkipFixAllInSolutionCheck

            Await context.RunAsync()
        End Function
    End Class

End Namespace
