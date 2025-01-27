' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports Microsoft.VisualBasic.Devices

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  Class that controls the thread that does the actual work of downloading or uploading.
    ''' </summary>
    Friend NotInheritable Class WebClientCopy

        ' Dialog shown if user wants to see progress UI. Allows the user to cancel the file transfer.
        Private WithEvents _progressDialog As ProgressDialog

        ' The WebClient performs the downloading or uploading operations for us
        Private WithEvents _webClient As WebClient

        ' Keeps track of the error that happened during upload/download so we can throw it
        ' once we can guarantee we are back on the main thread
        Private _exceptionEncounteredDuringFileTransfer As Exception

        ' The percentage of the operation completed
        Private _percentage As Integer

        ' Used for invoking ProgressDialog.Increment
        Private Delegate Sub DoIncrement(Increment As Integer)

        ''' <summary>
        '''  Creates an instance of a WebClientCopy, used to download or upload a file.
        ''' </summary>
        ''' <param name="client">The <see cref="WebClient"/> used to do the downloading or uploading.</param>
        ''' <param name="dialog">UI for indicating progress.</param>
        Public Sub New(client As WebClient, dialog As ProgressDialog)
            Debug.Assert(client IsNot Nothing, $"No {client}")

            _webClient = client
            _progressDialog = dialog
        End Sub

        ''' <summary>
        '''  If the user clicks cancel on the Progress dialog, we need to cancel
        '''  the current async file transfer operation.
        ''' </summary>
        ''' <remarks>
        '''  Note: that we don't want to close the progress dialog here. Wait until
        '''  the actual file transfer cancel event comes through and do it there.
        ''' </remarks>
        Private Sub _progressDialog_UserHitCancel() Handles _progressDialog.UserHitCancel
            ' Cancel the upload/download transfer. We'll close the ProgressDialog
            ' as soon as the WebClient cancels the xfer.
            _webClient.CancelAsync()
        End Sub

        ''' <summary>
        '''  Handles the WebClient's DownloadFileCompleted event.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub _webClient_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) _
            Handles _webClient.DownloadFileCompleted

            Try
                ' If the download was interrupted by an exception, keep track of the exception,
                ' which we'll throw from the main thread
                If e.Error IsNot Nothing Then
                    _exceptionEncounteredDuringFileTransfer = e.Error
                End If

                If Not e.Cancelled AndAlso e.Error Is Nothing Then
                    InvokeIncrement(100)
                End If
            Finally
                'We don't close the dialog until we receive the WebClient.DownloadFileCompleted event
                CloseProgressDialog(_progressDialog)
            End Try
        End Sub

        ''' <summary>
        '''  Handles event WebClient fires whenever progress of download changes.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub _webClient_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) _
            Handles _webClient.DownloadProgressChanged

            InvokeIncrement(e.ProgressPercentage)
        End Sub

        ''' <summary>
        '''  Handles the WebClient's UploadFileCompleted event.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub _webClient_UploadFileCompleted(sender As Object, e As UploadFileCompletedEventArgs) _
            Handles _webClient.UploadFileCompleted

            ' If the upload was interrupted by an exception, keep track of the exception,
            ' which we'll throw from the main thread
            Try
                If e.Error IsNot Nothing Then
                    _exceptionEncounteredDuringFileTransfer = e.Error
                End If
                If Not e.Cancelled AndAlso e.Error Is Nothing Then
                    InvokeIncrement(100)
                End If
            Finally
                ' We don't close the dialog until we receive the
                ' WebClient.DownloadFileCompleted event
                CloseProgressDialog(_progressDialog)
            End Try
        End Sub

        ''' <summary>
        '''  Handles event WebClient fires whenever progress of upload changes.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub _webClient_UploadProgressChanged(sender As Object, e As UploadProgressChangedEventArgs) _
            Handles _webClient.UploadProgressChanged

            Dim increment As Long = (e.BytesSent * 100) \ e.TotalBytesToSend
            InvokeIncrement(CInt(increment))
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
                        _progressDialog.BeginInvoke(
                            New DoIncrement(AddressOf _progressDialog.Increment),
                            increment)
                    End If

                End If
            End If
        End Sub

        ''' <summary>
        ''' Downloads a file.
        ''' </summary>
        ''' <param name="address">The source for the file.</param>
        ''' <param name="destinationFileName">The path and name where the file is saved.</param>
        Public Sub DownloadFile(address As Uri, destinationFileName As String)
            Debug.Assert(_webClient IsNot Nothing, $"No {NameOf(_webClient)}")
            Debug.Assert(address IsNot Nothing, $"No {NameOf(address)}")
            Dim path As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(destinationFileName))
            Debug.Assert((Not String.IsNullOrWhiteSpace(destinationFileName)) _
                AndAlso IO.Directory.Exists(path), $"Invalid {NameOf(path)}")

            ' If we have a dialog we need to set up an async download
            If _progressDialog IsNot Nothing Then
                _webClient.DownloadFileAsync(address, destinationFileName)
                'returns when the download sequence is over, whether due to success, error, or being canceled
                _progressDialog.ShowProgressDialog()
            Else
                _webClient.DownloadFile(address, destinationFileName)
            End If

            'Now that we are back on the main thread, throw the exception we encountered if the user didn't cancel.
            If _exceptionEncounteredDuringFileTransfer IsNot Nothing Then
                If _progressDialog Is Nothing OrElse Not _progressDialog.UserCanceledTheDialog Then
                    Throw _exceptionEncounteredDuringFileTransfer
                End If
            End If

        End Sub

        ''' <summary>
        '''  Uploads a file.
        ''' </summary>
        ''' <param name="sourceFileName">The name and path of the source file.</param>
        ''' <param name="address">The address to which the file is uploaded.</param>
        Public Sub UploadFile(sourceFileName As String, address As Uri)
            Debug.Assert(_webClient IsNot Nothing, $"No {NameOf(_webClient)}")
            Debug.Assert(address IsNot Nothing, $"No {NameOf(address)}")
            Debug.Assert((Not String.IsNullOrWhiteSpace(sourceFileName)) _
                AndAlso IO.File.Exists(sourceFileName), "Invalid file")

            ' If we have a dialog we need to set up an async download
            If _progressDialog IsNot Nothing Then
                _webClient.UploadFileAsync(address, sourceFileName)

                ' Returns when the download sequence is over,
                ' whether due to success, error, or being canceled
                _progressDialog.ShowProgressDialog()
            Else
                _webClient.UploadFile(address, sourceFileName)
            End If

            ' Now that we are back on the main thread, throw the exception we
            ' encountered if the user didn't cancel.
            If _exceptionEncounteredDuringFileTransfer IsNot Nothing Then
                If _progressDialog Is Nothing OrElse Not _progressDialog.UserCanceledTheDialog Then
                    Throw _exceptionEncounteredDuringFileTransfer
                End If
            End If
        End Sub

    End Class
End Namespace
