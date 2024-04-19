' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO.Pipes
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class SingleInstanceHelpersTests
        Private _resultArgs As String()

        Private Sub OnStartupNextInstanceMarshallingAdaptor(args As String())
            If args.Length = 1 Then
                _resultArgs = {"Hello"}
            End If
        End Sub

        <WinFormsFact>
        Public Sub TryCreatePipeServerTests()
            Dim pipeName As String = GetUniqueText()
            Dim pipeServer As NamedPipeServerStream = Nothing
            Assert.True(TryCreatePipeServer(pipeName, pipeServer))
            Assert.True(pipeServer.CanRead)
            Assert.False(pipeServer.CanSeek)
            Assert.False(pipeServer.CanWrite)
            Assert.Equal(PipeTransmissionMode.Byte, pipeServer.TransmissionMode)
            pipeServer.Close()
            pipeServer.Dispose()
        End Sub

        <WinFormsFact>
        Public Sub TryCreatePipeServerTwiceTests_Fail()
            Dim pipeName As String = GetUniqueText()
            Dim pipeServer As NamedPipeServerStream = Nothing
            Assert.True(TryCreatePipeServer(pipeName, pipeServer))
            Dim pipeServer1 As NamedPipeServerStream = Nothing
            Assert.False(TryCreatePipeServer(pipeName, pipeServer1))
            Assert.Null(pipeServer1)
            pipeServer.Close()
            pipeServer.Dispose()
        End Sub

        <WinFormsFact>
        Public Sub WaitForClientConnectionsAsyncTests()
            Dim pipeName As String = GetUniqueText()
            Dim pipeServer As NamedPipeServerStream = Nothing
            If TryCreatePipeServer(pipeName, pipeServer) Then

                Using pipeServer
                    Dim tokenSource As New CancellationTokenSource()
#Disable Warning BC42358 ' Call is not awaited.
                    WaitForClientConnectionsAsync(pipeServer, AddressOf OnStartupNextInstanceMarshallingAdaptor, cancellationToken:=tokenSource.Token)
#Enable Warning BC42358
                    Dim commandLine As String() = {"Hello"}
                    Dim awaitable As ConfiguredTaskAwaitable = SendSecondInstanceArgsAsync(pipeName, commandLine, cancellationToken:=tokenSource.Token).ConfigureAwait(False)
                    awaitable.GetAwaiter().GetResult()
                    Dim CancelToken As New CancellationToken
                    Dim buffer As Byte() = New Byte(commandLine.Length) {}

#Disable Warning CA2012 ' Use ValueTasks correctly
                    ' NOTE TO REVIEWS: Below needs to be fixed but I don't know how
                    Dim countTask As ValueTask(Of Integer) = pipeServer.ReadAsync(buffer.AsMemory(0, commandLine.Length))
#Enable Warning CA2012 ' Use ValueTasks correctly
                    Do
                        Thread.Sleep(5)
                    Loop Until _resultArgs IsNot Nothing AndAlso countTask.IsCompleted
                    Assert.True(countTask.IsCompletedSuccessfully)
                    Assert.Equal("Hello", _resultArgs(0))
                    tokenSource.Cancel()
                End Using
            End If

        End Sub

    End Class
End Namespace
