' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class WebListener
        Private ReadOnly _downloadFileUrlPrefix As String
        Private ReadOnly _fileSize As Integer
        Public ReadOnly Address As String

        Public Sub New(fileSize As Integer, <CallerMemberName> Optional memberName As String = Nothing)
            _fileSize = fileSize
            _downloadFileUrlPrefix = $"http://127.0.0.1:8080/{memberName}/"
            Address = $"{_downloadFileUrlPrefix}T{fileSize}"
        End Sub

        Friend Function ProcessRequests() As HttpListener

            If Not HttpListener.IsSupported Then
                Console.WriteLine("Windows XP SP2, Server 2003, or higher is required to use the HttpListener class.")
            End If

            ' Create a listener and add the prefixes.
            Dim listener As New HttpListener()
            listener.Prefixes.Add(_downloadFileUrlPrefix)
            listener.Start()
            Task.Run(
                Sub()
                    Try
                        ' Start the listener to begin listening for requests.
                        Debug.WriteLine("Listening...")

                        Dim response As HttpListenerResponse = Nothing
                        Try
                            ' Note: GetContext blocks while waiting for a request.
                            Dim context As HttpListenerContext = listener.GetContext()

                            ' Create the response.
                            response = context.Response
                            Dim responseString As String = Strings.StrDup(_fileSize, "A")
                            ' Simulate network traffic
                            Threading.Thread.Sleep(20)
                            Dim buffer() As Byte = Text.Encoding.UTF8.GetBytes(responseString)
                            response.ContentLength64 = buffer.Length
                            Dim output As Stream = response.OutputStream
                            output.Write(buffer, 0, buffer.Length)
                        Catch ex As HttpListenerException
                            Debug.WriteLine(ex.Message)
                        Finally
                            response?.Close()
                        End Try
                    Catch ex As HttpListenerException
                        Debug.WriteLine(ex.Message)
                    End Try
                End Sub)
            Return listener
        End Function

    End Class

End Namespace
