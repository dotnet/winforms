' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  Class that controls the thread that does the actual work of downloading or uploading.
    ''' </summary>
    Friend Class WebClientCopy

        ''' <summary>
        ''' Creates an instance of a WebClientCopy, used to download or upload a file
        ''' </summary>
        ''' <param name="client">The WebClient used to do the downloading or uploading</param>
        ''' <param name="dialog">UI for indicating progress</param>
        Public Sub New(client As WebClient, dialog As ProgressDialog)

            Debug.Assert(client IsNot Nothing, "No WebClient")

            m_WebClient = client
            m_ProgressDialog = dialog

        End Sub

        ''' <summary>
        ''' Downloads a file
        ''' </summary>
        ''' <param name="address">The source for the file</param>
        ''' <param name="destinationFileName">The path and name where the file is saved</param>
        Public Sub DownloadFile(address As Uri, destinationFileName As String)
            Debug.Assert(m_WebClient IsNot Nothing, "No WebClient")
            Debug.Assert(address IsNot Nothing, "No address")
            Debug.Assert((Not String.IsNullOrWhiteSpace(destinationFileName)) AndAlso Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(destinationFileName))), "Invalid path")

            ' If we have a dialog we need to set up an async download
            If m_ProgressDialog IsNot Nothing Then
                m_WebClient.DownloadFileAsync(address, destinationFileName)
                m_ProgressDialog.ShowProgressDialog() 'returns when the download sequence is over, whether due to success, error, or being canceled
            Else
                m_WebClient.DownloadFile(address, destinationFileName)
            End If

            'Now that we are back on the main thread, throw the exception we encountered if the user didn't cancel.
            If _exceptionEncounteredDuringFileTransfer IsNot Nothing Then
                If m_ProgressDialog Is Nothing OrElse Not m_ProgressDialog.UserCanceledTheDialog Then
                    Throw _exceptionEncounteredDuringFileTransfer
                End If
            End If

        End Sub

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
                    m_ProgressDialog.BeginInvoke(New MethodInvoker(AddressOf m_ProgressDialog.CloseDialog))
                Else
                    ' Ensure dialog is closed. If we get here it means the file was copied before the handle for
                    ' the progress dialog was created.
                    m_ProgressDialog.Close()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Handles the WebClient's DownloadFileCompleted event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub m_WebClient_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles m_WebClient.DownloadFileCompleted
            Try
                ' If the download was interrupted by an exception, keep track of the exception, which we'll throw from the main thread
                If e.Error IsNot Nothing Then
                    _exceptionEncounteredDuringFileTransfer = e.Error
                End If

                If Not e.Cancelled AndAlso e.Error Is Nothing Then
                    InvokeIncrement(100)
                End If
            Finally
                'We don't close the dialog until we receive the WebClient.DownloadFileCompleted event
                CloseProgressDialog()
            End Try
        End Sub

        ''' <summary>
        ''' Handles event WebClient fires whenever progress of download changes
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub m_WebClient_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles m_WebClient.DownloadProgressChanged
            InvokeIncrement(e.ProgressPercentage)
        End Sub

        ''' <summary>
        ''' Handles the WebClient's UploadFileCompleted event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub m_WebClient_UploadFileCompleted(sender As Object, e As UploadFileCompletedEventArgs) Handles m_WebClient.UploadFileCompleted

            ' If the upload was interrupted by an exception, keep track of the exception, which we'll throw from the main thread
            Try
                If e.Error IsNot Nothing Then
                    _exceptionEncounteredDuringFileTransfer = e.Error
                End If
                If Not e.Cancelled AndAlso e.Error Is Nothing Then
                    InvokeIncrement(100)
                End If
            Finally
                'We don't close the dialog until we receive the WebClient.DownloadFileCompleted event
                CloseProgressDialog()
            End Try
        End Sub

        ''' <summary>
        ''' Handles event WebClient fires whenever progress of upload changes
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub m_WebClient_UploadProgressChanged(sender As Object, e As UploadProgressChangedEventArgs) Handles m_WebClient.UploadProgressChanged
            Dim increment As Long = (e.BytesSent * 100) \ e.TotalBytesToSend
            InvokeIncrement(CInt(increment))
        End Sub

        ''' <summary>
        ''' If the user clicks cancel on the Progress dialog, we need to cancel
        ''' the current async file transfer operation
        ''' </summary>
        ''' <remarks>
        ''' Note that we don't want to close the progress dialog here.  Wait until
        ''' the actual file transfer cancel event comes through and do it there.
        ''' </remarks>
        Private Sub m_ProgressDialog_UserCancelledEvent() Handles m_ProgressDialog.UserHitCancel
            m_WebClient.CancelAsync() 'cancel the upload/download transfer.  We'll close the ProgressDialog as soon as the WebClient cancels the xfer.
        End Sub

        ' The WebClient performs the downloading or uploading operations for us
        Private WithEvents m_WebClient As WebClient

        ' Dialog shown if user wants to see progress UI.  Allows the user to cancel the file transfer.
        Private WithEvents m_ProgressDialog As ProgressDialog

        'Keeps track of the error that happened during upload/download so we can throw it once we can guarantee we are back on the main thread
        Private _exceptionEncounteredDuringFileTransfer As Exception

        ' Used for invoking ProgressDialog.Increment
        Private Delegate Sub DoIncrement(Increment As Integer)

        ' The percentage of the operation completed
        Private _percentage As Integer

    End Class
End Namespace
