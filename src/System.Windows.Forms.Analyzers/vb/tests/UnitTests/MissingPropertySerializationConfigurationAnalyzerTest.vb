Imports System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration
Imports System.Windows.Forms.VisualBasic.CodeFixes.AddDesignerSerializationVisibility
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.CodeAnalysis.VisualBasic.Testing
Imports Xunit

Public Class ControlPropertySerializationDiagnosticAnalyzerTest

    Private Const ProblematicCode As String =
"Imports System.Drawing
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
        Inherits System.Windows.Forms.Control

        private _scaledSize as SizeF

        Public Property [|ScaleFactor|] As Single = 1.0F

        ''' <Summary>
        '''  Sets or gets the scaled size of some foo bar thing.
        ''' </Summary>
        <System.ComponentModel.Description(""Sets or gets the scaled size of some foo bar thing."")>
        Public Property [|ScaledSize|] As SizeF
            Get
                Return _scaledSize
            End Get
            Set(value As SizeF)
                _scaledSize = value
            End Set
        End Property

        ''' <Summary>
        '''  Sets or gets the scaled location of some foo bar thing.
        ''' </Summary>
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
        Inherits System.Windows.Forms.Control

        private _scaledSize as SizeF

        <DefaultValue(1.0F)>
        Public Property ScaleFactor As Single = 1.0F

        ''' <Summary>
        '''  Sets or gets the scaled size of some foo bar thing.
        ''' </Summary>
        <System.ComponentModel.Description(""Sets or gets the scaled size of some foo bar thing."")>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaledSize As SizeF
            Get
                Return _scaledSize
            End Get
            Set(value As SizeF)
                _scaledSize = value
            End Set
        End Property

        ''' <Summary>
        '''  Sets or gets the scaled location of some foo bar thing.
        ''' </Summary>
        Public Property ScaledLocation As PointF

        Private Function ShouldSerializeScaledLocation as Boolean
            Return False
        End Function
    End Class
End Namespace
"

    Private Const FixedCode As String =
"Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

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
        Inherits System.Windows.Forms.Control

        private _scaledSize as SizeF

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaleFactor As Single = 1.0F

        ''' <Summary>
        '''  Sets or gets the scaled size of some foo bar thing.
        ''' </Summary>
        <System.ComponentModel.Description(""Sets or gets the scaled size of some foo bar thing."")>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ScaledSize As SizeF
            Get
                Return _scaledSize
            End Get
            Set(value As SizeF)
                _scaledSize = value
            End Set
        End Property

        ''' <Summary>
        '''  Sets or gets the scaled location of some foo bar thing.
        ''' </Summary>
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
    Public Async Function VB_MissingControlPropertySerializationConfigurationAnalyzer(referenceAssemblies As ReferenceAssemblies) As Task
        Dim context = New VisualBasicAnalyzerTest(Of
            MissingPropertySerializationConfigurationAnalyzer,
            DefaultVerifier) With
            {
                .TestCode = ProblematicCode,
                .ReferenceAssemblies = referenceAssemblies
            }

        context.TestState.OutputKind = OutputKind.WindowsApplication

        Await context.RunAsync().ConfigureAwait(continueOnCapturedContext:=True)
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

        Await context.RunAsync().ConfigureAwait(continueOnCapturedContext:=True)
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

        Await context.RunAsync().ConfigureAwait(continueOnCapturedContext:=True)
    End Function
End Class
