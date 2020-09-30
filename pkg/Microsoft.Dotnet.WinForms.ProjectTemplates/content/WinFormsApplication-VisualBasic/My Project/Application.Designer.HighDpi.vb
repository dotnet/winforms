Option Strict On
Option Explicit On

'This constant indicates whether the Application Framework is in use.
#Const APPLICATION_FRAMEWORK = True

#If APPLICATION_FRAMEWORK Then

#If NET5_0 And Not NET6_0 Then

Imports System.Collections.ObjectModel

Namespace My

    Partial Friend Class MyApplication

        Public Event ApplyHighDpiMode(sender As Object, e As ApplyHighDpiModeEventArgs)

        Private _highDpiMode As HighDpiMode?

        Friend Shadows Property HighDpiMode As HighDpiMode
            Get
                Return If(
                    _highDpiMode Is Nothing,
                    Application.HighDpiMode,
                    _highDpiMode.Value)
            End Get
            Set(value As HighDpiMode)
                _highDpiMode = value
            End Set
        End Property

        ' IMPORTANT:
        ' If this method causes an compilation error after you've unchecked 'Application Framework' 
        ' in the project properties, go to the top of this file and change the value to 'False' in this line:
        ' #Const APPLICATION_FRAMEWORK = False

        ' For more about using WinForms without the Application Framework 
        ' see: https://aka.ms/visualbasic-appframework-net5
        Protected Overrides Function OnInitialize(commandLineArgs As ReadOnlyCollection(Of String)) As Boolean
            Dim eventArgs = New ApplyHighDpiModeEventArgs(
                If(
                    _highDpiMode Is Nothing,
                    HighDpiMode.SystemAware,
                    _highDpiMode.Value))

            RaiseEvent ApplyHighDpiMode(Me, eventArgs)

            Windows.Forms.Application.SetHighDpiMode(eventArgs.HighDpiMode)

            Return MyBase.OnInitialize(commandLineArgs)
        End Function
    End Class

    Public Class ApplyHighDpiModeEventArgs
        Inherits EventArgs

        Public Sub New(highDpiMode As HighDpiMode)
            Me.HighDpiMode = highDpiMode
        End Sub

        Public Property HighDpiMode As HighDpiMode

    End Class

End Namespace

#End If ' #If NET5_0 And Not NET6_0
#End If ' #If APPLICATION_FRAMEWORK
