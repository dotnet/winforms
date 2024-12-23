' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Net.Http

Namespace Microsoft.VisualBasic.MyServices.Internal

    Friend Class ProgressableStreamContent
        Inherits HttpContent

        Private ReadOnly _bufferSize As Integer
        Private ReadOnly _content As HttpContent
        Private ReadOnly _progress As Action(Of Long, Long)
        Public Sub New(content As HttpContent, progress As Action(Of Long, Long), Optional bufferSize As Integer = 4096)
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferSize)
            ArgumentNullException.ThrowIfNull(content)
            _content = content
            _progress = progress
            _bufferSize = bufferSize
            For Each header As KeyValuePair(Of String, IEnumerable(Of String)) In content.Headers
                Headers.TryAddWithoutValidation(header.Key, header.Value)
            Next
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then _content.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Protected Overrides Async Function SerializeToStreamAsync(stream As Stream, context As TransportContext) As Task
            Dim buffer(_bufferSize - 1) As Byte
            Dim size As Long
            Dim uploaded As Long = 0
            TryComputeLength(size)

            Using sinput As Stream = Await _content.ReadAsStreamAsync().ConfigureAwait(False)
                While True
                    Dim bytesRead As Long = Await sinput.ReadAsync(buffer).ConfigureAwait(False)
                    If bytesRead <= 0 Then Exit While
                    uploaded += bytesRead
                    _progress?.Invoke(uploaded, size)
                    Await stream.WriteAsync(buffer.AsMemory(0, CInt(bytesRead))).ConfigureAwait(False)
                    Await Stream.FlushAsync().ConfigureAwait(False)
                End While
            End Using
            Await Stream.FlushAsync().ConfigureAwait(False)
        End Function

        Protected Overrides Function TryComputeLength(ByRef length As Long) As Boolean
            length = _content.Headers.ContentLength.GetValueOrDefault()
            Return True
        End Function

    End Class
End Namespace
