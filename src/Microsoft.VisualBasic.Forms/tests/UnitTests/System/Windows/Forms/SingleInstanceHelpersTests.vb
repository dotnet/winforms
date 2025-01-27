' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO.Pipes
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class SingleInstanceHelpersTests
        Private _resultArgs As String()

        <WinFormsFact>
        Public Sub TryCreatePipeServerTests()
            Dim pipeName As String = GetUniqueText()
            Dim pipeServer As NamedPipeServerStream = Nothing
            TryCreatePipeServer(pipeName, pipeServer).Should.BeTrue()
            Using pipeServer
                pipeServer.CanRead.Should.BeTrue()
                pipeServer.CanSeek.Should.BeFalse()
                pipeServer.CanWrite.Should.BeFalse()
                pipeServer.TransmissionMode.Should.Be(PipeTransmissionMode.Byte)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub TryCreatePipeServerTwiceTests_Fail()
            Dim pipeName As String = GetUniqueText()
            Dim pipeServer As NamedPipeServerStream = Nothing
            TryCreatePipeServer(pipeName, pipeServer).Should.BeTrue()
            Using pipeServer
                Dim pipeServer1 As NamedPipeServerStream = Nothing
                TryCreatePipeServer(pipeName, pipeServer1).Should.BeFalse()
                pipeServer1.Should.BeNull()
            End Using
        End Sub

        <WinFormsFact>
        Public Async Function WaitForClientConnectionsAsyncTests() As Task
            Dim pipeName As String = GetUniqueText()
            Dim pipeServer As NamedPipeServerStream = Nothing

            If TryCreatePipeServer(pipeName, pipeServer) Then

                Using pipeServer
                    Dim tokenSource As New CancellationTokenSource()
                    Dim commandLine As String() = {"Hello"}
                    Dim clientConnection As Task = WaitForClientConnectionsAsync(
                        pipeServer,
                        callback:=Sub(args As String())
                                      If args.Length = 1 Then
                                          _resultArgs = commandLine
                                      End If
                                  End Sub,
                        cancellationToken:=tokenSource.Token)

                    Dim awaitable As ConfiguredTaskAwaitable = SendSecondInstanceArgsAsync(
                        pipeName,
                        args:=commandLine,
                        cancellationToken:=tokenSource.Token) _
                            .ConfigureAwait(continueOnCapturedContext:=False)

                    awaitable.GetAwaiter().GetResult()
                    Dim CancelToken As New CancellationToken
                    Dim buffer As Byte() = New Byte(commandLine.Length) {}
                    Dim count As Integer =
                        Await pipeServer.ReadAsync(buffer:=buffer.AsMemory(start:=0, length:=commandLine.Length)) _
                            .ConfigureAwait(continueOnCapturedContext:=True)

                    ' Ensure the result is set
                    Do
                        Await Task.Delay(5).ConfigureAwait(continueOnCapturedContext:=True)
                    Loop Until _resultArgs IsNot Nothing
                    _resultArgs(0).Should.Be("Hello")
                    Await tokenSource.CancelAsync().ConfigureAwait(continueOnCapturedContext:=True)
                End Using
            End If
        End Function

    End Class
End Namespace
