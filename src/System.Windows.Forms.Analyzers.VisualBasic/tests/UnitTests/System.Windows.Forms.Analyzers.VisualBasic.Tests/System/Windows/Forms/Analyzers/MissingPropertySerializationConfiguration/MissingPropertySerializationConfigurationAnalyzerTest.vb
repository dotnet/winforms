Imports System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration
Imports System.Windows.Forms.VisualBasic.CodeFixes.AddDesignerSerializationVisibility
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.CodeAnalysis.VisualBasic.Testing
Imports Xunit

Public Class ControlPropertySerializationDiagnosticAnalyzerTest

    Private Const ProblematicCode As String =
"
Imports System.Drawing
Imports System.Windows.Forms

Namespace VBControls

    Public Module Program
        Public Sub Main()
            Dim control As New ScalableControl()

            ' We deliberately format this weirdly, to make sure we only format code our code fix touches.
            control.ScaleFactor = 1.5F
            control.ScaledSize = New SizeF(100, 100)
            control.ScaledLocation = New PointF(10, 10)
        End Sub
    End Module

    ' We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
    ' since this is nothing our code fix touches.
    Public Class ScalableControl
        Inherits System.Windows.Forms.Control

        Public Property [|ScaleFactor|] As Single = 1.0F

        Public Property [|ScaledSize|] As SizeF

        Public Property [|ScaledLocation|] As PointF
    End Class

End Namespace
"

    Private Const CorrectCode As String =
"Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace VBControls

    Public Module Program
        Public Sub Main()
            Dim control As New ScalableControl()
        
            control.ScaleFactor = 1.5F
            control.ScaledSize = New SizeF(100, 100)
            control.ScaledLocation = New PointF(10, 10)
        End Sub
    End Module
        
    Public Class ScalableControl
        Inherits Control

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaleFactor As Single = 1.0F

        <DefaultValue(GetType(SizeF), ""0,0"")>
        Public Property ScaledSize As SizeF

        Public Property ScaledLocation As PointF
        Private Function ShouldSerializeScaledLocation() As Boolean
            Return Me.ScaledLocation <> PointF.Empty
        End Function
    End Class
        
End Namespace
"

    Private Const FixedCode As String =
"Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace VBControls

    Public Module Program
        Public Sub Main()
            Dim control As New ScalableControl()

            ' We deliberately format this weirdly, to make sure we only format code our code fix touches.
            control.ScaleFactor = 1.5F
            control.ScaledSize = New SizeF(100, 100)
            control.ScaledLocation = New PointF(10, 10)
        End Sub
    End Module

    ' We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
    ' since this is nothing our code fix touches.
    Public Class ScalableControl
        Inherits System.Windows.Forms.Control

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaleFactor As Single = 1.0F

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaledSize As SizeF

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaledLocation As PointF
    End Class

End Namespace
"

    ' We are testing the analyzer with all versions of the .NET SDK from 6.0 on.
    Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
        Yield New Object() {ReferenceAssemblies.Net.Net60Windows}
        Yield New Object() {ReferenceAssemblies.Net.Net70Windows}
        Yield New Object() {ReferenceAssemblies.Net.Net80Windows}
        Yield New Object() {ReferenceAssemblies.Net.Net90Windows}
    End Function

    <Theory>
    <MemberData(NameOf(GetReferenceAssemblies))>
    Public Async Function VB_ControlPropertySerializationConfigurationAnalyzer(referenceAssemblies As ReferenceAssemblies) As Task
        Dim context = New VisualBasicAnalyzerTest(Of
            MissingPropertySerializationConfigurationAnalyzer,
            DefaultVerifier) With
            {
                .TestCode = CorrectCode,
                .ReferenceAssemblies = referenceAssemblies
            }

        context.TestState.OutputKind = OutputKind.WindowsApplication

        Await context.RunAsync()
    End Function

    <Theory>
    <MemberData(NameOf(GetReferenceAssemblies))>
    Public Async Function VB_MissingControlPropertySerializationConfigurationAnalyzer(referenceAssemblies As ReferenceAssemblies) As Task
        Dim context = New VisualBasicAnalyzerTest(Of
            MissingPropertySerializationConfigurationAnalyzer,
            DefaultVerifier) With
            {
                .TestCode = ProblematicCode,
                .ReferenceAssemblies = referenceAssemblies
            }

        context.TestState.OutputKind = OutputKind.WindowsApplication

        Await context.RunAsync()
    End Function

    <Theory>
    <MemberData(NameOf(GetReferenceAssemblies))>
    Public Async Function VB_AddDesignerSerializationVisibilityCodeFix(referenceAssemblies As ReferenceAssemblies) As Task
        Dim context = New VisualBasicCodeFixTest(Of
            MissingPropertySerializationConfigurationAnalyzer,
            AddDesignerSerializationVisibilityCodeFixProvider,
            DefaultVerifier) With
            {
                .TestCode = ProblematicCode,
                .FixedCode = FixedCode,
                .ReferenceAssemblies = referenceAssemblies,
                .NumberOfFixAllIterations = 2
            }

        context.TestState.OutputKind = OutputKind.WindowsApplication

        Await context.RunAsync()
    End Function
End Class
