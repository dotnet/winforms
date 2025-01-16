' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    Partial Public Class Network
        ''' <summary>
        '''  Uploads a file to the network to the specified path.
        ''' </summary>
        ''' <param name="sourceFileName">Name and path of file to be uploaded.</param>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="clientHandler">An HttpClientHandler of the user performing the download.</param>
        ''' <param name="dialog">Progress Dialog.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing).</param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Friend Shared Async Function UploadFileAsync(
            sourceFileName As String,
            addressUri As Uri,
            clientHandler As HttpClientHandler,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            onUserCancel As UICancelOption,
            Optional cancelToken As CancellationToken = Nothing) As Task

            If cancelToken = Nothing Then
                cancelToken = New CancellationTokenSource().Token
            End If

            If connectionTimeout <= 0 Then
                Throw VbUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If addressUri Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(addressUri))
            End If

            Dim normalizedFilePath As String = FileSystemUtils.NormalizeFilePath(sourceFileName, NameOf(sourceFileName))

            ' Make sure we have a meaningful file.
            If String.IsNullOrEmpty(normalizedFilePath) Then
                Throw VbUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            Dim client As HttpClient = If(clientHandler Is Nothing,
                                          New HttpClient(),
                                          New HttpClient(clientHandler))
            client.Timeout = New TimeSpan(0, 0, 0, 0, connectionTimeout)

            ' Create the copier
            Dim copier As New HttpClientCopy(client, dialog)

            ' Upload the file
            Try
                Await copier.UploadFileWorkerAsync(
                    filePath:=normalizedFilePath,
                    requestURI:=addressUri,
                    externalToken:=cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
            Catch ioEx As IO.IOException
                Throw

            Catch ex As Exception
                If onUserCancel = UICancelOption.ThrowException OrElse (dialog IsNot Nothing AndAlso Not dialog.UserCanceledTheDialog) Then
                    If ex.Message.Contains("401") Then
                        Throw New WebException(SR.net_webstatus_Unauthorized, WebExceptionStatus.ProtocolError)
                    End If
                    Throw
                End If
                If ex.Message.Contains("401") Then
                    Throw New WebException(SR.net_webstatus_Unauthorized)
                End If
            Finally
                CloseProgressDialog(dialog)
            End Try

        End Function

    End Class
End Namespace
