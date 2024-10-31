' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports Microsoft.VisualBasic.Devices.NetworkUtilities

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  Class that controls the thread that does the actual work of downloading or uploading.
    ''' </summary>
    Friend NotInheritable Class HttpClientCopy

        ' Dialog shown if user wants to see progress UI. Allows the user to cancel the file transfer.
        Private WithEvents _progressDialog As ProgressDialog

        ' The WebClient performs the downloading or uploading operations for us
        Private ReadOnly _httpClient As HttpClient

        Private _cancelTokenSourceGet As CancellationTokenSource
        Private _cancelTokenSourceRead As CancellationTokenSource
        Private _cancelTokenSourceReadStream As CancellationTokenSource
        Private _cancelTokenSourceWrite As CancellationTokenSource

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
                        _progressDialog.BeginInvoke(New DoIncrement(AddressOf _progressDialog.Increment), increment)
                    End If
                End If
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
            _cancelTokenSourceGet.Cancel()
            _cancelTokenSourceRead.Cancel()
            _cancelTokenSourceReadStream.Cancel()
            _cancelTokenSourceWrite.Cancel()
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
            _cancelTokenSourceRead = New CancellationTokenSource()
            _cancelTokenSourceReadStream = New CancellationTokenSource()
            _cancelTokenSourceWrite = New CancellationTokenSource()

            Dim response As HttpResponseMessage = Nothing

            _cancelTokenSourceGet = New CancellationTokenSource()
            Using linkedCts As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSourceGet.Token, externalToken)
                Try
                    response = Await _httpClient.GetAsync(
                        requestUri:=addressUri,
                        completionOption:=HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken:=_cancelTokenSourceGet.Token).ConfigureAwait(continueOnCapturedContext:=False)

                Catch ex As TaskCanceledException
                    If ex.CancellationToken = externalToken Then
                        externalToken.ThrowIfCancellationRequested()
                    ElseIf ex.CancellationToken = _cancelTokenSourceGet.Token Then
                        ' a real cancellation, triggered by the caller
                        ' a real cancellation, triggered by the caller
                        Throw
                    Else
                        Throw New WebException(SR.net_webstatus_Timeout, WebExceptionStatus.Timeout)
                    End If
                End Try
            End Using
            Select Case response.StatusCode
                Case HttpStatusCode.OK
                    _cancelTokenSourceRead = CancellationTokenSource.CreateLinkedTokenSource(New CancellationTokenSource().Token, externalToken)
                    _cancelTokenSourceReadStream = CancellationTokenSource.CreateLinkedTokenSource(New CancellationTokenSource().Token, externalToken)
                    _cancelTokenSourceWrite = CancellationTokenSource.CreateLinkedTokenSource(New CancellationTokenSource().Token, externalToken)
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

                                Dim buffer(8191) As Byte
                                Dim totalBytesRead As Long = 0
                                Dim bytesRead As Integer
                                Try
                                    bytesRead = Await responseStream.ReadAsync(
                                        buffer:=buffer.AsMemory(start:=0, buffer.Length),
                                        cancellationToken:=_cancelTokenSourceRead.Token).
                                            ConfigureAwait(continueOnCapturedContext:=False)

                                    Do While bytesRead > 0
                                        totalBytesRead += bytesRead

                                        Await fileStream.WriteAsync(
                                            buffer:=buffer.AsMemory(0, bytesRead),
                                            cancellationToken:=_cancelTokenSourceWrite.Token).
                                                ConfigureAwait(continueOnCapturedContext:=False)

                                        If _progressDialog IsNot Nothing Then
                                            Dim percentage As Integer = CInt(totalBytesRead / contentLength.Value * 100)
                                            InvokeIncrement(percentage)
                                        End If
                                        bytesRead = Await responseStream.ReadAsync(
                                            buffer:=buffer.AsMemory(start:=0, buffer.Length),
                                            cancellationToken:=_cancelTokenSourceRead.Token).
                                                ConfigureAwait(continueOnCapturedContext:=False)
                                    Loop
                                Finally
                                    CloseProgressDialog(_progressDialog)
                                End Try
                            End Using
                        End Using
                        If response.StatusCode = HttpStatusCode.OK Then
                        End If

                        If _progressDialog IsNot Nothing Then
                            RemoveHandler _progressDialog.UserHitCancel, AddressOf _progressDialog_UserHitCancel
                        End If
                    End If

                Case HttpStatusCode.Unauthorized
                    Throw New WebException(SR.net_webstatus_Unauthorized, WebExceptionStatus.ProtocolError)
                Case Else
                    Throw New WebException()
            End Select
            response?.Dispose()
        End Function

#If False Then ' Future code to implement upload using Http
        ''' <summary>
        '''  Uploads a file
        ''' </summary>
        ''' <param name="sourceFileName">The name and path of the source file</param>
        ''' <param name="address">The address to which the file is uploaded</param>
        Public Sub UploadFile(sourceFileName As String, address As Uri)
            Debug.Assert(_httpClient IsNot Nothing, "No WebClient")
            Debug.Assert(address IsNot Nothing, "No address")
            Debug.Assert((Not String.IsNullOrWhiteSpace(sourceFileName)) AndAlso
                File.Exists(sourceFileName), "Invalid file")

            ' If we have a dialog we need to set up an async download
            If _progressDialog IsNot Nothing Then
                _httpClient.UploadFileAsync(address, sourceFileName)
                 ' returns when the download sequence is over, whether due to success, error, or being canceled
                _progressDialog.ShowProgressDialog()
            Else
                _httpClient.UploadFile(address, sourceFileName)
            End If

            ' Now that we are back on the main thread, throw the exception we encountered if the user didn't cancel.
            If _exceptionEncounteredDuringFileTransfer IsNot Nothing Then
                If _progressDialog Is Nothing OrElse Not _progressDialog.UserCanceledTheDialog Then
                    Throw _exceptionEncounteredDuringFileTransfer
                End If
            End If
        End Sub
#End If

    End Class
End Namespace
