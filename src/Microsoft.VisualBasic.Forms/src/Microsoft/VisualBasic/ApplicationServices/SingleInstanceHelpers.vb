' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.IO.Pipes
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Threading

Namespace Microsoft.VisualBasic.ApplicationServices

    Friend Module SingleInstanceHelpers
        Private Const NamedPipeOptions As PipeOptions = PipeOptions.Asynchronous Or PipeOptions.CurrentUserOnly

        Private Async Function ReadArgsAsync(
            pipeServer As NamedPipeServerStream,
            cancellationToken As CancellationToken) As Task(Of String())

            Const bufferLength As Integer = 1024

            Using stream As New MemoryStream
                While True
                    Dim buffer As Byte() = New Byte(bufferLength - 1) {}
                    Dim bytesRead As Integer =
                        Await pipeServer.ReadAsync(
                            buffer:=buffer.AsMemory(start:=0, length:=bufferLength),
                            cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
                    If bytesRead = 0 Then
                        Exit While
                    End If
                    Await stream.WriteAsync(
                        buffer:=buffer.AsMemory(start:=0, length:=bytesRead),
                        cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
                End While
                stream.Seek(0, SeekOrigin.Begin)
                Dim serializer As New DataContractSerializer(GetType(String()))
                Try
                    Return DirectCast(serializer.ReadObject(stream), String())
                Catch ex As Exception
                    Return Nothing
                End Try
            End Using
        End Function

        Private Async Function WriteArgsAsync(
            pipeClient As NamedPipeClientStream,
            args As String(),
            cancellationToken As CancellationToken) As Task

            Dim content As Byte()
            Using stream As New MemoryStream
                Dim serializer As New DataContractSerializer(GetType(String()))
                serializer.WriteObject(stream, args)
                content = stream.ToArray()
            End Using
            Await pipeClient.WriteAsync(
                buffer:=content.AsMemory(start:=0, length:=content.Length),
                cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        Friend Async Function SendSecondInstanceArgsAsync(
            pipeName As String,
            args As String(),
            cancellationToken As CancellationToken) As Task

            Using pipeClient As New NamedPipeClientStream(
                serverName:=".",
                pipeName:=pipeName,
                direction:=PipeDirection.Out,
                options:=NamedPipeOptions)

                Await pipeClient.ConnectAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
                Await WriteArgsAsync(pipeClient, args, cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
            End Using
        End Function

        Friend Function TryCreatePipeServer(
            pipeName As String,
            <Out> ByRef pipeServer As NamedPipeServerStream) As Boolean

            Try
                pipeServer = New NamedPipeServerStream(
                    pipeName:=pipeName,
                    direction:=PipeDirection.In,
                    maxNumberOfServerInstances:=1,
                    transmissionMode:=PipeTransmissionMode.Byte,
                    options:=NamedPipeOptions)
                Return True
            Catch ex As Exception
                pipeServer = Nothing
                Return False
            End Try
        End Function

        Friend Async Function WaitForClientConnectionsAsync(
            pipeServer As NamedPipeServerStream,
            callback As Action(Of String()),
            cancellationToken As CancellationToken) As Task

            While True
                cancellationToken.ThrowIfCancellationRequested()
                Await pipeServer.WaitForConnectionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
                Try
                    Dim args() As String =
                        Await ReadArgsAsync(pipeServer, cancellationToken).ConfigureAwait(continueOnCapturedContext:=False)
                    If args IsNot Nothing Then
                        callback(args)
                    End If
                Finally
                    pipeServer.Disconnect()
                End Try
            End While
        End Function

    End Module
End Namespace
