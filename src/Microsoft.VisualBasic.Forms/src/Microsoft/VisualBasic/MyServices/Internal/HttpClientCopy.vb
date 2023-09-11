' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Threading

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  Class that controls the thread that does the actual work of downloading or uploading.
    ''' </summary>
    Friend NotInheritable Class HttpClientCopy

        ''' <summary>
        ''' Creates an instance of a HttpClientCopy, used to download or upload a file
        ''' </summary>
        ''' <param name="client">The HttpClient used to do the downloading or uploading</param>
        ''' <param name="dialog">UI for indicating progress</param>
        Public Sub New(client As HttpClient, dialog As ProgressDialog)

            Debug.Assert(client IsNot Nothing, "No HttpClient")

            _httpClient = client
            m_ProgressDialog = dialog
            If m_ProgressDialog IsNot Nothing Then
                AddHandler m_ProgressDialog.UserHitCancel, AddressOf m_ProgressDialog_UserHitCancel
            End If

        End Sub

        ''' <summary>
        ''' Downloads a file
        ''' </summary>
        ''' <param name="address">The source for the file</param>
        ''' <param name="destinationFileName">The path and name where the file is saved</param>
        Public Async Function DownloadFileAsync(address As Uri, destinationFileName As String) As Task
            Debug.Assert(_httpClient IsNot Nothing, "No HttpClient")
            Debug.Assert(address IsNot Nothing, "No address")
            Debug.Assert((Not String.IsNullOrWhiteSpace(destinationFileName)) AndAlso Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(destinationFileName))), "Invalid path")

            _cancelTokenSourceGet = New CancellationTokenSource()
            _cancelTokenSourceRead = New CancellationTokenSource()
            _cancelTokenSourceReadStream = New CancellationTokenSource()
            _cancelTokenSourceWrite = New CancellationTokenSource()

            Dim response As HttpResponseMessage = Nothing
            Try
                response = Await _httpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead, _cancelTokenSourceGet.Token).ConfigureAwait(False)
            Catch ex As TaskCanceledException
                If ex.CancellationToken = _cancelTokenSourceRead.Token Then
                    ' a real cancellation, triggered by the caller
                    Throw
                Else
                    Throw New WebException(SR.net_webstatus_Timeout, WebExceptionStatus.Timeout)
                End If
            Catch ex As Exception
                Throw
            End Try
            Dim contentLength? As Long = response?.Content.Headers.ContentLength
            If contentLength.HasValue Then
                Using responseStream As Stream = Await response.Content.ReadAsStreamAsync(_cancelTokenSourceReadStream.Token).ConfigureAwait(False)
                    Using fileStream As New FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.None)

                        Dim buffer(8191) As Byte
                        Dim totalBytesRead As Long = 0
                        Dim bytesRead As Integer
                        Try
                            bytesRead = Await responseStream.ReadAsync(buffer.AsMemory(0, buffer.Length), _cancelTokenSourceRead.Token).ConfigureAwait(False)
                            Do While bytesRead > 0
                                totalBytesRead += bytesRead

                                Await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), _cancelTokenSourceWrite.Token).ConfigureAwait(False)
                                If m_ProgressDialog IsNot Nothing Then
                                    Dim percentage As Integer = CInt(contentLength.Value / totalBytesRead * 100)
                                    InvokeIncrement(percentage)
                                End If
                                bytesRead = Await responseStream.ReadAsync(buffer.AsMemory(0, buffer.Length), _cancelTokenSourceRead.Token).ConfigureAwait(False)
                            Loop
                        Catch ex As Exception
                            Throw
                        Finally
                            CloseProgressDialog()
                        End Try
                    End Using
                End Using
            End If
            response?.Dispose()
            If m_ProgressDialog IsNot Nothing Then
                RemoveHandler m_ProgressDialog.UserHitCancel, AddressOf m_ProgressDialog_UserHitCancel
            End If

        End Function

#If False Then
        ''' <summary>
        ''' Uploads a file
        ''' </summary>
        ''' <param name="sourceFileName">The name and path of the source file</param>
        ''' <param name="address">The address to which the file is uploaded</param>
        Public Sub UploadFile(sourceFileName As String, address As Uri)
            Debug.Assert(m_WebClient IsNot Nothing, "No WebClient")
            Debug.Assert(address IsNot Nothing, "No address")
            Debug.Assert((Not String.IsNullOrWhiteSpace(sourceFileName)) AndAlso File.Exists(sourceFileName), "Invalid file")

            ' If we have a dialog we need to set up an async download
            If m_ProgressDialog IsNot Nothing Then
                m_WebClient.UploadFileAsync(address, sourceFileName)
                m_ProgressDialog.ShowProgressDialog() 'returns when the download sequence is over, whether due to success, error, or being canceled
            Else
                m_WebClient.UploadFile(address, sourceFileName)
            End If

            'Now that we are back on the main thread, throw the exception we encountered if the user didn't cancel.
            If _exceptionEncounteredDuringFileTransfer IsNot Nothing Then
                If m_ProgressDialog Is Nothing OrElse Not m_ProgressDialog.UserCanceledTheDialog Then
                    Throw _exceptionEncounteredDuringFileTransfer
                End If
            End If
        End Sub

