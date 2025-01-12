' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class WebListener
        Private Const BufferSize As Integer = 4096
        Private ReadOnly _address As String
        Private ReadOnly _fileName As String
        Private ReadOnly _fileSize As Integer
        Private ReadOnly _fileUrlPrefix As String
        Private ReadOnly _password As String
        Private ReadOnly _userName As String

        ''' <summary>
        '''  The name of the function that creates the server is used to establish the file to be downloaded.
        ''' </summary>
        ''' <param name="fileSize">Is used to create the file name and the size of download.</param>
        ''' <param name="memberName">Used to establish the file path to be downloaded.</param>
        Public Sub New(fileSize As Integer, <CallerMemberName> Optional memberName As String = Nothing)
            Debug.Assert(fileSize > 0)
            _fileName = $"{[Enum].GetName(GetType(FileSizes), fileSize)}.zip"
            _fileSize = fileSize
            _fileUrlPrefix = $"http://127.0.0.1:8080/{memberName}/"
            _address = $"{_fileUrlPrefix}{_fileName}"
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
            Get
                Return _address
            End Get
        End Property

        Public ReadOnly Property FileSize As Long
            Get
                Return _fileSize
            End Get
        End Property

        ''' <summary>
        '''  This server will save a <see langword="String"/> when something in the stream doesn't
        '''  match what is expected. These will never be visible to the user.
        ''' </summary>
        ''' <value>A <see langword="String"/> with a description of the issue or <see langword="Nothing"/></value>
        Public Property ServerFaultMessage As String

        Private Shared Function GetBoundary(contentType As String) As String
            Dim elements As String() = contentType.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
            Dim element As String = elements.FirstOrDefault(Function(e) e.Trim().StartsWith("boundary=", StringComparison.OrdinalIgnoreCase))
            Return If(element IsNot Nothing, element.Substring(startIndex:=element.IndexOf("="c) + 1).Trim().Trim(""""c), String.Empty)
        End Function

        Private Shared Function GetContentDispositionHeader(
                lines As List(Of String),
                ByRef headerParts() As String) As Integer
            For dispositionIndex As Integer = 0 To lines.Count - 1
                Dim line As String = lines(dispositionIndex)
                If line.Contains("Content-Disposition", StringComparison.InvariantCultureIgnoreCase) Then
                    headerParts = line.Split({":"c}, count:=2)
                    Dim result As Boolean = headerParts.Length = 2 _
                        AndAlso headerParts(0).Trim().Equals(value:="Content-Disposition",
                        comparisonType:=StringComparison.OrdinalIgnoreCase)
                    Return dispositionIndex
                End If
            Next
            Return -1
        End Function

        Private Shared Function GetDataLength(lines As List(Of String), dispositionIndex As Integer) As Integer
            For Each line As String In lines
                If line.Substring(0, 1) = vbNullChar Then
                    Return line.Length
                End If
            Next
            Return 0
        End Function

        Private Shared Function GetFilename(headerParts() As String) As String
            Dim value As String = ""
            Dim line As String = headerParts(1)
            Dim startIndex As Integer = line.IndexOf("filename=""", StringComparison.InvariantCultureIgnoreCase)
            If startIndex > -1 Then
                line = line.Substring(startIndex + 10)
                Dim length As Integer = line.IndexOf(""""c)
                value = line.Substring(0, length)
            End If

            Return value
        End Function

        Friend Function ProcessRequests() As HttpListener
            ' Create a listener and add the prefixes.
            Dim listener As New HttpListener()

            If _fileUrlPrefix.Contains("8080") Then

                listener.Prefixes.Add(_fileUrlPrefix)
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
                            If listener.Prefixes(0).Contains("UploadFile") Then
                                Dim request As HttpListenerRequest = context.Request
                                If request.HttpMethod.Equals("Post", StringComparison.OrdinalIgnoreCase) _
                                    AndAlso request.HasEntityBody Then

                                    Dim formData As Dictionary(Of String, String) = GetMultipartFormData(request)

                                    Using bodyStream As Stream = request.InputStream
                                        Using reader As New StreamReader(
                                            stream:=bodyStream,
                                            encoding:=request.ContentEncoding,
                                            detectEncodingFromByteOrderMarks:=True,
                                            BufferSize)
                                        End Using
                                    End Using
                                    Try
                                        Dim dataLength As String = formData(NameOf(dataLength))
                                        If _fileSize.ToString <> dataLength Then
                                            ServerFaultMessage = $"File size mismatch, expected {_fileSize} actual {dataLength}"
                                        Else
                                            Dim fileName As String = formData("filename")
                                            If Not fileName.Equals(_fileName, StringComparison.OrdinalIgnoreCase) Then
                                                ServerFaultMessage = $"Filename incorrect, expected '{_fileName}', actual {fileName}"
                                            End If
                                        End If
                                    Catch ex As Exception
                                        ' Ignore is case upload is cancelled
                                    End Try
                                End If
                                response.StatusCode = 200
                            Else
                                Dim responseString As String = Strings.StrDup(_fileSize, "A")
                                Dim buffer() As Byte = Text.Encoding.UTF8.GetBytes(responseString)
                                response.ContentLength64 = buffer.Length
                                Using output As Stream = response.OutputStream
                                    Try
                                        output.Write(buffer, offset:=0, count:=buffer.Length)
                                    Catch ex As Exception
                                        ' ignore it will be handled elsewhere
                                    End Try
                                End Using
                            End If
                        Finally
                            Try
                                response?.Close()
                            Catch ex As Exception

                            End Try
                            response = Nothing
                        End Try
                    End Sub

                Task.Run(action)
            End If
            Return listener
        End Function

        ''' <summary>
        '''  Parses a <see cref="HttpListenerRequest"/> and gets the fileName of the uploaded file
        '''  and the lenght of the data file in bytes
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns>
        '''  A <see cref="Dictionary(Of String, String)"/> that contains the filename and lenght of the data file.
        ''' </returns>
        Public Function GetMultipartFormData(request As HttpListenerRequest) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)

            If request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) Then
                Dim boundary As String = GetBoundary(request.ContentType)
                Using reader As New StreamReader(request.InputStream, request.ContentEncoding)
                    Try
                        Dim content As String = reader.ReadToEnd()
                        Dim parts As String() = content.Split(boundary, StringSplitOptions.RemoveEmptyEntries)

                        For Each part As String In parts
                            If part.Trim() <> "--" Then
                                Dim separator As String() = New String() {Environment.NewLine}
                                Dim lines As List(Of String) = part.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList
                                If lines.Count > 2 Then
                                    Dim headerParts As String() = Nothing
                                    Dim dispositionIndex As Integer = GetContentDispositionHeader(lines, headerParts)

                                    If dispositionIndex > -1 Then
                                        result.Add("filename", GetFilename(headerParts))
                                        If lines.Count > dispositionIndex + 1 Then
                                            result.Add("dataLength", GetDataLength(lines, dispositionIndex).ToString)
                                        End If
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                    Catch ex As Exception
                        ServerFaultMessage = "MultipartFormData format Error"
                    End Try
                End Using
            End If

            Return result
        End Function

    End Class
End Namespace
