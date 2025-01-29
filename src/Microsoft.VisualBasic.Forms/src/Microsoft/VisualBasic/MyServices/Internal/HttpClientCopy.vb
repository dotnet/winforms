' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading
Imports Microsoft.VisualBasic.Devices.NetworkUtilities

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  Class that controls the thread that does the actual work of downloading or uploading.
    ''' </summary>
    Friend NotInheritable Class HttpClientCopy

        ' Dialog shown if user wants to see progress UI. Allows the user to cancel the file transfer.
        Private WithEvents _progressDialog As ProgressDialog

        Private Const BufferSize As Integer = 8192

        ' The WebClient performs the downloading or uploading operations for us
        Private ReadOnly _httpClient As HttpClient

        Private _cancelTokenSourceGet As CancellationTokenSource
        Private _cancelTokenSourcePost As CancellationTokenSource
        Private _cancelTokenSourceReadStream As CancellationTokenSource
        Private _cancelTokenSourceWriteStream As CancellationTokenSource

        ' The percentage of the operation completed
        Private _percentage As Integer

        ' Used for invoking ProgressDialog.Increment
        Private Delegate Sub DoIncrement(Increment As Integer)

        ''' <summary>
        '''  Creates an instance of a HttpClientCopy, used to download or upload a file.
        ''' </summary>
        ''' <param name="client">The HttpClient used to do the downloading or uploading.</param>
        ''' <param name="dialog">UI for indicating progress.</param>
        Public Sub New(client As HttpClient, dialog As ProgressDialog)

            Debug.Assert(client IsNot Nothing, "No HttpClient")

            _httpClient = client
            _progressDialog = dialog
            If _progressDialog IsNot Nothing Then
                AddHandler _progressDialog.UserHitCancel, AddressOf _progressDialog_UserHitCancel
            End If

        End Sub

        ''' <summary>
        '''  If the user clicks cancel on the Progress dialog, we need to cancel
        '''  the current async file transfer operation.
        '''  </summary>
        ''' <remarks>
        '''  Note that we don't want to close the progress dialog here. Wait until
        '''  the actual file transfer cancel event comes through and do it there.
        ''' </remarks>
        Private Sub _progressDialog_UserHitCancel()
            ' Cancel the upload/download transfer. We'll close the ProgressDialog
            ' as soon as the HttpClient cancels the xfer.
            _cancelTokenSourceGet?.Cancel()
            _cancelTokenSourceReadStream?.Cancel()
            _cancelTokenSourcePost?.Cancel()
            _cancelTokenSourceWriteStream?.Cancel()
        End Sub

        ''' <summary>
        '''  Notifies the progress dialog to increment the progress bar.
        ''' </summary>
        ''' <param name="progressPercentage">The percentage of bytes read.</param>
        Private Sub InvokeIncrement(progressPercentage As Integer)
            ' Don't invoke unless dialog is up and running
            If _progressDialog IsNot Nothing Then
                If _progressDialog.IsHandleCreated Then

                    ' For performance, don't invoke if increment is 0
                    Dim increment As Integer = progressPercentage - _percentage
                    _percentage = progressPercentage
                    If increment > 0 Then
                        _progressDialog.BeginInvoke(method:=New DoIncrement(AddressOf _progressDialog.Increment), increment)
                    End If

                End If
            End If
        End Sub

        ''' <summary>
        '''  Downloads a file.
        ''' </summary>
        ''' <param name="addressUri">The source for the file.</param>
        ''' <param name="normalizedFilePath">The path and name where the file is to be saved.</param>
        Friend Async Function DownloadFileWorkerAsync(addressUri As Uri, normalizedFilePath As String, externalToken As CancellationToken) As Task
            Debug.Assert(_httpClient IsNot Nothing, "No HttpClient")
            Debug.Assert(addressUri IsNot Nothing, "No address")
            Dim directoryPath As String = Path.GetDirectoryName(Path.GetFullPath(normalizedFilePath))
            Debug.Assert((Not String.IsNullOrWhiteSpace(normalizedFilePath)) AndAlso
                         Directory.Exists(directoryPath), "Invalid path")

            _cancelTokenSourceGet = New CancellationTokenSource()
            _cancelTokenSourceReadStream = New CancellationTokenSource()
            _cancelTokenSourceWriteStream = New CancellationTokenSource()

            Dim response As HttpResponseMessage = Nothing
            Dim totalBytesRead As Long = 0
            Try
                Using linkedCts As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSourceGet.Token, externalToken)
                    response = Await _httpClient.GetAsync(
                    requestUri:=addressUri,
                    completionOption:=HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken:=_cancelTokenSourceGet.Token).ConfigureAwait(continueOnCapturedContext:=False)
                End Using
            Catch ex As TaskCanceledException
                If ex.CancellationToken = externalToken Then
                    externalToken.ThrowIfCancellationRequested()
                ElseIf ex.CancellationToken = _cancelTokenSourceGet.Token Then
                    ' a real cancellation, triggered by the caller
                    Throw
                Else
                    Throw New WebException(SR.net_webstatus_Timeout, WebExceptionStatus.Timeout)
                End If
            End Try
            Select Case response.StatusCode
                Case HttpStatusCode.OK
                    _cancelTokenSourceReadStream = CancellationTokenSource.CreateLinkedTokenSource(New CancellationTokenSource().Token, externalToken)
                    _cancelTokenSourceWriteStream = CancellationTokenSource.CreateLinkedTokenSource(New CancellationTokenSource().Token, externalToken)
                    Dim contentLength? As Long = response?.Content.Headers.ContentLength
                    If contentLength.HasValue Then
                        Using responseStream As Stream = Await response.Content.ReadAsStreamAsync(
                            cancellationToken:=_cancelTokenSourceReadStream.Token).
                                ConfigureAwait(continueOnCapturedContext:=False)

                            Using fileStream As New FileStream(
                                path:=normalizedFilePath,
                                mode:=FileMode.Create,
                                access:=FileAccess.Write,
                                share:=FileShare.None)

                                Dim buffer(BufferSize - 1) As Byte
                                Dim bytesRead As Integer
                                Try
                                    bytesRead = Await responseStream.ReadAsync(
                                        buffer:=buffer.AsMemory(start:=0, buffer.Length),
                                        cancellationToken:=_cancelTokenSourceReadStream.Token).
                                            ConfigureAwait(continueOnCapturedContext:=False)

                                    Do While bytesRead > 0
                                        totalBytesRead += bytesRead

                                        Await fileStream.WriteAsync(
                                            buffer:=buffer.AsMemory(0, bytesRead),
                                            cancellationToken:=_cancelTokenSourceWriteStream.Token).
                                                ConfigureAwait(continueOnCapturedContext:=False)

                                        If _progressDialog IsNot Nothing Then
                                            Dim percentage As Integer = CInt(totalBytesRead / contentLength.Value * 100)
                                            InvokeIncrement(percentage)
                                        End If
                                        bytesRead = Await responseStream.ReadAsync(
                                            buffer:=buffer.AsMemory(start:=0, buffer.Length),
                                            cancellationToken:=_cancelTokenSourceReadStream.Token).
                                                ConfigureAwait(continueOnCapturedContext:=False)
                                    Loop
                                Finally
                                    CloseProgressDialog(_progressDialog)
                                End Try
                            End Using
                        End Using

                        If _progressDialog IsNot Nothing Then
                            RemoveHandler _progressDialog.UserHitCancel, AddressOf _progressDialog_UserHitCancel
                        End If
                    End If
                Case HttpStatusCode.NotFound
                    Throw New WebException(SR.net_webstatus_NotFound)
                Case HttpStatusCode.Unauthorized, HttpStatusCode.NetworkAuthenticationRequired
                    Throw New WebException(SR.net_webstatus_Unauthorized, WebExceptionStatus.ProtocolError)
                Case HttpStatusCode.RequestTimeout
                    Throw New WebException(SR.net_webstatus_Timeout)
                Case Else
                    Throw New WebException()
            End Select
            response?.Dispose()
        End Function

        ''' <summary>
        '''  Uploads a file
        ''' </summary>
        ''' <param name="filePath">The name and path of the source file</param>
        ''' <param name="requestURI">The address to which the file is uploaded</param>
        Public Async Function UploadFileWorkerAsync(filePath As String, requestURI As Uri, externalToken As CancellationToken) As Task
            Debug.Assert(_httpClient IsNot Nothing, "No WebClient")
            Debug.Assert(requestURI IsNot Nothing, "No address")

            _cancelTokenSourceReadStream = New CancellationTokenSource()
            _cancelTokenSourcePost = New CancellationTokenSource()
            Dim contentLength As Long = New FileInfo(filePath).Length
            Dim totalBytesRead As Long = 0
            Dim progress As Action(Of Long, Long) =
                Sub(bytesRead As Long, streamLength As Long)
                    totalBytesRead += bytesRead
                    If _progressDialog IsNot Nothing Then
                        Dim progressPercentage As Integer = CInt(totalBytesRead / streamLength * 100)
                        InvokeIncrement(progressPercentage)
                        Thread.Sleep(millisecondsTimeout:=1)
                    End If
                End Sub
            Using linkedStreamCts As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSourceReadStream.Token, _cancelTokenSourcePost.Token, externalToken)
                Try
                    Dim response As HttpResponseMessage
                    Using multipartContent As New MultipartFormDataContent("----boundary")
                        Dim fileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        Dim fileContent As New StreamContent(fileStream)
                        fileContent.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")
                        Dim fileName As String = Path.GetFileName(filePath)
                        multipartContent.Add(fileContent, "file", $"""{fileName}""")
                        If _progressDialog Is Nothing Then
                            response =
                                Await _httpClient.PostAsync(requestURI, multipartContent, cancellationToken:=linkedStreamCts.Token) _
                                    .ConfigureAwait(continueOnCapturedContext:=False)
                        Else
                            Dim progressContent As New ProgressableStreamContent(multipartContent, progress, BufferSize)
                            response =
                                Await _httpClient.PostAsync(requestURI, progressContent, cancellationToken:=linkedStreamCts.Token) _
                                    .ConfigureAwait(continueOnCapturedContext:=False)
                        End If
                        response.EnsureSuccessStatusCode()
                        Select Case response.StatusCode
                            Case HttpStatusCode.OK
                            Case HttpStatusCode.Unauthorized, HttpStatusCode.NetworkAuthenticationRequired
                                Throw New WebException(SR.net_webstatus_Unauthorized, WebExceptionStatus.ProtocolError)
                            Case HttpStatusCode.RequestTimeout
                                Throw New WebException(SR.net_webstatus_Timeout)
                            Case Else
                                Throw New WebException()
                        End Select
                        response?.Dispose()
                        Await fileStream.DisposeAsync().ConfigureAwait(False)
                    End Using
                Catch ex As HttpRequestException
                    Throw
                Catch ex As TaskCanceledException
                    If ex.CancellationToken = externalToken Then
                        externalToken.ThrowIfCancellationRequested()
                    ElseIf linkedStreamCts.IsCancellationRequested Then
                        ' a real cancellation, triggered by the caller
                        Throw
                    Else
                        Throw New WebException(SR.net_webstatus_Timeout, WebExceptionStatus.Timeout)
                    End If
                Finally
                    CloseProgressDialog(_progressDialog)
                End Try
            End Using
            If _progressDialog IsNot Nothing Then
                RemoveHandler _progressDialog.UserHitCancel, AddressOf _progressDialog_UserHitCancel
            End If
        End Function
    End Class
End Namespace