#End If

        ''' <summary>
        '''  Notifies the progress dialog to increment the progress bar
        ''' </summary>
        ''' <param name="progressPercentage">The percentage of bytes read</param>
        Private Sub InvokeIncrement(progressPercentage As Integer)
            ' Don't invoke unless dialog is up and running
            If m_ProgressDialog IsNot Nothing Then
                If m_ProgressDialog.IsHandleCreated Then

                    ' For performance, don't invoke if increment is 0
                    Dim increment As Integer = progressPercentage - _percentage
                    _percentage = progressPercentage
                    If increment > 0 Then
                        m_ProgressDialog.BeginInvoke(New DoIncrement(AddressOf m_ProgressDialog.Increment), increment)
                    End If

                End If
            End If
        End Sub

        ''' <summary>
        '''  Posts a message to close the progress dialog
        ''' </summary>
        Private Sub CloseProgressDialog()
            ' Don't invoke unless dialog is up and running
            If m_ProgressDialog IsNot Nothing Then
                m_ProgressDialog.IndicateClosing()

                If m_ProgressDialog.IsHandleCreated Then
                    m_ProgressDialog.BeginInvoke(New System.Windows.Forms.MethodInvoker(AddressOf m_ProgressDialog.CloseDialog))
                Else
                    ' Ensure dialog is closed. If we get here it means the file was copied before the handle for
                    ' the progress dialog was created.
                    m_ProgressDialog.Close()
                End If
            End If
        End Sub

        ''' <summary>
        ''' If the user clicks cancel on the Progress dialog, we need to cancel
        ''' the current async file transfer operation
        ''' </summary>
        ''' <remarks>
        ''' Note that we don't want to close the progress dialog here.  Wait until
        ''' the actual file transfer cancel event comes through and do it there.
        ''' </remarks>
        Private Sub m_ProgressDialog_UserHitCancel()
            'cancel the upload/download transfer.  We'll close the ProgressDialog as soon as the HttpClient cancels the xfer.
            _cancelTokenSourceGet.Cancel()
            _cancelTokenSourceRead.Cancel()
            _cancelTokenSourceReadStream.Cancel()
            _cancelTokenSourceWrite.Cancel()
        End Sub

        Private _cancelTokenSourceGet As CancellationTokenSource
        Private _cancelTokenSourceRead As CancellationTokenSource
        Private _cancelTokenSourceReadStream As CancellationTokenSource
        Private _cancelTokenSourceWrite As CancellationTokenSource

        ' The WebClient performs the downloading or uploading operations for us
        Private ReadOnly _httpClient As HttpClient

        ' Dialog shown if user wants to see progress UI.  Allows the user to cancel the file transfer.
        Private WithEvents m_ProgressDialog As ProgressDialog

        ' Used for invoking ProgressDialog.Increment
        Private Delegate Sub DoIncrement(Increment As Integer)

        ' The percentage of the operation completed
        Private _percentage As Integer

    End Class

End Namespace
