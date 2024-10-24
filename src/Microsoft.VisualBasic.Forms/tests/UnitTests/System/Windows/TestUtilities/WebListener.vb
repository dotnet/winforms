' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class WebListener
        Private ReadOnly _downloadFileUrlPrefix As String
        Private ReadOnly _fileSize As Integer

        Private _password As String
        Private _userName As String

        ''' <summary>
        '''  The name of the function that creates the server is uses to establish the file to be downloaded.
        ''' </summary>
        ''' <param name="fileSize">Is used to create the file name and the size of download.</param>
        ''' <param name="memberName">Used to establish the file path to be downloaded.</param>
        Public Sub New(fileSize As Integer, <CallerMemberName> Optional memberName As String = Nothing)
            _fileSize = fileSize
            _downloadFileUrlPrefix = $"http://127.0.0.1:8080/{memberName}/"
            Address = $"{_downloadFileUrlPrefix}T{fileSize}"
        End Sub

        ''' <summary>
        '''  Used for authenticated download.
        ''' </summary>
        ''' <param name="fileSize">Passed to Me.New.</param>
        ''' <param name="userName">Name to match for authorization.</param>
        ''' <param name="password">Password to match for authorization.</param>
        ''' <param name="memberName">Passed to Me.New.</param>
        Public Sub New(
            fileSize As Integer,
            userName As String,
            password As String,
            <CallerMemberName> Optional memberName As String = Nothing)

            Me.New(fileSize, memberName)
            _userName = userName
            _password = password
        End Sub

        Public ReadOnly Property Address As String

        Friend Function ProcessRequests() As HttpListener
            ' Create a listener and add the prefixes.
            Dim listener As New HttpListener()

            listener.Prefixes.Add(_downloadFileUrlPrefix)
            If _userName IsNot Nothing OrElse _password IsNot Nothing Then
                listener.AuthenticationSchemes = AuthenticationSchemes.Basic
            End If
            listener.Start()
            Dim action As Action =
                Sub()
                    ' Start the listener to begin listening for requests.
                    Dim response As HttpListenerResponse = Nothing
                    Try
                        ' Note: GetContext blocks while waiting for a request.
                        Dim context As HttpListenerContext = listener.GetContext()
                        ' Create the response.
                        response = context.Response

                        If context.User?.Identity.IsAuthenticated Then
                            Dim identity As HttpListenerBasicIdentity =
                                              CType(context.User?.Identity, HttpListenerBasicIdentity)
                            If String.IsNullOrWhiteSpace(identity.Name) _
                                OrElse identity.Name <> _userName _
                                OrElse String.IsNullOrWhiteSpace(identity.Password) _
                                OrElse identity.Password <> _password Then

                                response.StatusCode = HttpStatusCode.Unauthorized
                                Exit Try
                            End If
                        End If
                        ' Simulate network traffic
                        Thread.Sleep(millisecondsTimeout:=20)
                        Dim responseString As String = Strings.StrDup(_fileSize, "A")
                        Dim buffer() As Byte = Text.Encoding.UTF8.GetBytes(responseString)
                        response.ContentLength64 = buffer.Length
                        Using output As Stream = response.OutputStream
                            output.Write(buffer, offset:=0, count:=buffer.Length)
                        End Using
                    Finally
                        response?.Close()
                        response = Nothing
                    End Try
                End Sub

            Task.Run(action)
            Return listener
        End Function

    End Class
End Namespace
