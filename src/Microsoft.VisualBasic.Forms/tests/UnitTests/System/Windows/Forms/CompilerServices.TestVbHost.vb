' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class CompilerServicesTests
        Private Shared ReadOnly s_title As String = GetUniqueText()

        Private NotInheritable Class TestVbHost
            Implements IVbHost, IDisposable

            Private _disposedValue As Boolean
            Private ReadOnly _testControl As New Control()

            Public Function GetParentWindow() As IWin32Window Implements IVbHost.GetParentWindow
                Return _testControl
            End Function

            Public Function GetWindowTitle() As String Implements IVbHost.GetWindowTitle
                Return s_title
            End Function

            Private Sub Dispose(disposing As Boolean)
                If Not _disposedValue Then
                    If disposing Then
                        _testControl.Dispose()
                    End If
                    _disposedValue = True
                End If
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                Dispose(disposing:=True)
                GC.SuppressFinalize(Me)
            End Sub

        End Class
    End Class
End Namespace
