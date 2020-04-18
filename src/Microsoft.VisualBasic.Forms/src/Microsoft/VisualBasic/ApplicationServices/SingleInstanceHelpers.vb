' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.IO.Pipes
Imports System.Runtime.Serialization
Imports System.Threading

Namespace Microsoft.VisualBasic.ApplicationServices

    Friend Module SingleInstanceHelpers
        Private Const NamedPipeOptions As PipeOptions = PipeOptions.Asynchronous Or PipeOptions.CurrentUserOnly

        Friend Function CreatePipeServer(pipeName As String) As NamedPipeServerStream
            Try
                Return New NamedPipeServerStream(
                        pipeName:=pipeName,
                        direction:=PipeDirection.In,
                        maxNumberOfServerInstances:=1,
                        transmissionMode:=PipeTransmissionMode.Byte,
                        options:=NamedPipeOptions)
            Catch ex As IOException
                Return Nothing
            End Try
        End Function

        Friend Async Function WaitForClientConnectionsAsync(pipeServer As NamedPipeServerStream, callback As Action(Of String()), cancellationToken As CancellationToken) As Task
            While True
                cancellationToken.ThrowIfCancellationRequested()
                Try
                    Await pipeServer.WaitForConnectionAsync(cancellationToken)
                    Try
                        Dim args = Await ReadArgsAsync(pipeServer, cancellationToken)
                        If args IsNot Nothing Then
                            callback(args)
                        End If
                    Finally
                        pipeServer.Disconnect()
                    End Try
                Catch ex As IOException
                Catch ex As ObjectDisposedException
                    Return
                End Try
            End While
        End Function

        Friend Function SendSecondInstanceArgs(pipeName As String, timeout As Integer, args As String()) As Boolean
            Try
                Using pipeClient As New NamedPipeClientStream(
                            serverName:=".",
                            pipeName:=pipeName,
                            direction:=PipeDirection.Out,
                            options:=NamedPipeOptions)
                    Dim tokenSource = New CancellationTokenSource()
                    tokenSource.CancelAfter(timeout)
                    Try
                        Dim task = WriteArgsAsync(pipeClient, args, tokenSource.Token)
                        task.GetAwaiter().GetResult()
                        Return True
                    Catch ex As TaskCanceledException
                    End Try
                End Using
            Catch ex As IOException
            End Try
            Return False
        End Function

        Private Async Function ReadArgsAsync(pipeServer As NamedPipeServerStream, cancellationToken As CancellationToken) As Task(Of String())
            Const nBuffer = 1024
            Dim buffer = New Byte(nBuffer - 1) {}
            Using stream As New MemoryStream
                While True
                    Dim nRead = Await pipeServer.ReadAsync(buffer, 0, nBuffer, cancellationToken)
                    If nRead = 0 Then
                        Exit While
                    End If
                    stream.Write(buffer, 0, nRead)
                End While
                stream.Seek(0, SeekOrigin.Begin)
                Dim serializer = New DataContractSerializer(GetType(String()))
                Try
                    Return DirectCast(serializer.ReadObject(stream), String())
                Catch ex As Exception
                    Return Nothing
                End Try
            End Using
        End Function

        Private Async Function WriteArgsAsync(pipeClient As NamedPipeClientStream, args As String(), cancellationToken As CancellationToken) As Task
            Await pipeClient.ConnectAsync(cancellationToken)
            Dim content As Byte()
            Using stream As New MemoryStream
                Dim serializer = New DataContractSerializer(GetType(String()))
                serializer.WriteObject(stream, args)
                content = stream.ToArray()
            End Using
            Await pipeClient.WriteAsync(content, 0, content.Length, cancellationToken)
        End Function
    End Module

End Namespace
