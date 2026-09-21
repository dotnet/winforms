' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Tests
Imports System.Windows.Forms.VisualBasic.Analyzers.Designer
Imports Microsoft.CodeAnalysis.Testing
Imports Xunit

''' <summary>
'''  Tests the distinction between member-based and local-variable Designer event wiring.
''' </summary>
<ForceGC()>
<SkipOnArchitecture(TestArchitectures.X86, "Analyzer tests require Roslyn reference assembly extraction")>
Public Class DesignerEventAnalyzerTests
    Private Const MainSource As String = "
Imports System
Imports System.Windows.Forms
Partial Class Form1
    Inherits Form
    Private Sub HandleClick(sender As Object, e As EventArgs)
    End Sub
End Class
"

    <Theory>
    <InlineData("Private WithEvents Button1 As Button")>
    <InlineData("Private {|WFO3011:Button1|} As Button")>
    Public Async Function MemberEvent_RequiresHandles(field As String) As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            $"
Imports System.Windows.Forms
Partial Class Form1
    Private Sub InitializeComponent()
        Button1 = New Button()
        Button1 = New Button()
        {{|WFO3012:AddHandler|}} Button1.Click, AddressOf HandleClick
    End Sub
    {field}
End Class
")

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function LocalAndRuntimeEvents_NoDiagnostic() As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            "
Imports System
Imports System.ComponentModel
Imports System.Windows.Forms
Partial Class Form1
    Private Sub InitializeComponent()
        components = New Container()
        Dim local As New Button()
        AddHandler local.Click, AddressOf HandleClick
        Dim helper As New Other.Component()
    End Sub
    Private components As IContainer
End Class
Namespace Other
    Public Class Component
    End Class
End Namespace
")
        testCase.Sources.Add(New AnalyzerTestSource("Runtime.vb",
            "
Partial Class Form1
    Private Sub ConfigureRuntime()
        AddHandler Me.Click, AddressOf HandleClick
    End Sub
End Class
"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Theory>
    <InlineData("Me.", "Me")>
    <InlineData("MyBase.", "MyBase")>
    <InlineData("MyClass.", "MyClass")>
    <InlineData("myclass.", "MyClass")>
    <InlineData("", "Me")>
    Public Async Function RootEvent_RequiresHandles(receiver As String, expectedReceiver As String) As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            $"
Partial Class Form1
    Private Sub InitializeComponent()
        {{|#0:AddHandler|}} {receiver}Click, AddressOf HandleClick
    End Sub
End Class
")
        testCase.ExpectedDiagnostics.Add(
            New DiagnosticResult("WFO3012", Microsoft.CodeAnalysis.DiagnosticSeverity.Error).
                WithLocation(0).
                WithArguments($"{expectedReceiver}.Click"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Theory>
    <InlineData("Me")>
    <InlineData("MyBase")>
    <InlineData("MyClass")>
    Public Async Function RootEvent_HandlesRepair_NoDiagnostic(receiver As String) As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            "
Partial Class Form1
    Private Sub InitializeComponent()
    End Sub
End Class
")
        testCase.Sources.Add(New AnalyzerTestSource("Handlers.vb",
            $"
Partial Class Form1
    Private Sub RootClick(sender As Object, e As System.EventArgs) Handles {receiver}.Click
    End Sub
End Class
"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function WithEventsAndHandles_NoDiagnostic() As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            "
Imports System.ComponentModel
Imports System.Windows.Forms
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
    Private components As IContainer
    Private Sub InitializeComponent()
        Button1 = New Button()
        SuspendLayout()
        Controls.Add(Button1)
        ResumeLayout(False)
    End Sub
    Friend WithEvents Button1 As Button
End Class
")
        testCase.Sources.Add(New AnalyzerTestSource("Handlers.vb",
            "
Partial Class Form1
    Private Sub ButtonClick(sender As Object, e As System.EventArgs) Handles Button1.Click
    End Sub
End Class
"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of InitializeComponentAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerFileStructureAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function InheritedWithEvents_RequiresHandlesWithoutRedeclaration() As Task
        Dim testCase As New AnalyzerTestCase(
            ReferenceAssemblies.Net.Net90Windows,
            New AnalyzerTestSource("Form1.vb",
                "
Imports System
Imports System.Windows.Forms
Class BaseForm
    Inherits Form
    Protected WithEvents InheritedButton As Button
    Protected PlainButton As Button
End Class
Partial Class Form1
    Inherits BaseForm
    Private Sub HandleClick(sender As Object, e As EventArgs)
    End Sub
End Class
"),
            New AnalyzerTestSource("Form1.Designer.vb",
                "
Partial Class Form1
    Private Sub InitializeComponent()
        {|WFO3012:AddHandler|} InheritedButton.Click, AddressOf HandleClick
        AddHandler PlainButton.Click, AddressOf HandleClick
    End Sub
End Class
"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function Diagnostics_CanBeSuppressedIndividually() As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            "
Imports System.Windows.Forms
Partial Class Form1
    Private Sub InitializeComponent()
        Button1 = New Button()
        {|WFO3012:AddHandler|} Button1.Click, AddressOf HandleClick
    End Sub
    Private Button1 As Button
End Class
")
        testCase.AnalyzerConfigFiles.Add(("/.globalconfig", "
is_global = true
dotnet_diagnostic.WFO3011.severity = none
"))

        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    Private Shared Function CreateTestCase(designer As String) As AnalyzerTestCase
        Return New AnalyzerTestCase(
            ReferenceAssemblies.Net.Net90Windows,
            New AnalyzerTestSource("Form1.vb", MainSource),
            New AnalyzerTestSource("Form1.Designer.vb", designer))
    End Function

    <Theory>
    <InlineData("System.ComponentModel.Component", True)>
    <InlineData("Other.Component", False)>
    Public Async Function ComponentMember_UsesFrameworkIdentity(memberType As String, expected As Boolean) As Task
        Dim member As String = If(expected, "{|WFO3011:component1|}", "component1")
        Dim hookup As String = If(expected, "{|WFO3012:AddHandler|}", "AddHandler")
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            $"
Partial Class Form1
    Private Sub InitializeComponent()
        component1 = (New {memberType}())
        {hookup} component1.Disposed, AddressOf HandleClick
    End Sub
    Private {member} As {memberType}
End Class
Namespace Other
    Public Class Component
        Public Event Disposed As System.EventHandler
    End Class
End Namespace
")
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function

    <Fact>
    Public Async Function UnrelatedWithEventsType_NoDiagnostic() As Task
        Dim testCase As AnalyzerTestCase = CreateTestCase(
            "
Partial Class Form1
    Private Sub InitializeComponent()
        helper = New Other.Component()
        AddHandler helper.Changed, AddressOf HandleClick
    End Sub
    Private WithEvents helper As Other.Component
End Class
Namespace Other
    Public Class Component
        Public Event Changed As System.EventHandler
    End Class
End Namespace
")
        Await AnalyzerTestFactory.CreateVisualBasicAnalyzerTest(Of DesignerEventAnalyzer)(testCase).
            RunAsync(TestContext.Current.CancellationToken)
    End Function
End Class
