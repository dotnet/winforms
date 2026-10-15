' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Tests
Imports System.Windows.Forms.VisualBasic.Analyzers.Designer
Imports Microsoft.CodeAnalysis.Testing
Imports Xunit

''' <summary>
'''  Tests the Visual Basic WinForms Designer guardrails.
''' </summary>
<ForceGC()>
<SkipOnArchitecture(TestArchitectures.X86, "Analyzer tests require Roslyn reference assembly extraction")>
Public Class DesignerAnalyzerTests

    Private Const MainSource As String = "
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub
    End Class
End Namespace
"

    <Fact>
    Public Async Function InitializeComponent_ValidGeneratedCode_NoDiagnostic() As Task
        Const designerSource As String = "
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Private components As IContainer

        Protected Overrides Sub Dispose(disposing As Boolean)
            components?.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Button1 = New Button()
            Controls.Add(Button1)
        End Sub

        Private WithEvents Button1 As Button
    End Class
End Namespace
"

        Dim testCase As AnalyzerTestCase = CreateTestCase(designerSource)

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of DesignerFileStructureAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function InitializeComponent_ControlFlow_ReportsDiagnostics() As Task
        Const designerSource As String = "
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Private Sub InitializeComponent()
            {|WFO3000:For|} i As Integer = 0 To Controls.Count
            Next

            {|WFO3000:For|} Each control As Control In Controls
            Next

            {|WFO3000:While|} DesignMode
            End While

            {|WFO3000:Do|}
            Loop While DesignMode

            {|WFO3000:If|} DesignMode Then
            End If

            {|WFO3000:If|} DesignMode Then Text = Name

            {|WFO3000:Select|} Case Controls.Count
            End Select

            {|WFO3000:GoTo|} Done
Done:
            {|WFO3000:Try|}
            Catch
            End Try

            {|WFO3000:SyncLock|} Me
            End SyncLock
        End Sub
    End Class
End Namespace
"

        Dim testCase As AnalyzerTestCase = CreateTestCase(designerSource)

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function InitializeComponent_CodeDomUnsupportedExpressions_ReportDiagnostics() As Task
        Const designerSource As String = "
Imports System
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Private Sub InitializeComponent()
            Dim name As String = {|WFO3005:NameOf|}(Form1)
            Dim text As String = {|WFO3009:$""|}{name}: {Controls.Count}""
            Dim handler As EventHandler = {|WFO3010:Sub|}(sender, e) Text = text
            Dim value As Object = {|WFO3007:If|}(Tag, Me)
            Dim control As Control = {|WFO3008:Parent|}?.Parent
            Dim count As Integer = {|WFO3006:If|}(DesignMode, 0, Controls.Count)
        End Sub
    End Class
End Namespace
"

        Dim testCase As AnalyzerTestCase = CreateTestCase(designerSource)

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function DesignerMembers_ReportStructuralDiagnostics() As Task
        Const designerSource As String = "
Imports System
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Private WithEvents Button1 As Button

        Private Sub InitializeComponent()
            Button1 = New Button()
        End Sub

        Public Property {|WFO3001:Value|} As Integer
        Public Event {|WFO3003:Changed|} As EventHandler
        Public Delegate Sub {|WFO3003:Callback|}()
    End Class
End Namespace
"

        Dim testCase As AnalyzerTestCase = CreateTestCase(designerSource)
        testCase.ExpectedDiagnostics.Add(
            New DiagnosticResult("WFO3002", Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).
                WithSpan("/0/Form1.Designer.vb", 8, 28, 8, 35).
                WithArguments("Button1"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of DesignerFileStructureAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function MainFileField_ReferencedByInitializeComponent_ReportsDiagnostic() As Task
        Const mainSource As String = "
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Inherits Form

        Private WithEvents {|WFO3002:Button1|} As Button

        Public Sub New()
            InitializeComponent()
        End Sub
    End Class
End Namespace
"

        Const designerSource As String = "
Imports System.Windows.Forms

Namespace Test

    Partial Class Form1
        Private Sub InitializeComponent()
            Button1 = New Button()
            Controls.Add(Button1)
        End Sub
    End Class
End Namespace
"

        Dim testCase As New AnalyzerTestCase(
            ReferenceAssemblies.Net.Net90Windows,
            New AnalyzerTestSource("Form1.vb", mainSource),
            New AnalyzerTestSource("Form1.Designer.vb", designerSource))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of DesignerFileStructureAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function DesignerFile_MalformedMembers_ReportUnexpectedMember() As Task
        Const designerSource As String = "
Namespace Test

    Partial Class Form1
        Private Sub InitializeComponent()
        End Sub

        Private Sub {|WFO3001:Dispose|}(disposing As Integer)
        End Sub
    End Class
End Namespace
"

        Dim testCase As AnalyzerTestCase = CreateTestCase(designerSource)

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of DesignerFileStructureAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function UnrelatedDesignerNamedFile_NoDiagnostic() As Task
        Const mainSource As String = "
Namespace Test

    Partial Class Model
    End Class
End Namespace
"

        Const designerSource As String = "
Namespace Test

    Partial Class Model
        Private Sub InitializeComponent()
            If True Then
            End If
        End Sub

        Public Property Value As Integer
    End Class
End Namespace
"

        Dim testCase As New AnalyzerTestCase(
            ReferenceAssemblies.Net.Net90Windows,
            New AnalyzerTestSource("Model.vb", mainSource),
            New AnalyzerTestSource("Model.Designer.vb", designerSource))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of DesignerFileStructureAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function InitializeComponent_DiagnosticSuppressedByAnalyzerConfig() As Task
        Const designerSource As String = "
Namespace Test

    Partial Class Form1
        Private Sub InitializeComponent()
            If DesignMode Then
            End If
        End Sub
    End Class
End Namespace
"

        Dim testCase As AnalyzerTestCase = CreateTestCase(designerSource)
        testCase.AnalyzerConfigFiles.Add(
            (
                "/.globalconfig",
                "
is_global = true

dotnet_diagnostic.WFO3000.severity = none
"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    Private Shared Function CreateTestCase(designerSource As String) As AnalyzerTestCase
        Return New AnalyzerTestCase(
            ReferenceAssemblies.Net.Net90Windows,
            New AnalyzerTestSource("Form1.vb", MainSource),
            New AnalyzerTestSource("Form1.Designer.vb", designerSource))
    End Function

    <Theory>
    <InlineData("InitializeComponent")>
    <InlineData("initializecomponent")>
    <InlineData("INITIALIZECOMPONENT()")>
    Public Async Function InitializeComponent_OptionalSyntax_ReportsControlFlow(declaration As String) As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            $"
Namespace Test
    Partial Class Form1
        Private Sub {declaration}
            {{|WFO3000:If|}} True Then
            End If
        End Sub
    End Class
End Namespace
")

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)

        Dim structureCase As AnalyzerTestCase = CreateTestCase(
            testCase.Sources(1).Source.Replace("{|WFO3000:If|}", "If"))
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(
            Of DesignerFileStructureAnalyzer)(structureCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Theory>
    <InlineData("System.ComponentModel.Component", True)>
    <InlineData("ActualComponent", True)>
    <InlineData("DerivedComponent", True)>
    <InlineData("Other.Component", False)>
    <InlineData("Other.Control", False)>
    Public Async Function DesignerType_UsesFrameworkIdentity(baseType As String, expected As Boolean) As Task
        Dim condition As String = If(expected, "{|WFO3000:If|}", "If")
        Dim testCase As New AnalyzerTestCase(
            ReferenceAssemblies.Net.Net90Windows,
            New AnalyzerTestSource("Form1.vb", $"
Imports ActualComponent = System.ComponentModel.Component
Class DerivedComponent
    Inherits ActualComponent
End Class
Namespace Other
    Class Component
    End Class
    Class Control
    End Class
End Namespace
Partial Class Form1
    Inherits {baseType}
End Class
"),
            New AnalyzerTestSource("Form1.Designer.vb", $"
Partial Class Form1
    Private Sub InitializeComponent()
        {condition} True Then
        End If
    End Sub
End Class
"))
        Dim test = AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of InitializeComponentAnalyzer)(testCase)
        test.SolutionTransforms.Add(
            Function(solution, projectId) solution.WithProjectCompilationOptions(
                projectId,
                DirectCast(solution.GetProject(projectId).CompilationOptions,
                    Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilationOptions).WithRootNamespace("TestRoot")))

        Await test.RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function InitializeComponent_UsingAndAwait_ReportDiagnostics() As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            "
Namespace Test
    Partial Class Form1
        Private Async Sub InitializeComponent()
            {|WFO3000:Using|} component As New System.ComponentModel.Component()
            End Using
            {|WFO3000:Await|} System.Threading.Tasks.Task.CompletedTask
        End Sub
    End Class
End Namespace
")
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function
End Class
