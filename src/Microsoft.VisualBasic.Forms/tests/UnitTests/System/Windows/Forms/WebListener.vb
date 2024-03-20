﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class WebListener
        Private ReadOnly _downloadFileUrlPrefix As String
        Private ReadOnly _fileSize As Integer

        Private _password As String
        Private _userName As String

        Public ReadOnly Address As String

        ''' <summary>
        '''  The name of the function that creates the server is uses to establish the file to be downloaded
        ''' </summary>
        ''' <param name="fileSize">Is used to create the file name and the size of download</param>
        ''' <param name="memberName">Used to establish the file path to be downloaded</param>
        Public Sub New(fileSize As Integer, <CallerMemberName> Optional memberName As String = Nothing)
            _fileSize = fileSize
            _downloadFileUrlPrefix = $"http://127.0.0.1:8080/{memberName}/"
            Address = $"{_downloadFileUrlPrefix}T{fileSize}"
        End Sub

        ''' <summary>
        '''  Used for authenticated download
        ''' </summary>
        ''' <param name="fileSize">Passed to Me.New</param>
        ''' <param name="userName">Name to match for authorization</param>
        ''' <param name="password">Password to match for authorization</param>
        ''' <param name="memberName">Passed to Me.New</param>
        Public Sub New(fileSize As Integer, userName As String, password As String, <CallerMemberName> Optional memberName As String = Nothing)
            Me.New(fileSize, memberName)
            _userName = userName
            _password = password
        End Sub

        Friend Function ProcessRequests() As HttpListener
            ' Create a listener and add the prefixes.
            Dim listener As New HttpListener()

            listener.Prefixes.Add(_downloadFileUrlPrefix)
            If _userName IsNot Nothing OrElse _password IsNot Nothing Then
                listener.AuthenticationSchemes = AuthenticationSchemes.Basic
            End If
            listener.Start()
            Task.Run(
                Sub()
                    ' Start the listener to begin listening for requests.
                    Dim response As HttpListenerResponse = Nothing

                    Try
                        ' Note: GetContext blocks while waiting for a request.
                        Dim context As HttpListenerContext = listener.GetContext()
                        ' Create the response.
                        response = context.Response
                        Dim identity As HttpListenerBasicIdentity = CType(context.User?.Identity, HttpListenerBasicIdentity)
                        If context.User?.Identity.IsAuthenticated Then
                            If String.IsNullOrWhiteSpace(identity.Name) OrElse identity.Name <> _userName OrElse String.IsNullOrWhiteSpace(identity.Password) OrElse identity.Password <> _password Then
                                response.StatusCode = HttpStatusCode.Unauthorized
                                Exit Try
                            End If
                        End If
                        Dim responseString As String = Strings.StrDup(_fileSize, "A")
                        ' Simulate network traffic
                        Threading.Thread.Sleep(20)
                        Dim buffer() As Byte = Text.Encoding.UTF8.GetBytes(responseString)
                        response.ContentLength64 = buffer.Length
                        Dim output As Stream = response.OutputStream
                        output.Write(buffer, 0, buffer.Length)
                    Finally
                        response?.Close()
                    End Try
                End Sub)
            Return listener
        End Function

    End Class
End Namespace
