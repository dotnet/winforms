Option Strict On
Option Explicit On

Imports System.Windows.Forms

'This constant indicates whether the Application Framework is in use.
#Const APPLICATION_FRAMEWORK = True

#If APPLICATION_FRAMEWORK Then

Imports System.Collections.ObjectModel

Namespace My

#If NET5_0 Then

    Partial Friend Class MyApplication

        Public Event ApplyHighDpiMode(sender As Object, e As ApplyHighDpiModeEventArgs)

        Private _highDpiMode As HighDpiMode?

        Friend Shadows Property HighDpiMode As HighDpiMode
            Get
                If _highDpiMode Is Nothing Then
                    Return Application.HighDpiMode
                End If
                Return _highDpiMode.Value
            End Get
            Set(value As HighDpiMode)
                _highDpiMode = value
            End Set
        End Property

        'IMPORTANT:
        'If this method causes an compilation error after you've unchecked 'Application Framework' 
        'in the project properties, go to the top of this file and change the value to 'False' in this line:
        '#Const APPLICATION_FRAMEWORK = False
        Protected Overrides Function OnInitialize(commandLineArgs As ReadOnlyCollection(Of String)) As Boolean
            Dim eventArgs = New ApplyHighDpiModeEventArgs(
                If(
                    _highDpiMode Is Nothing,
                    HighDpiMode.SystemAware,
                    _highDpiMode.Value))

            RaiseEvent ApplyHighDpiMode(Me, eventArgs)

            Dim test = System.Windows.Forms.Application.SetHighDpiMode(eventArgs.HighDpiMode)
            Return MyBase.OnInitialize(commandLineArgs)
        End Function
    End Class

    Public Class ApplyHighDpiModeEventArgs
        Inherits EventArgs

        Sub New(highDpiMode As HighDpiMode)
            Me.HighDpiMode = highDpiMode
        End Sub

        Public Property HighDpiMode As HighDpiMode

    End Class

End Namespace
#End If

#Else

' For more about using WinForms without the Application Framework 
' see: https://aka.ms/visualbasic-appframework-net5
Friend Module Program
    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1)
    End Sub
End Module

#End If
