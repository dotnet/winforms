' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO

Namespace Microsoft.VisualBasic.Logging

    Partial Public Class FileLogTraceListener

        Friend NotInheritable Class ReferencedStream
            Implements IDisposable

            ' Used for synchronizing writing and reference counting
            Private ReadOnly _syncObject As New Object

            ' Indicates whether or not the object has been disposed
            Private _disposed As Boolean

            ' The number of FileLogTraceListeners using the stream
            Private _referenceCount As Integer

            ' The stream that does the writing
            Private _stream As StreamWriter

            ''' <summary>
            '''  Creates a new referenced <paramref name="Stream"/>.
            ''' </summary>
            ''' <param name="stream">The stream that does the actual writing.</param>
            Friend Sub New(stream As StreamWriter)
                _stream = stream
            End Sub

            ''' <summary>
            '''  Ensures <see cref="Stream"/> is closed at GC.
            ''' </summary>
            Protected Overrides Sub Finalize()
                ' Do not change this code. Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(False)
                MyBase.Finalize()
            End Sub

            ''' <summary>
            '''  The size of the log file.
            ''' </summary>
            ''' <value>The size.</value>
            Friend ReadOnly Property FileSize() As Long
                Get
                    Return _stream.BaseStream.Length
                End Get
            End Property

            ''' <summary>
            '''  Indicates whether or not the <see cref="Stream"/> is still in use by a FileLogTraceListener.
            ''' </summary>
            ''' <value>
            '''  <see langword="True"/> if the stream is being used,
            '''  otherwise <see langword="False"/>.
            ''' </value>
            Friend ReadOnly Property IsInUse() As Boolean
                Get
                    Return _stream IsNot Nothing
                End Get
            End Property

            ''' <summary>
            '''  Ensures the <see cref="Stream"/> is closed (flushed) no matter how we are closed.
            ''' </summary>
            ''' <param name="disposing">Indicates who called dispose.</param>
            Private Overloads Sub Dispose(disposing As Boolean)
                If disposing Then
                    If Not _disposed Then
                        _stream?.Close()
                        _disposed = True
                    End If
                End If
            End Sub

            ''' <summary>
            '''  Increments the reference count for the <see cref="Stream"/>.
            ''' </summary>
            Friend Sub AddReference()
                SyncLock _syncObject
                    _referenceCount += 1
                End SyncLock
            End Sub

            ''' <summary>
            '''  Decrements the reference count to the <see cref="Stream"/> and
            '''  closes the stream if the reference count is zero.
            ''' </summary>
            Friend Sub CloseStream()
                SyncLock _syncObject
                    Try
                        _referenceCount -= 1
                        _stream.Flush()
                        Debug.Assert(_referenceCount >= 0, "Ref count is below 0")
                    Finally
                        If _referenceCount <= 0 Then
                            _stream.Close()
                            _stream = Nothing
                        End If
                    End Try
                End SyncLock
            End Sub

            ''' <summary>
            '''  Flushes the <see cref="Stream"/>.
            ''' </summary>
            Friend Sub Flush()
                SyncLock _syncObject
                    _stream.Flush()
                End SyncLock
            End Sub

            ''' <summary>
            '''  Writes a message to the <see cref="Stream"/>.
            ''' </summary>
            ''' <param name="message">The message to write.</param>
            Friend Sub Write(message As String)
                SyncLock _syncObject
                    _stream.Write(message)
                End SyncLock
            End Sub

            ''' <summary>
            '''  Writes a message to the <see cref="Stream"/> as a line.
            ''' </summary>
            ''' <param name="message">The message to write.</param>
            Friend Sub WriteLine(message As String)
                SyncLock _syncObject
                    _stream.WriteLine(message)
                End SyncLock
            End Sub

            ''' <summary>
            '''  Standard implementation of IDisposable.
            ''' </summary>
            Public Overloads Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code. Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

        End Class 'ReferencedStream
    End Class
End Namespace
