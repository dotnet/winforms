Imports System.IO
Imports System.IO.Pipes
Imports System.Net
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class PipeListener
        Private _pipeServer As NamedPipeServerStream

        Public Sub New(pipeName As String, maxNumberOfServerInstances As Integer, transmissionMode As PipeTransmissionMode, options As PipeOptions)
            Task.Run(
            Sub()
                Try
                    ' Start the listener to begin listening for requests.
                    _pipeServer = New NamedPipeServerStream(
                    pipeName,
                    direction:=PipeDirection.Out,
                    maxNumberOfServerInstances:=1,
                    transmissionMode,
                    options)

                Catch ex As Exception
                    _pipeServer = Nothing
                End Try
            End Sub)
        End Sub

    End Class
End Namespace
