' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class WebListener

        Friend Shared Function ProcessRequests(prefixes() As String) As HttpListener

            If Not HttpListener.IsSupported Then
                Console.WriteLine("Windows XP SP2, Server 2003, or higher is required to use the HttpListener class.")
            End If

            ' URI prefixes are required,
            If prefixes Is Nothing OrElse prefixes.Length = 0 Then
                Throw New ArgumentException("prefixes")
            End If

            ' Create a listener and add the prefixes.
            Dim listener As New HttpListener()
            For Each s As String In prefixes
                listener.Prefixes.Add(s)
            Next
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
                            Dim rawUrl As String = context.Request.RawUrl.Split("/").Last.Substring(1)
                            Dim size As Integer = Integer.Parse(rawUrl)
                            Dim responseString As String = Strings.StrDup(size, "A")
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
