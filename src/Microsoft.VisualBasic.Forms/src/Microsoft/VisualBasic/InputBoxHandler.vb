' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms

Namespace Microsoft.VisualBasic

    Partial Friend Module _Interaction

        Friend NotInheritable Class InputBoxHandler
            Private ReadOnly _defaultResponse As String
            Private ReadOnly _parentWindow As IWin32Window
            Private ReadOnly _prompt As String
            Private ReadOnly _title As String
            Private ReadOnly _xPos As Integer
            Private ReadOnly _yPos As Integer
            Private _exception As Exception
            Private _result As String

            Public Sub New(Prompt As String, Title As String, DefaultResponse As String, XPos As Integer, YPos As Integer, ParentWindow As IWin32Window)
                _prompt = Prompt
                _title = Title
                _defaultResponse = DefaultResponse
                _xPos = XPos
                _yPos = YPos
                _parentWindow = ParentWindow
            End Sub

            Friend ReadOnly Property Exception As Exception
                Get
                    Return _exception
                End Get
            End Property

            Public ReadOnly Property Result() As String
                Get
                    Return _result
                End Get
            End Property

            Public Sub StartHere()
                Try
                    _result = InternalInputBox(_prompt, _title, _defaultResponse, _xPos, _yPos, _parentWindow)
                Catch ex As Exception
                    _exception = ex
                End Try
            End Sub

        End Class
    End Module
End Namespace
