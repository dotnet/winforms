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
                Return _highDpiMode
            End Get
            Set(value As HighDpiMode)
                _highDpiMode = value
            End Set
        End Property

        Protected Overrides Function OnInitialize(commandLineArgs As ReadOnlyCollection(Of String)) As Boolean
            Dim eventArgs = New ApplyHighDpiModeEventArgs(
                If(
                    _highDpiMode Is Nothing,
                    HighDpiMode.SystemAware,
                    _highDpiMode))

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

        Public Property HighDpiMode

    End Class
#End If

End Namespace
