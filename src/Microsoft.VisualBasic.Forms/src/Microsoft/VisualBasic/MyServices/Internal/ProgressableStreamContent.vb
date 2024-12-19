' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Net.Http

Namespace Microsoft.VisualBasic.MyServices.Internal

    Friend Class ProgressableStreamContent
        Inherits HttpContent

        Private ReadOnly _content As HttpContent
        Private ReadOnly _bufferSize As Integer
        Private ReadOnly _progress As Action(Of Long, Long)

        Public Sub New(content As HttpContent, progress As Action(Of Long, Long), Optional bufferSize As Integer = 4096)
            _content = content
            _progress = progress
            _bufferSize = bufferSize
            For Each header As KeyValuePair(Of String, IEnumerable(Of String)) In content.Headers
                Headers.TryAddWithoutValidation(header.Key, header.Value)
            Next
        End Sub

        Protected Overrides Async Function SerializeToStreamAsync(stream As Stream, context As TransportContext) As Task
            Dim buffer(_bufferSize - 1) As Byte
            Dim size As Long
            Dim uploaded As Long = 0

            TryComputeLength(size)

            Using sInput As Stream = Await _content.ReadAsStreamAsync().ConfigureAwait(continueOnCapturedContext:=False)
                While True
                    Dim length As Integer = Await sInput.ReadAsync(buffer).ConfigureAwait(continueOnCapturedContext:=False)
                    If length <= 0 Then Exit While

                    Await stream.WriteAsync(buffer.AsMemory(start:=0, length)).ConfigureAwait(continueOnCapturedContext:=False)
                    uploaded += length
                    _progress?.Invoke(uploaded, size)
                End While
            End Using
        End Function


        Protected Overrides Function TryComputeLength(ByRef length As Long) As Boolean
            length = _content.Headers.ContentLength.GetValueOrDefault()
            Return True
        End Function

    End Class
End Namespace
